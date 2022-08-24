﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TODO: Find better objects than List<T> for most of the below
namespace BeatGraphs.Modules
{
    /// <summary>
    /// The Builder class's job is to read the scores in the database and populate the member lists with the data required
    /// to create the output files. Once the data is collected and processed, the Writer class is called to output the .php
    /// and .png files for that instnace. The Builder can take in parameters in a way that allows it to run multiple sets
    /// in a single go.
    /// </summary>
    public static class Builder
    {
        public static BeatGraphForm form;   // Used only to allow logging to the form.

        // Private lists are used for calculation purposes only
        private static List<List<int>> allPaths = new List<List<int>>();    // Tracks all paths involved in loops at each loop size
        private static List<List<int>> beatLoops = new List<List<int>>();   // Tracks surviving loops at each loop size
        private static List<bool> countedPoints = new List<bool>();         // Tracks whether or not a team has had its points counted
        private static List<int> failedLoopTeams = new List<int>();         // Tracks which teams haven't been completed in finding loops
        private static List<int> currentPath = new List<int>();             // Tracks teams involved in the current path while traversing trees
        private static List<Game> beatList = new List<Game>();              // Tracks links and their weights that will be used to resolve the loops

        // Public lists are needed during the Write phase and are the "output" of the Builder class
        public static List<List<int>> beatLoopList = new List<List<int>>(); // Tracks all of the loops broken in a run
        public static List<List<int>> longestPath = new List<List<int>>();  // Tracks all of the paths sharing the most amount of teams
        public static List<double> lossPoints = new List<double>();         // List of each team's path weight coming in (losses)
        public static List<double> winPoints = new List<double>();          // List of each team's path weight going out (wins)
        public static List<Team> matrix = new List<Team>();                 // All of the teams' data, primarly required for the ScoreList
        public static List<int> teams = new List<int>();                    // List of the teamIDs for index lookups.

        /// <summary>
        /// Entry point for the Builder class. Iterates through requested input and does the calculations followed by a call to the
        /// Writer class for each.
        /// </summary>
        /// <param name="leagues">Which leagues will be processed in the run</param>
        /// <param name="seasons">Which seasons will be processed in the run</param>
        /// <param name="methods">Which methods will be processed in the run</param>
        /// <param name="range">Which range of weeks will be processed in the run</param>
        /// <param name="bgForm">The main form for logging purposes</param>
        public static void Run(List<string> leagues, List<string> seasons, List<Method> methods, int range, BeatGraphForm bgForm)
        {
            form = bgForm;

            foreach (var season in seasons)
            {
                foreach (var league in leagues)
                {
                    foreach (var method in methods)
                    {
                        var weeks = new List<int>();

                        if (range > 0) // Specific Week
                        {
                            weeks.Add(range);
                        }
                        else
                        {
                            var availableWeeks = Helpers.GetWeeks(int.Parse(season), league);
                            if (range == 0) // All weeks up to current
                            {
                                availableWeeks.Reverse();
                                weeks = availableWeeks;
                            }
                            else // Just the most current week (passed in as range == -1)
                            {
                                if (availableWeeks.Count > 0)
                                    weeks.Add(availableWeeks[0]);
                            }
                        }

                        foreach (var week in weeks)
                        {
                            // Now that we know the exact details for the run, build the data and write the files.
                            form.Log($"Building the {method.ToString()} graph of Week {week} of the {season} {league} season.");

                            BuildFiles(league, season, method, week.ToString());
                            Writer.ProcessFiles(league, season, method, week.ToString(), bgForm);
                        }
                    }
                }
            }
            form.Log($"Graph building complete.");
        }

        /// <summary>
        /// Starting point for every single entry of the run. Clears data from any previous runs and makes the calls
        /// out to the build steps.
        /// </summary>
        private static void BuildFiles(string league, string season, Method method, string week)
        {
            // Reset output data
            longestPath.Clear();
            teams.Clear();
            matrix.Clear();
            beatLoopList.Clear();

            LoadScores(league, season, method, week);
            ResolveLoops(method);
            CalculateScores();

            //TODO: See if Iterative and Weighted can be calculated now that NCAA isn't a thing anymore
            if (method == Method.Standard)
                FindLongestPaths();
        }

        /// <summary>
        /// Get the teams from the database to find the list of teams and initialize the matrix. Trigger call to
        /// load the games.
        /// </summary>
        private static void LoadScores(string league, string season, Method method, string week)
        {
            teams = Helpers.GetTeams(league, season);

            foreach (var team in teams)
            {
                matrix.Add(new Team(team, league, int.Parse(season)));
                matrix[teams.IndexOf(team)].BuildScoreList(teams);
            }

            LoadGames(league, season, method, week);
        }

        /// <summary>
        /// Get the games from the database and load them into the matrix. Reduce scores between each pair of teams
        /// until one of them is at 0. Any team with points remaining is considered to have a "BeatWin" over the team
        /// with 0.
        /// </summary>
        private static void LoadGames(string league, string season, Method method, string week)
        {            
            foreach (var game in Helpers.GetGames(league, season, week))
            {
                // Weighted method keeps the actual game score, other methods, winners get 1 point, losers get 0.
                // TODO: Check if this is necessary.. I don't think we're using this directly
                if (method != Method.Weighted && game.weight != 0)
                {
                    game.weight = 1;
                }

                matrix[teams.IndexOf(game.winner)].AddScore(game.loser, game.weight);
            }

            //Reduce teams with lower score than opponent to 0 and higher team by the same amount.
            //Doing this also removes "season splits" also known as "2-team loops".
            for (int i = 0; i < teams.Count; i++)
            {
                for (int j = 0; j < teams.Count; j++)
                {
                    if (matrix[i].ScoreList[teams[j]] >= matrix[j].ScoreList[teams[i]])
                    {
                        matrix[i].ScoreList[teams[j]] = matrix[i].ScoreList[teams[j]] - matrix[j].ScoreList[teams[i]];
                        matrix[j].ScoreList[teams[i]] = 0;
                    }
                    else
                    {
                        matrix[j].ScoreList[teams[i]] = matrix[j].ScoreList[teams[i]] - matrix[i].ScoreList[teams[j]];
                        matrix[i].ScoreList[teams[j]] = 0;
                    }
                }
            }

            form.Log("Games loaded from database.", LogLevel.verbose);
        }

        /// <summary>
        /// Starting at size 3, find all loops of each size and break them until there are no remaining loops.
        /// </summary>
        private static void ResolveLoops(Method method)
        {
            int iLoopSize;
            int teamA;
            int teamB;

            iLoopSize = 3;  // Loops of size 3 are the smallest we can have, so let's start there (Size 2 loops, "head-to-head splits", are reduced automatically in score gathering)
            allPaths.Clear();  // Empty the tracker.  This will be cleared for each time we increase the loop size

            // While there are loops of any size
            while (isLoop())
            {
                form.Log($"We're starting off with these ambiguous BeatWins for LoopSize {iLoopSize}", LogLevel.verbose);
                beatLoops.Clear();  // Clears the amount of BeatLoops of this size

                // Go through all of the teams
                for (int i = 0; i < teams.Count; i++)
                {
                    // We have to clear the current path tracker outside of FindPathHelper because it is recursive.
                    currentPath.Clear();
                    FindPathHelper(teams[i], teams[i], iLoopSize); // Find all BeatLoops of a specific size from a team to itself
                }
                // That will have populated allPaths with all of the loops of the size in question

                // For each loop in the list
                for (int i = 0; i < allPaths.Count; i++)
                {
                    for (int j = i + 1; j < allPaths.Count; j++)
                    {
                        // Compare it to every other loop.  If the two loops contain the same teams, remove the duplicate loop from the list.
                        if (EqualLoop(new List<int>(allPaths[i]), new List<int>(allPaths[j])))
                        {
                            allPaths.RemoveAt(j--);
                        }
                    }

                    // Any loops that survive this long are logged, once for the list to be broken, one for the list to report later.
                    beatLoops.Add(new List<int>(allPaths[i]));
                    beatLoopList.Add(new List<int>(allPaths[i]));

                    // Record all BeatWins involved in all of the BeatLoops
                    for (int j = 0; j < allPaths[i].Count() - 1; j++)
                    {
                        teamA = allPaths[i][j];
                        teamB = allPaths[i][j + 1];

                        // Only add the BeatWin to the list if it's not already there, otherwise, build a list of all links involved in loops at this size
                        if (!InBeatList(teamA, teamB))
                        {
                            form.Log($"{matrix[teams.IndexOf(teamA)].abbreviation}({teamA})->{matrix[teams.IndexOf(teamB)].abbreviation}({teamB})", LogLevel.verbose);
                            beatList.Add(new Game(teamA, teamB, matrix[teams.IndexOf(teamA)].ScoreList[teamB]));
                        }  // beatList is a list of links and their weights that will be used to resolve the loops
                    }
                }

                // Resolve loops as long as there are loops to resolve
                while (beatLoops.Count > 0)
                {
                    if (method == Method.Iterative)
                        ResolveIterative();
                    else
                        ResolveStandard(); // All other methods use the Standard method to resolve loops, the difference is the weights given to the links
                }
                iLoopSize++; // Increase the loop size for the next pass
                allPaths.Clear(); // Clear the list of paths
            }
        }

        /// <summary>
        /// The standard method of resolving loops means each path in the loop is reduced in weight equally until one path is at weight 0.
        /// </summary>
        private static void ResolveStandard()
        {
            int teamA;
            int teamB;
            double minPoints = FindMinPoints(); // Get the lowest weight for the set of paths in remaining loops.

            form.Log($"Found minimum points of {minPoints} and subtracting from all involved BeatLoop strengths.", LogLevel.verbose);
            form.Log($"These BeatLoops were broken in this pass:", LogLevel.verbose);

            // Reduce all paths by the minimum weight
            for (int i = 0; i < beatList.Count; i++)
            {
                teamA = teams.IndexOf(beatList[i].winner);
                teamB = teams.IndexOf(beatList[i].loser);
                matrix[teamA].ScoreList[teams[teamB]] = matrix[teamA].ScoreList[teams[teamB]] - minPoints;
            }

            // Any paths that have their weights reduced to 0 have their BeatWin removed, breaking the loop.
            for (int i = 0; i < beatLoops.Count; i++)
            {
                if (LoopContainsZero(beatLoops[i]))
                {
                    form.Log(string.Join("->", beatLoops[i]), LogLevel.verbose);
                    beatLoops.RemoveAt(i--);
                }
            }
            form.Log($"Which leaves these ambiguous games on the BeatList:", LogLevel.verbose);
            beatList.Clear();

            // Reload the beatList with games still involved in loops.
            for (int i = 0; i < beatLoops.Count; i++)
            {
                for (int j = 0; j < (beatLoops[i].Count() - 1); j++)
                {
                    teamA = beatLoops[i][j];
                    teamB = beatLoops[i][j + 1];
                    if (!InBeatList(teamA, teamB))
                    {
                        form.Log($"{matrix[teams.IndexOf(teamA)].abbreviation}({teamA})->{matrix[teams.IndexOf(teamB)].abbreviation}({teamB})", LogLevel.verbose);
                        beatList.Add(new Game(teamA, teamB, matrix[teams.IndexOf(teamA)].ScoreList[teamB]));
                    }
                }
            }
        }

        // TODO: See if ResolveIterative and ResolveStandard can be combined to reduce code duplication
        /// <summary>
        /// Resolving loops iteratively means that paths that occur more frequently in loops of the given size are broken first.
        /// </summary>
        private static void ResolveIterative()
        {
            int teamA;
            int teamB;
            double minWeight = FindMinWeight(); // Get the lowest weight for the set of paths in remaining loops.

            form.Log($"Found minimum weight of {minWeight} and subtracting from all involved BeatLoop strengths.", LogLevel.verbose);
            form.Log($"These BeatLoops were broken in this pass:", LogLevel.verbose);

            // Reduce all paths by the minimum weight
            for (int i = 0; i < beatLoops.Count; i++)
            {
                for (int j = 0; j < beatLoops[i].Count() - 1; j++)
                {
                    teamA = teams.IndexOf(beatLoops[i][j]);
                    teamB = teams.IndexOf(beatLoops[i][j + 1]);
                    matrix[teamA].ScoreList[teams[teamB]] = matrix[teamA].ScoreList[teams[teamB]] - minWeight;
                    // Thanks to computer math, sometimes what should be 0 comes out as only near 0. Adjust to 0 if below this threshhold.
                    if (matrix[teamA].ScoreList[teams[teamB]] < 0.00001)
                        matrix[teamA].ScoreList[teams[teamB]] = 0;
                }
            }

            // Any paths that have their weights reduced to 0 have their BeatWin removed, breaking the loop.
            for (int i = 0; i < beatLoops.Count; i++)
            {
                if (LoopContainsZero(beatLoops[i]))
                {
                    form.Log(string.Join("->", beatLoops[i]), LogLevel.verbose);
                    beatLoops.RemoveAt(i--);
                }
            }
            form.Log($"Which leaves these ambiguous games on the BeatList:", LogLevel.verbose);
            beatList.Clear();

            // Reload the beatList with games still involved in loops.
            for (int i = 0; i < beatLoops.Count; i++)
            {
                for (int j = 0; j < beatLoops[i].Count() - 1; j++)
                {
                    teamA = beatLoops[i][j];
                    teamB = beatLoops[i][j + 1];
                    if (!InBeatList(teamA, teamB))
                    {
                        form.Log($"{matrix[teams.IndexOf(teamA)].abbreviation}({teamA})->{matrix[teams.IndexOf(teamB)].abbreviation}({teamB})", LogLevel.verbose);
                        beatList.Add(new Game(teamA, teamB, matrix[teams.IndexOf(teamA)].ScoreList[teamB]));
                    }
                }
            }
        }

        /// <summary>
        /// Find the fewest amount of points/wins among all items in the BeatList. The BeatList only contains BeatWins involved in BeatLoops.
        /// </summary>
        private static double FindMinPoints()
        {
            double dMinPoints;
            dMinPoints = beatList[0].weight;

            for (int i = 0; i < beatList.Count; i++)
            {
                if (beatList[i].weight < dMinPoints)
                {
                    dMinPoints = beatList[i].weight;
                }
            }
            return dMinPoints;
        }

        /// <summary>
        /// Find the lowest weight among all items in the BeatList. The BeatList only contains BeatWins involved in BeatLoops.
        /// If
        /// </summary>
        private static double FindMinWeight()
        {
            List<List<double>> beatWeights = new List<List<double>>();  // Replaces use of beatList to track BeatWin weights. 
            double minWeight = 9999;    // Set to an arbirarily high number so the first weight tested will be lower than it
            int teamA, teamB;

            // Initialize beatWeights
            for (int i = 0; i < matrix.Count; i++)
            {
                beatWeights.Add(new List<double>());
                foreach (Team tTeam in matrix)
                {
                    beatWeights[i].Add(0);
                }
            }

            // For each path in each loop, increment the occurrences of that path, then divide the existing weight of the path
            // by the occurrences to find the new weight.
            for (int i = 0; i < beatLoops.Count; i++)
            {
                for (int j = 0; j < (beatLoops[i].Count() - 1); j++)
                {
                    teamA = teams.IndexOf(beatLoops[i][j]);
                    teamB = teams.IndexOf(beatLoops[i][j + 1]);
                    beatWeights[teamA][teamB] = beatWeights[teamA][teamB] + 1.0;
                    if (matrix[teamA].ScoreList[teams[teamB]] / beatWeights[teamA][teamB] < minWeight)
                        minWeight = matrix[teamA].ScoreList[teams[teamB]] / beatWeights[teamA][teamB];
                }
            }

            return minWeight;
        }

        /// <summary>
        /// Checks to see if a path exists from any team to itself.
        /// </summary>
        private static bool isLoop()
        {
            for (int i = 0; i < teams.Count; i++)
            {
                if (IsPath(teams[i], teams[i]))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Checks to see if the path in question has any links with a remaining weight of 0.
        /// </summary>
        private static bool LoopContainsZero(List<int> path)
        {
            for (int i = 0; i < path.Count() - 1; i++)
            {
                int teamA = path[i];
                int teamB = path[i + 1];
                if (matrix[teams.IndexOf(teamA)].ScoreList[teamB] == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks to see if the passed in loops are equal. Loops are considered equal if they are the same length and contain
        /// the same teams. By definition, the loops will be in the same order as any other arrangement would lead to smaller loops
        /// which would have been broken by now.
        /// </summary>
        private static bool EqualLoop(List<int> loopA, List<int> loopB)
        {
            // If the paths don't have the same amount of teams, we know the loops aren't equal, so we can return early.
            if (loopA.Count != loopB.Count)
                return false;

            // All BeatLoops have the same team in the first and last positions.  If they start in different locations, 
            // the duplicates have to be removed in order to properly compare the trails.
            loopA.RemoveAt(0);
            loopB.RemoveAt(0);

            while (loopA.Count > 0)
            {
                // If Path A contains the team that's at the beginning of Path B..
                if (loopA.Contains(loopB[0]))
                {
                    // ..remove that team from both paths (reducing Path A's count for the while condition)
                    loopA.Remove(loopB[0]);
                    loopB.RemoveAt(0);
                }
                else
                {
                    // If Path B has a team not in Path A, the loops are not equal and we can quit
                    return false;
                }
            }

            return true;  // If we get here, both loops are now empty, indicating the original loops were equal.
        }

        /// <summary>
        /// Checks the beatList to see if it contains a beatWin of TeamA over TeamB
        /// </summary>
        private static bool InBeatList(int teamA, int  teamB)
        {
            foreach (Game beatWin in beatList)
            {
                if (beatWin.winner == teamA && beatWin.loser == teamB)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if there exists a path leading from the OriginTeam to the EndTeam
        /// </summary>
        public static bool IsPath(int OriginTeam, int EndTeam)
        {
            currentPath.Clear();
            failedLoopTeams.Clear();
            return IsPathHelper(OriginTeam, OriginTeam, EndTeam, teams.Count); // Tries to find a path from the origin team to the end team with a maximum path size of all teams in the league.
        }

        /// <summary>
        /// Recursive test to find a path from the OriginTeam to the End team.
        /// </summary>
        /// <param name="OriginTeam">The team at the start of the path we're looking for.</param>
        /// <param name="CurrentTeam">The recursive step we're on along the path.</param>
        /// <param name="EndTeam">The team at the end of the path we're looking for.</param>
        /// <param name="maxLength">The longest allowed path length, used to restrict to the loop sizes we want.</param>
        /// <returns></returns>
        private static bool IsPathHelper(int OriginTeam, int CurrentTeam, int EndTeam, int maxLength)
        {
            int iCurrentIndex = teams.IndexOf(CurrentTeam);

            // If the current team has a win over the end team, a path exists. Bubble up a true result.
            if (matrix[iCurrentIndex].ScoreList[EndTeam] > 0)
            {
                return true;
            }
            else
            {
                // The current team doesn't have a win over the end team, add it to the current path we're checking
                currentPath.Add(CurrentTeam);

                // TODO: Can this be cleaned into fewer blocks?
                // Then iterate through teams that the current team has defeated.
                for (int i = 0; i < matrix[iCurrentIndex].ScoreList.Count; i++)
                {
                    // As long as we're not exceeding the maximum length and the defeated team hasn't already been checked...
                    if ((currentPath.Count() < maxLength) && !failedLoopTeams.Contains(teams[i]))
                    {
                        // ...and the current path doesn't already contain the defeated team...
                        if (!currentPath.Contains(teams[i]) && (matrix[iCurrentIndex].ScoreList[teams[i]] > 0))
                        {
                            // ...make the defeated team the current team for the next recursion level.
                            if (IsPathHelper(OriginTeam, teams[i], EndTeam, maxLength))
                                return true;
                        }
                    }
                }

                // TODO: Can these be moved to be with the return false below?
                // If we get here, no valid path from the current team to the end team exists, mark it and remove from the current path.
                failedLoopTeams.Add(CurrentTeam);
                currentPath.Remove(CurrentTeam);
            }
            return false;
        }

        /// <summary>
        /// Similar to IsPathHelper except instead of simply returning true/false based on the results, it builds a set of the
        /// paths that exists. The output is stored in allPaths and longestPaths.
        /// </summary>
        private static void FindPathHelper(int CurrentTeam, int EndTeam, int MaxPathSize)
        {
            int currentIndex = teams.IndexOf(CurrentTeam);

            // Add the current team to the path we're making
            currentPath.Add(CurrentTeam);

            // If the current team has a win over the end team, a path exists to the end team.
            if (matrix[currentIndex].ScoreList[EndTeam] > 0)
            {
                // Add the end team to the path and log
                currentPath.Add(EndTeam);
                allPaths.Add(new List<int>(currentPath));

                // If the path is longer than the current set of longest paths...
                if (longestPath.Count > 0 && currentPath.Count > longestPath[0].Count)
                    longestPath.Clear(); // ...clear the existing list.
                if (longestPath.Count == 0 || currentPath.Count >= longestPath[0].Count)
                    longestPath.Add(new List<int>(currentPath)); // Add the current path to the longest path set.
                currentPath.RemoveAt(currentPath.LastIndexOf(EndTeam)); // Pop the path stack.
            }

            // As long as we haven't exceeded the maximum path size (usually the number of teams at this point)...
            if (currentPath.Count < MaxPathSize)
            {
                for (int i = 0; i < matrix[currentIndex].ScoreList.Count; i++)
                {
                    // ...look for more paths leading from teams that the current team has defeated.
                    if (matrix[currentIndex].ScoreList[teams[i]] > 0 && !currentPath.Contains(teams[i]))
                    {
                        FindPathHelper(teams[i], EndTeam, MaxPathSize);
                    }
                }
            }
            currentPath.RemoveAt(currentPath.LastIndexOf(CurrentTeam)); // Pop the team from the path
        }

        /// <summary>
        /// Using the results of pathfinding, gives scores to each team based on remaining paths.
        /// </summary>
        public static void CalculateScores()
        {
            // TODO: Can I ditch the scoreString and just use a double for the final score (would have to rename current score to rawScore)?
            string scoreString;
            double maxScore;
            double minScore;
            double scoreRange;
            double score;

            // Clear and initialize the output result trackers.
            winPoints.Clear();
            lossPoints.Clear();
            countedPoints.Clear();

            for (int i = 0; i < teams.Count; i++)
            {
                winPoints.Add(0.0);
                lossPoints.Add(0.0);
                countedPoints.Add(false);
            }

            // Find the winPoints and lossPoints for each team
            CountPaths();

            // TODO: Does winPoints.Max(w => w); work instead?
            maxScore = winPoints[0]; 
            minScore = lossPoints[0];
            for (int i = 0; i < winPoints.Count; i++)
            {
                if (winPoints[i] > maxScore)
                    maxScore = winPoints[i];
                if (lossPoints[i] > minScore)
                    minScore = lossPoints[i];
            }

            // The scale for scores will be based on the difference between the max score and min score. Since minScore is 
            // a positive number now but is considered negative, it is added to know the total size of the range.
            scoreRange = maxScore + minScore;
            for (int i = 0; i < matrix.Count; i++)
            {
                scoreString = "";
                // Each team's raw score is the difference between their win points and loss points.
                score = winPoints[i] - lossPoints[i];

                // Calculate the final score using the SUPARSEKRIT method here which makes no real sense.
                // The gist of the calculation is that higher scores are exponentially higher, so SQRT them
                // to put them on a more reasonable scale.
                // TODO: See if a LOG function would do a better job smoothing out the scores
                // TODO: If I ditch the string, can I math it out so I don't need to check if score is negative?
                if (score < 0)
                {
                    scoreString += "-";
                    score = (Math.Sqrt(((-score) * 100) / scoreRange) * 100) / 100;
                }
                else
                {
                    score = (Math.Sqrt(((score) * 100) / scoreRange) * 100) / 100;
                }
                scoreString += score;
                matrix[i].score = scoreString;
            }
        }

        /// <summary>
        /// CountPaths is used to do the set up for calculating the scores. Each team will have
        /// its total number of paths leading in and out totaled. This is done by finding teams
        /// with no wins and building up, or no losses and building down.
        /// </summary>
        private static void CountPaths()
        {
            // Count all paths leading out from each team
            for (int i = 0; i < teams.Count; i++)
            {
                if (!countedPoints[i])
                {
                    CountPathHelper(teams.IndexOf(teams[i]));
                }
            }

            // Set all teams back to not having had their paths counted
            countedPoints = countedPoints.Select(c => false).ToList();

            // Count all paths leading into each team
            for (int i = 0; i < teams.Count; i++)
            {
                if (!countedPoints[i])
                {
                    CountLossPathHelper(teams.IndexOf(teams[i]));
                }
            }
        }

        /// <summary>
        /// Recursive part of the CountPaths.
        /// </summary>
        private static void CountPathHelper(int iTeamIndex)
        {
            for (int i = 0; i < matrix.Count; i++)
            {
                // If this team has BEATEN other teams, drill down.
                if (matrix[iTeamIndex].ScoreList[teams[i]] > 0)
                {
                    if (!countedPoints[i])
                        CountPathHelper(i);
                    winPoints[iTeamIndex] = winPoints[iTeamIndex] + winPoints[i] + matrix[iTeamIndex].ScoreList[teams[i]];
                }
            }
            countedPoints[iTeamIndex] = true;
        }

        /// <summary>
        /// Recursive part of the CountPaths.
        /// </summary>
        private static void CountLossPathHelper(int iTeamIndex)
        {
            for (int i = 0; i < matrix.Count; i++)
            {
                // If this team has BEEN BEATEN BY other teams, drill down.
                if (matrix[i].ScoreList[teams[iTeamIndex]] > 0)
                {
                    if (!countedPoints[i])
                        CountLossPathHelper(i);
                    lossPoints[iTeamIndex] = lossPoints[iTeamIndex] + lossPoints[i] + matrix[i].ScoreList[teams[iTeamIndex]];
                }
            }
            countedPoints[iTeamIndex] = true;
        }

        /// <summary>
        /// Finds the longest existing paths.
        /// </summary>
        private static void FindLongestPaths()
        {
            longestPath.Clear();
            currentPath.Clear();

            // Starting only from teams with no losses as they're the only candidates for longest path.
            for (int i = 0; i < matrix.Count; i++)
            {
                if (lossPoints[i] == 0)
                {
                    // Keep adding teams to potential paths using the recursive path helper.
                    currentPath.Add(teams[i]);
                    for (int j = 0; j < matrix[i].ScoreList.Count; j++)
                    {
                        if (matrix[i].ScoreList[teams[j]] > 0)
                        {
                            FindLongPathHelper(teams[j]);
                        }
                    }
                    currentPath.RemoveAt(currentPath.Count - 1);
                }
            }
        }

        /// <summary>
        /// Recursive part of finding longest paths.
        /// </summary>
        private static void FindLongPathHelper(int CurrentTeam)
        {
            int iCurrentIndex = teams.IndexOf(CurrentTeam);

            // Starting from the current team
            currentPath.Add(CurrentTeam);
            // If there are no paths yet or the current path is equal to the longest, add the current path to the set.
            // TODO: Should be able to combine the first two cases which do the same thing due to short-circuit magic.
            if (longestPath.Count == 0)
            {
                longestPath.Add(new List<int>(currentPath));
            }
            else if (longestPath[0].Count == currentPath.Count)
            {
                longestPath.Add(new List<int>(currentPath));
            }
            else if (longestPath[0].Count < currentPath.Count)
            {
                // If this is a new record for longest, clear the longest set and add the current one for a new set.
                longestPath.Clear();
                longestPath.Add(new List<int>(currentPath));
            }

            // Keep drilling down from the current team to teams the current team has defeated to find longer paths.
            for (int i = 0; i < matrix[iCurrentIndex].ScoreList.Count; i++)
            {
                if (!currentPath.Contains(teams[i]) && (matrix[iCurrentIndex].ScoreList[teams[i]] > 0))
                {
                    FindLongPathHelper(teams[i]);
                }
            }

            currentPath.RemoveAt(currentPath.Count - 1);
        }
    }

    // TODO: Find a better place for this
    public enum Method
    {
        Standard,
        Iterative,
        Weighted
    }
}