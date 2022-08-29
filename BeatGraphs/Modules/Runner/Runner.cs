using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BeatGraphs.Modules
{
    /// <summary>
    /// The Runner class is responsible for the automated functions of the application
    /// </summary>
    public static class Runner
    {
        /// <summary>
        /// Entry point for the scheduled process runs
        /// </summary>
        public static void Run(List<string> inLeagues, List<Method> methods)
        {
            Logger.Log($"Starting schedule run at {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}.", LogLevel.warning);
            // Process each league in turn.
            foreach (var league in inLeagues)
            {
                var info = Helpers.GetCurrentWeeks();
                var seasons = new List<string>();
                var season = info[league].Item1;

                // Only send the most current season for the league
                seasons.Add(season.ToString());
                var leagues = inLeagues.Where(l => l == league).ToList(); // Updater takes a list of leagues, so make the desired league into a list

                // Call the Updater and Builder to process the most current data
                Updater.Run(leagues, season.ToString());
                Builder.Run(leagues, seasons, methods, -1); // -1 tells the Builder to run the most current week only

                // The builder uploads the normal .php and .png files, but the Top 5 has to be done here to avoid manual runs of non-recent times
                foreach (var method in methods)
                    Helpers.FtpUploadFile($"{league}_{method.ToString()[0]}.php");
            }

            // Build files included by other pages for structure
            BuildFooter();
            BuildLeagueData();

            Logger.Log($"Scheduled run complete: {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}", LogLevel.special);
        }

        /// <summary>
        /// On the Runner tab there are 4 buttons which are able to insert new seasons into the DB. This function initializes their states.
        /// </summary>
        public static void LoadNewButton(Button button, string league, Tuple<int, int, int> info)
        {
            var season = info.Item1;
            var week = info.Item2;
            var wins = info.Item3;

            // If it's the championship
            if (week == 504)
            {
                // And all of the games in the championship have been played
                if (wins == 4 || (league == "NFL" && wins == 1))
                {
                    // The league is eligible for creating a new season.
                    button.Enabled = true;
                    button.Text = $"Insert {season + 1} {league} Season";
                    return;
                }
            }

            // If we get here, the most recent league is the active league.
            button.Enabled = false;
            button.Text = week == 0 ? $"{league} Season Not Started" : $"{league} Season In Week {week}";
        }

        /// <summary>
        /// Updates the last run time on the footer
        /// </summary>
        private static void BuildFooter()
        {
            var regex = new Regex(@"[aA-zZ]{3} \d{2}, \d{4} at \d+:\d{2} \wM");
            var footer = Helpers.ReadFile(BasePath.file, "/footer.php");
            footer = regex.Replace(footer, DateTime.Now.ToString("MMM d, yyyy 'at' h:mm tt"));//  footer.Replace("[[UPLOADDATE]]", DateTime.Now.ToString("MMM d, yyyy 'at' h:mm tt"));
            Helpers.WriteFile(BasePath.file, "/footer.php", footer);

            Logger.Log("Uploading Footer");
            Helpers.FtpUploadFile("/footer.php");
        }

        /// <summary>
        /// Builds the filelist file which is responsible for telling the web pages what game data is available
        /// </summary>
        private static void BuildLeagueData()
        {
            var leagues = Helpers.GetAllSeasons();
            var sbFileList = new StringBuilder();

            sbFileList.Append("<?\n\n");
            sbFileList.Append("$filearray = array(");

            // Build the arrays for the leagues
            var isFirst = true;
            foreach (var league in leagues)
            {
                sbFileList.Append($@"{(!isFirst ? ",\n" : "")}""{league.Key}"" => array()");
                isFirst = false;
            }
            sbFileList.Append(");\n\n");

            // Build the arrays for the seasons
            foreach (var league in leagues)
            {
                sbFileList.Append($@"$filearray[""{league.Key}""] = array(");
                isFirst = true;
                foreach (var year in leagues[league.Key])
                {
                    sbFileList.Append($@"{(!isFirst ? ",\n" : "")}""{year.Key}"" => array()");
                    isFirst = false;
                }
                sbFileList.Append(");\n\n");
            }

            // Build the arrays for the weeks
            foreach (var league in leagues)
            {
                foreach (var year in leagues[league.Key])
                {
                    sbFileList.Append($@"$filearray[""{league.Key}""][""{year.Key}""] = array({string.Join(", ", year.Value)});{Environment.NewLine}");
                }
            }
            sbFileList.Append("\n?>");

            // Write out the file and upload it
            Helpers.WriteFile(BasePath.file, "/filelist.php", sbFileList.ToString());
            Logger.Log("Uploading File List");
            Helpers.FtpUploadFile("/filelist.php");
        }

        /// <summary>
        /// Primary function for building each of the playoff history web pages
        /// </summary>
        public static void BuildPlayoffs(string league)
        {
            // Get all of the years for the league
            Logger.Log($"Building playoffs for {league}.");
            var seasons = Helpers.GetYearsByLeague(league);
            var sbOut = new StringBuilder();

            // Build the header for the playoff history page
            sbOut.Append("<? // Set variables\n\n");
            sbOut.Append("$mymethodinit = \"X\";\n\n");
            sbOut.Append("?><? include \"header.php\"; ?>\n\n");
            sbOut.Append("\t\t\t\t<!-- Main -->\n");
            sbOut.Append("\t\t\t\t<div id=\"main-wrapper\">\n");
            sbOut.Append("\t\t\t\t\t<div id=\"main\" class=\"container\">\n");
            sbOut.Append("\t\t\t\t\t\t<div id=\"content\">\n\n");
            sbOut.Append("\t\t\t\t\t\t\t<!-- Post -->\n");
            sbOut.Append("\t\t\t\t\t\t\t\t<article class=\"box post\">\n");
            sbOut.Append("\t\t\t\t\t\t\t\t\t<header>\n");
            sbOut.Append($"\t\t\t\t\t\t\t\t\t\t<h2><strong>{league} Playoff History</strong></h2>\n");
            sbOut.Append("\t\t\t\t\t\t\t\t\t</header>\n");

            // Build each season's playoff bracket
            foreach (var season in seasons)
            {
                // Retrieve all of the series involved in the season's playoffs
                var results = Helpers.GetPlayoffResults(league, season);
                if (results.Count > 0)
                {
                    // Build the playoff bracket based on the series, and convert the structure into the output bracket
                    BuildPlayoffTree(results, results[0], league, season);
                    sbOut.Append(WritePlayoffTree(results[0], league, season));
                }
                #region Addenda
                if (league == "MLB" && season == 1995) // Strike shortened baseball season
                {
                    sbOut.Append("\t\t\t\t\t\t\t\t\t<h4>1994</h4>\n");
                    sbOut.Append("\t\t\t\t\t\t\t\t\t<h4><h3 style=\"text-align:center; width: 100%\">1994 playoffs canceled</h3></h4>\n");
                }
                if (league == "NHL" && season == 2005) // Strike canceled hockey season
                {
                    sbOut.Append("\t\t\t\t\t\t\t\t\t<h4>2004-05</h4>\n");
                    sbOut.Append("\t\t\t\t\t\t\t\t\t<h4><h3 style=\"text-align:center; width: 100%\">2004-05 season canceled</h3></h4>\n");
                }
                if (league == "NFL" && season == 1970) // Super Bowls prior to BeatGraph tracking
                {
                    // TODO: Add in SuperBowl 1-4 (1966-1969 seasons)
                    sbOut.Append("\t\t\t\t\t\t\t\t\t<h4>1966-70: SuperBowls I - IV</h4>\n");
                    sbOut.Append("\t\t\t\t\t\t\t\t\t<h4><h3 style=\"text-align:center; width: 100%\">BeatGraphs only goes back to 1970.</h3></h4>\n");
                }
                #endregion
            }

            // Build the page's footer
            sbOut.Append("\t\t\t\t\t\t\t\t\t</article>\n");
            sbOut.Append("\t\t\t\t\t\t\t</div>\n");
            sbOut.Append("\t\t\t\t\t\t</div>\n");
            sbOut.Append("\t\t\t\t\t</div>\n");
            sbOut.Append("<? include \"footer.php\"; ?>");

            // Write out the file and upload it
            Helpers.WriteFile(BasePath.file, $"{league}Playoffs.php", sbOut.ToString());
            Helpers.FtpUploadFile($"{league}Playoffs.php");
            Logger.Log($"Playoffs for {league} complete.");
        }

        // Recursively takes the list of series and turns it into a tree structure by linking branches together
        private static void BuildPlayoffTree(List<Series> results, Series step, string league, int season)
        {
            // Get the logo for this node
            step.iconURL = Helpers.GetImage(step.winnerID, league, season.ToString());

            // Look for the home team in the match prior to this round
            step.homeTeam = results.Where(r => r.winnerID == step.homeID && step.round == r.round + 1).FirstOrDefault();
            if (step.homeTeam != null)
            {
                // If there's a next team, remove it from the remaining list since it's been added to the tree, then continue building down
                results.Remove(step.homeTeam);
                BuildPlayoffTree(results, step.homeTeam, league, season);
            }
            else
            {
                // If there is no next team, add the team as a leaf node.
                step.homeTeam = new Series(step.round - 1, step.homeID, 0, 0);
                step.homeTeam.iconURL = Helpers.GetImage(step.homeID, league, season.ToString());
            }

            // TODO: Combine the home and away section into one function
            step.awayTeam = results.Where(r => r.winnerID == step.awayID && step.round == r.round + 1).FirstOrDefault();
            if (step.awayTeam != null)
            {
                results.Remove(step.awayTeam);
                BuildPlayoffTree(results, step.awayTeam, league, season);
            }
            else
            {
                step.awayTeam = new Series(step.round - 1, step.awayID, 0, 0);
                step.awayTeam.iconURL = Helpers.GetImage(step.awayID, league, season.ToString());
            }
        }

        /// <summary>
        /// Write the complete bracket for a season
        /// </summary>
        private static string WritePlayoffTree(Series result, string league, int season)
        {
            var sbOut = new StringBuilder();
            int bracketSpacer = 16; // 16 is the closest set. Always start here.

            // Print the season header.
            sbOut.Append($"\t\t\t\t\t\t\t\t\t<h4>{season}{(league != "MLB" ? $"-{(season + 1).ToString().Substring(2)}{(league == "NFL" ? ": " + GetSuperBowl(season) : "")}" : "")}</h4>\n");

            // Find the first round of the playoffs and get all teams involved in it. Space them out with nulls
            var (round, teams) = GetLowestRound(result);
            PrintRound(sbOut, teams, bracketSpacer);

            // Continue printing rounds until we get to the championship (round 504)
            while (round++ < 504)
            {
                // Covid year of NHL had an extra round with 24 playoff teams. This handles the case
                if (teams.Count != 32) 
                    bracketSpacer /= 2;

                teams = GetNextRound(round, result, new List<string>());
                PrintRound(sbOut, teams, bracketSpacer);
            }

            return sbOut.ToString();
        }

        /// <summary>
        /// Like GetLowestRound, gets all of the teams involved in a round of the playoffs. This time we now the round number and can retrieve directly.
        /// </summary>
        private static List<string> GetNextRound(int round, Series series, List<string> images)
        {
            // If we're in the round, add the team and bubble up
            if (round == series.round)
            {
                images.Add(series.iconURL);
                return images;
            }

            // Recursively traverse the tree for all series in the round.
            if (series.awayTeam != null)
                images = GetNextRound(round, series.awayTeam, images);
            if (series.homeTeam != null)
                images = GetNextRound(round, series.homeTeam, images);

            return images;
        }

        /// <summary>
        /// Print out a single round of a playoff bracket. This include the set of teams as well as the bracket markers below it.
        /// </summary>
        private static void PrintRound(StringBuilder sbOut, List<string> teams, int bracketSpacer)
        {
            // Print the teams involved in the playoff round (special styling for NHL Covid playoffs)
            sbOut.Append($"\t\t\t\t\t\t\t\t\t<div class=\"round{bracketSpacer}\"{(teams.Count == 32 ? @" style=""position:relative; right:22px; !important""" : "")}>\n");
            foreach (var team in teams)
            {
                if (team != null)
                    sbOut.Append($"\t\t\t\t\t\t\t\t\t\t<div><img src=\"{team}\" /></div>\n");
                else if (teams.Count != 32)
                    sbOut.Append($"\t\t\t\t\t\t\t\t\t\t<div></div>\n");
            }
            sbOut.Append("\t\t\t\t\t\t\t\t\t</div>\n");

            // If this isn't just printing the league champion, print the brackets leading to the next round (special styling for NHL Covid playoffs)
            if (teams.Count != 1)
            {
                sbOut.Append($"\t\t\t\t\t\t\t\t\t<div class=\"bround{bracketSpacer / 2}\"{(teams.Count == 32 ? @" style=""position:relative; right:22px; !important""" : "")}>\n");
                for (int i = 0; i < teams.Count; i += 2)
                {
                    if (teams[i] != null)
                        sbOut.Append($"\t\t\t\t\t\t\t\t\t\t<div><img src=\"images/Bracket{bracketSpacer}.png\" /></div>\n");
                    else if (teams.Count != 32)
                        sbOut.Append($"\t\t\t\t\t\t\t\t\t\t<div>&nbsp;</div>\n");
                }
                sbOut.Append("\t\t\t\t\t\t\t\t\t</div>\n");
            }
        }

        /// <summary>
        /// Find the first round of the playoffs
        /// </summary>
        private static (int, List<string>) GetLowestRound(Series series)
        {
            int lowest;
            var teams = new List<string>();

            lowest = TraverseTree(series, 505, teams);

            return (lowest, teams);
        }

        /// <summary>
        /// Recursive helper for finding the first round of the playoffs and adding all series in that round to a list
        /// </summary>
        private static int TraverseTree(Series series, int lowest, List<string> teams)
        {
            // If this round is before the lowest one seen so far...
            if (series.round < lowest)
            {
                // Update the lowest seen, clear teams previously added in higher rounds, and add blank teams if needed to buffer for byes
                lowest = series.round;
                teams.Clear();
                var preblank = teams.Count * 2;
                for (int i = 0; i < preblank; i++)
                {
                    teams.Add(null);
                }
            }

            // If this matches there are no children to this node and it's in the first round seen, add the team to the list.
            if (series.awayID == 0 && series.homeID == 0 && series.round == lowest)
            {
                teams.Add(series.iconURL);
                return lowest;
            }

            // Getting this far means we're not on a leaf, check branches from this node.
            int awayround = lowest;
            int homeround = lowest;

            // Traverse away before home to ensure away ends up to the left
            if (series.awayTeam != null)
                awayround = TraverseTree(series.awayTeam, lowest, teams);
            else
                teams.Add(null);

            if (series.homeTeam != null)
                homeround = TraverseTree(series.homeTeam, awayround, teams); // Pass awayround instead of lowest because it might be lower
            else
                teams.Add(null);
            
            return awayround < homeround ? awayround : homeround;
        }

        /// <summary>
        /// Get the number of the SuperBowl and requests a transformation to Roman Numerals if needed
        /// </summary>
        private static string GetSuperBowl(int year)
        {
            var number = year - 1965;
            if (number == 1)
                return "NFL/AFL Championship Game";
            else if (number == 50)
                return "50";
            else
                return ToRoman(number);
        }

        /// <summary>
        /// Converts an integer to a Roman Numeral
        /// </summary>
        private static string ToRoman(int number)
        {
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("Not a valid Roman Numeral number");
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900);
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            throw new ArgumentOutOfRangeException("Not a valid Roman Numeral number");
        }
    }
}
