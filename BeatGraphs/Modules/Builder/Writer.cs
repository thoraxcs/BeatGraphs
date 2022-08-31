using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeatGraphs.Modules
{
    /// <summary>
    /// The Writer class is responsible for taking the calculated results from the Builder class and creating the physical output.
    /// This includes creating a .txt file that gets input to GraphViz, resulting in a .png with the visual graph. Additionally, it
    /// creates a .php file for the run as well as a Top5 .php file which is imported into the index page. These files are all then
    /// FTP'd to BeatGraphs.com.
    /// </summary>
    public static class Writer
    {
        /// <summary>
        /// Entry point for the Writer class. Creates physical directory structure if necessary and calls components to do the work.
        /// </summary>
        /// <param name="league">Which league will be processed in the run</param>
        /// <param name="season">Which season will be processed in the run</param>
        /// <param name="method">Which method will be processed in the run</param>
        /// <param name="week">Which week will be processed in the run</param>
        public static void ProcessFiles(string league, string season, Method method, string week)
        {
            // Creates necessary directory structure if it doesn't yet exist.
            Helpers.InitializeDirectory(league, method.ToString().Substring(0, 1), season);

            // Execute steps to build required files
            PrintWebContent(league, season, method, week);
            PrintGraphFile(league, season, method);

            // Set up commands for GraphViz and generate the graph
            string imgPath = $"\\{league}\\{method.ToString()[0]}\\{season}\\{week}.png";
            Helpers.GenerateGraph(imgPath);

            // Upload files to BeatGraphs.com
            FtpFiles(league, season, method, week);
        }

        /// <summary>
        /// Creates the web page and Top5 subsection
        /// </summary>
        private static void PrintWebContent(string league, string season, Method method, string week)
        {
            StringBuilder sbOut = new StringBuilder();
            StringBuilder sbTop5 = new StringBuilder();

            //Output Longest Paths
            sbOut.Append("<p><br><b>The longest BeatPaths this week are:</b><br>\n");
            foreach (var path in Builder.longestPath)
            {
                for (int i = 0; i < path.Count; i++)
                {
                    sbOut.Append(Builder.matrix[Builder.teams.IndexOf(path[i])].abbreviation);
                    if (i + 1 < path.Count)
                        sbOut.Append(" &rarr; ");
                    else
                        sbOut.Append("<br>\n");
                }
            }

            // Output Winless and Lossless Teams
            sbOut.Append("<br><b>These teams have no surviving BeatLosses:</b><br>\n");
            sbOut.Append(string.Join(", ", Builder.matrix.Where(team => Builder.matrix.Sum(sl => sl.ScoreList[team.franchiseID]) == 0).Select(team => team.abbreviation).OrderBy(abbr => abbr)));

            sbOut.Append("\n<br>\n<br><b>These teams have no surviving BeatWins:</b><br>\n");
            sbOut.Append(string.Join(", ", Builder.matrix.Where(team => team.ScoreList.Sum(sl => sl.Value) == 0).Select(team => team.abbreviation).OrderBy(abbr => abbr)));

            // Output BeatLoops
            sbOut.Append("\n<br>\n<br><b>These were the BeatLoops resolved:</b><br>\n");
            foreach (var loops in Builder.beatLoopList)
            {
                for (int i = 0; i < loops.Count; i++)
                {
                    if (i > 0)
                        sbOut.Append(" &rarr; ");
                    sbOut.Append(Builder.matrix[Builder.teams.IndexOf(loops[i])].abbreviation);
                }
                sbOut.Append("<br>\n");
            }

            // Static HTML wrapping for the pages.
            sbOut.Append("\n</p></div><br>\n");
            sbOut.Append("<div class='pagearea'>\n");
            sbOut.Append("<div class='rankarea'>\n");
            sbOut.Append("<div class='scoretable'>\n");
            sbOut.Append("<div class='scorerow'>\n");
            sbOut.Append("<div class='scoresampleheader'>Rank</div>\n");
            sbOut.Append("<div class='scoresampleheader'>Team</div>\n");
            sbOut.Append("<div class='scoresampleheader'>Score</div>\n");
            sbOut.Append("<div class='scoresampleheader'>Out</div>\n");
            sbOut.Append("<div class='scoresampleheader'>In</div>\n");
            sbOut.Append("</div>\n\n");

            sbTop5.Append("<div class='top5'>\n");
            sbTop5.Append($@"<a href=""graphs.php?league={league}&method={method.ToString()[0]}""><div class='top5header'>{method}<div class='toptop'> Top 5</div></div></a>");
            sbTop5.Append("\n<div class='top5row'><div class='top5subhead'>#</div><div class='top5subhead'>Team</div><div class='top5subhead'>Score</div></div>");

            // Write the score table for the left side of the page
            int rank = 0, stored = 0;
            double lastScore = 0;

            Builder.matrix.OrderByDescending(team => team.score).ToList().ForEach(team => {
                sbOut.Append("\n<div class='scorerow'>");
                if (team.score == lastScore)
                {
                    // Keep the rank the same for teams with the same score. Increment stored to track how many have the same score.
                    stored++;
                }
                else
                {
                    // Increase rank by one for each team, plus any amount of stored ranks from above ties. Reset store count.
                    rank += stored + 1;
                    stored = 0;
                }
                // Output column data for the team at this rank
                sbOut.Append($"<div class='scoresamplecell'>{rank}</div>");
                sbOut.Append($"<div class='scoresamplecell'><img src='{Helpers.GetImage(team.franchiseID, league, season)}'></div>");
                sbOut.Append($"<div class='scoresamplecell'>{string.Format("{0:N2}", Math.Round(team.score, 2))}</div>");
                sbOut.Append($"<div class='scoresamplecell'>{nFormat(Math.Round(Builder.winPoints[team.index], 2))}</div>");
                sbOut.Append($"<div class='scoresamplecell'>{nFormat(Math.Round(Builder.lossPoints[team.index], 2))}</div></div>");

                // Make a copy of this row if it's one of the top 5 teams.
                if (rank + stored <= 5)
                {
                    sbTop5.Append($"\n<div class='top5row'><div class='top5cell'>{rank}</div>");
                    sbTop5.Append($"<div class='top5cell top5midcell'><img src='{Helpers.GetImage(team.franchiseID, league, season)}' /></div>");
                    sbTop5.Append($"<div class='top5cell'>{string.Format("{0:N2}", Math.Round(team.score, 2))}</div></div>");
                }

                lastScore = team.score; // Track this team's score to see if the next team has the same score
            });

            // Close out the files and write them to disk.
            sbOut.Append($"</div></div>\n");
            sbOut.Append($"<div class='grapharea'>\n");
            sbOut.Append($"<img src='{league}/{method.ToString()[0]}/{season}/{week}.png' />\n");
            sbOut.Append($"</div></div></div>\n");
            sbOut = sbOut.Replace("\n", Environment.NewLine);

            sbTop5.Append("\n</div>");
            sbTop5 = sbTop5.Replace("\n", Environment.NewLine);

            // Write the output files, Top5 will only get uploaded from the runner to ensure only most current data is uploaded.
            Helpers.WriteFile(BasePath.file, $"/{league}/{method.ToString()[0]}/{season}/{week}.php", sbOut.ToString());
            Helpers.WriteFile(BasePath.file, $"/{league}_{method.ToString()[0]}.php", sbTop5.ToString());
        }

        /// <summary>
        /// Number formatter for double output. The intent is:
        /// 0 decimal places for numbers greater than 10 (or 0 exactly)
        /// 1 decimal place for numbers 1 > x > 10
        /// 2 decimal places for numbers less than 1
        /// This is irrespective of negatives, so use absolute values to determine.
        /// </summary>
        private static string nFormat(double dNum)
        {
            if (dNum == 0)
                return string.Format("{0:N0}", dNum);
            else if (Math.Abs(dNum) < 1)
                return string.Format("{0:N2}", dNum);
            else if (Math.Abs(dNum) < 10)
                return string.Format("{0:N1}", dNum);
            else
                return string.Format("{0:N0}", dNum);
        }

        /// <summary>
        /// This function writes the .txt file used by GraphViz to produce the .png graph file
        /// </summary>
        private static void PrintGraphFile(string league, string season, Method method)
        {
            //TextWriter twOut = new StreamWriter($"{filePath}GraphOut.txt", false);
            var sbOut = new StringBuilder();
            var scores = new List<double>();
            var tiers = new List<List<int>>();
            var teamsLeft = new List<int>();
            string confColor; //Default black outline
            string divColor; //Default white fill
            double minScore;
            bool bNewTier;

            // Static starting code for the file
            sbOut.Append("digraph beatgraphs {");
            sbOut.Append("\n\tnode [label=\"\\N\", shape=\"box\", style=\"filled, rounded\", fontsize=\"10\"];");
            sbOut.Append("\n\tgraph [nodesep=\"0.1\", ranksep=\"0.3\", size=\"8,12\"];");
            sbOut.Append("\n\tedge [arrowsize=\"0.5\"];\n");

            // Add each team's properties to the file
            foreach (Team tTeam in Builder.matrix)
            {
                scores.Add(tTeam.score);
                teamsLeft.Add(tTeam.franchiseID);
                #region Assign Colors
                switch (tTeam.conference)
                {
                    case "EAST":
                    case "AFC":
                    case "AL":
                    case "WALE":
                        confColor = "#FF0000"; //Red outline
                        break;
                    case "WEST":
                    case "NFC":
                    case "NL":
                    case "CAMP":
                        confColor = "#0000FF"; //Blue outline
                        break;
                    default:
                        confColor = "#000000"; //Black outline
                        break;
                }

                switch (tTeam.division)
                {
                    case "AFCE":
                    case "ALE":
                    case "ATL":
                    case "EAST":
                        divColor = "#FBB4AE"; //Red fill
                        break;
                    case "AFCS":
                        divColor = "#FDDAEC"; //Pink fill
                        break;
                    case "AFCN":
                    case "AFCC":
                    case "ALC":
                    case "SE":
                        divColor = "#FED9A6"; //Orange fill
                        break;
                    case "AFCW":
                    case "ALW":
                    case "ADA":
                    case "MET":
                    case "NE":
                        divColor = "#FFFFCC"; //Yellow fill
                        break;
                    case "NFCE":
                    case "NLE":
                    case "MID":
                        divColor = "#B3E2CD"; //Green fill
                        break;
                    case "NFCS":
                        divColor = "#E0ECF4"; //Cyan fill
                        break;
                    case "NFCW":
                    case "NLW":
                    case "PAC":
                    case "WEST":
                    case "SMY":
                        divColor = "#CAB2D6"; //Blue fill
                        break;
                    case "NFCN":
                    case "NFCC":
                    case "NLC":
                    case "SW":
                    case "NW":
                        divColor = "#9EBCDA"; //Purple fill
                        break;
                    case "CEN":
                        if (tTeam.conference == "EAST")
                            divColor = "#FFFFCC"; //Yellow fill
                        else
                            divColor = "#B3E2CD"; //Green fill
                        break;
                    case "NOR":
                    case "PAT":
                        if (tTeam.conference == "WALE")
                            divColor = "#FBB4AE"; //Red fill
                        else
                            divColor = "#B3E2CD"; //Green fill
                        break;
                    default:
                        divColor = "#FFFFFF"; //White fill
                        break;
                }
                #endregion

                sbOut.Append($"\n\t\"{tTeam.abbreviation.Replace(" ", "")}\" [fillcolor=\"{divColor}\"][color=\"{confColor}\"][label=<<TABLE border='0' cellpadding='0' cellspacing='0'><TR><TD><IMG SRC='{Helpers.GetImage(tTeam.franchiseID, league, season, true)}'/></TD></TR><TR><TD>{tTeam.abbreviation}</TD></TR></TABLE>>];");
            }
            sbOut.Append("\n");

            #region Determine Graph Tiers
            // Sort all the scores from lowest to highest and create the first tier of teams
            scores.Sort();
            tiers.Add(new List<int>());

            // While there are teams that haven't been added to the graph yet...
            while (teamsLeft.Count > 0)
            {
                // Create a set of teams that will be added this tier and start with the lowest remanining score
                var teamsToAdd = new List<int>();
                minScore = scores[0];

                // Add every team with the minimum score to the active tier and remove them from the remaining team set
                Builder.matrix.FindAll(delegate (Team t) { return t.score == minScore; }).ForEach(team => 
                    { 
                        teamsToAdd.Add(team.franchiseID);
                        teamsLeft.Remove(team.franchiseID);
                    });

                // Remove all instances of the minimum score
                scores.RemoveAll(delegate (double score) { return score == minScore; });

                // If the current team has a path to another team on the current tier, a new tier will need to be created
                bNewTier = false;
                foreach (int fromTeam in teamsToAdd)
                {
                    foreach (int toTeam in tiers[0])
                    {
                        if (Builder.IsPath(fromTeam, toTeam))
                        {
                            bNewTier = true;
                        }
                    }
                }

                // If a new tier needs to be created, insert a new empty tier at the beginning of the tier set.
                if (bNewTier)
                    tiers.Insert(0, new List<int>());

                // Add the current team to the top tier
                foreach (int iTeam in teamsToAdd)
                {
                    tiers[0].Add(iTeam);
                }
            }

            // Now that we have the tiers and the teams that belong in them, write out this information to the file
            foreach (var tier in tiers)
            {
                sbOut.Append("\n\t{rank=same;");

                foreach (int team in tier)
                {
                    sbOut.Append($" \"{Builder.matrix[Builder.teams.IndexOf(team)].abbreviation.Replace(" ", "")}\"");
                }

                sbOut.Append("}");
            }
            #endregion

            // Write each individual BeatWin to the output with a proper style dependant on the method and scores.
            sbOut.Append("\n");
            foreach (Team team in Builder.matrix)
            {
                for (int i = 0; i < team.ScoreList.Count; i++)
                {
                    if (team.ScoreList[Builder.teams[i]] > 0)
                    {
                        // For each win the team has... temporarily store away the score for the win and pretend it doesn't exist
                        var weightScore = team.ScoreList[Builder.teams[i]]; // ...temporarily store away the score for the win...
                        team.ScoreList[Builder.teams[i]] = 0; // ...and pretend it doesn't exist by setting the original score to 0.

                        //If there is no indirect BeatPath, draw this arrow. The direct win is ignored due to the trickery above.
                        if (!Builder.IsPath(team.franchiseID, Builder.teams[i])) 
                        {
                            sbOut.Append($"\n\"{team.abbreviation.Replace(" ", "")}\"->\"{Builder.matrix[i].abbreviation.Replace(" ", "")}\"");
                            #region Apply Arrow Styling
                            if (method == Method.Iterative)
                            {
                                if (weightScore >= 1.5)
                                {
                                    sbOut.Append(" [style=bold]");
                                }
                                else if (weightScore < 0.5)
                                {
                                    sbOut.Append(" [style=dotted]");
                                }
                                else if (weightScore < 1)
                                {
                                    sbOut.Append(" [style=dashed]");
                                }
                            }
                            else if (method == Method.Weighted)
                            {
                                if (league == "MLB")
                                {
                                    if (weightScore < 5)
                                    {
                                        sbOut.Append(" [color=red]");
                                    }
                                    else if (weightScore >= 15)
                                    {
                                        sbOut.Append(" [color=blue]");
                                    }
                                }
                                else if (league == "NBA")
                                {
                                    if (weightScore < 6)
                                    {
                                        sbOut.Append(" [color=red]");
                                    }
                                    else if (weightScore >= 20)
                                    {
                                        sbOut.Append(" [color=blue]");
                                    }
                                }
                                else if (league == "NFL")
                                {
                                    if (weightScore < 7)
                                    {
                                        sbOut.Append(" [color=red]");
                                    }
                                    else if (weightScore >= 21)
                                    {
                                        sbOut.Append(" [color=blue]");
                                    }
                                }
                                else if (league == "NHL")
                                {
                                    if (weightScore < 2)
                                    {
                                        sbOut.Append(" [color=red]");
                                    }
                                    else if (weightScore >= 4)
                                    {
                                        sbOut.Append(" [color=blue]");
                                    }
                                }
                            }
                            #endregion
                            sbOut.Append(";");
                        }

                        // Restore the score to the origin
                        team.ScoreList[Builder.teams[i]] = weightScore;
                    }
                }
            }

            sbOut.Append("\n}");

            // Output the text file to disk
            Helpers.WriteFile(BasePath.file, "GraphOut.txt", sbOut.ToString());
            //twOut.Write(sbOut.ToString());
            //twOut.Close();
        }

        /// <summary>
        /// Upload the created files to the web site.
        /// </summary>
        private static void FtpFiles(string league, string season, Method method, string week)
        {
            // Build directory structure on the web site if necessary.
            try
            {
                Logger.Log("Creating League Directory", LogLevel.verbose);
                Helpers.FtpCreateDirectory(league);
                Logger.Log("Creating Method Directory", LogLevel.verbose);
                Helpers.FtpCreateDirectory($"{league}/{method.ToString()[0]}");
                Logger.Log("Creating Season Directory", LogLevel.verbose);
                Helpers.FtpCreateDirectory($"{league}/{method.ToString()[0]}/{season}");
            }
            catch (Exception ex)
            {
                Logger.Log($"Error building ftp directory tree: {ex.Message}", LogLevel.error);
                return;
            }

            //Send the files (Top5 is only sent by Runner to ensure only most recent data gets sent)
            Helpers.FtpUploadFile($"{league}/{method.ToString()[0]}/{season}/{week}.php");
            Helpers.FtpUploadFile($"{league}/{method.ToString()[0]}/{season}/{week}.png");
        }

    }
}
