using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
        public static BeatGraphForm form;   // Used for logging to the form's progress textbox
        public static bool ftpEnabled = false; // User setting for whether or not to immediately FTP results to web

        static readonly string filePath = ConfigurationManager.AppSettings.Get("filePath"); // The base file path on the computer/server

        /// <summary>
        /// Entry point for the Writer class. Creates physical directory structure if necessary and calls components to do the work.
        /// </summary>
        /// <param name="league">Which league will be processed in the run</param>
        /// <param name="season">Which season will be processed in the run</param>
        /// <param name="method">Which method will be processed in the run</param>
        /// <param name="week">Which week will be processed in the run</param>
        /// <param name="bgForm">The main form for logging purposes</param>
        public static void ProcessFiles(string league, string season, Method method, string week, BeatGraphForm bgForm)
        {
            form = bgForm;

            // Creates necessary directory structure if it doesn't yet exist.
            if (!Directory.Exists($"{filePath}\\{league}"))
                Directory.CreateDirectory($"{filePath}\\{league}");
            if (!Directory.Exists($"{filePath}\\{league}\\{method.ToString()[0]}"))
                Directory.CreateDirectory($"{filePath}\\{league}\\{method.ToString()[0]}");
            if (!Directory.Exists($"{filePath}\\{league}\\{method.ToString()[0]}\\{season}"))
                Directory.CreateDirectory($"{filePath}\\{league}\\{method.ToString()[0]}\\{season}");

            // Set up commands for GraphViz
            string imageFile = $"{filePath}\\{league}\\{method.ToString()[0]}\\{season}\\{week}.png";
            string command = string.Concat(new object[] { "-Tpng -o\"", imageFile, "\" -Kdot \"", $"{filePath}GraphOut.txt\"" });

            // Execute steps to build required files
            PrintWebContent(league, season, method, week);
            PrintGraphFile(league, season, method);

            // Execute GraphViz based on graph file results
            // TODO: Move this to a function of its own? Probably in the Helper class since it's external
            Process pGraphVis = new Process();
            pGraphVis.StartInfo.FileName = @"D:\Graphviz\bin\dot.exe";
            pGraphVis.StartInfo.Arguments = command;
            pGraphVis.StartInfo.RedirectStandardOutput = true;
            pGraphVis.StartInfo.UseShellExecute = false;
            pGraphVis.StartInfo.CreateNoWindow = true;
            pGraphVis.Start();
            pGraphVis.WaitForExit();

            // Upload files to BeatGraphs.com
            if (ftpEnabled)
                FtpFiles(league, season, method, week);
        }

        /// <summary>
        /// Creates the web page and Top5 subsection
        /// </summary>
        private static void PrintWebContent(string league, string season, Method method, string week)
        {
            StringBuilder sbOut = new StringBuilder();
            StringBuilder sbTop5 = new StringBuilder();
            TextWriter twOut = new StreamWriter($"{filePath}/{league}/{method.ToString()[0]}/{season}/{week}.php", false);

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
            // TODO: Try to simplify into something like Builder.matrix.Where(m => m.WHEREALL(s => s.ScoreList == 0)).Join(", ")
            sbOut.Append("<br><b>These teams have no surviving BeatLosses:</b><br>\n");
            bool bLess, bFirst = true;
            foreach (Team tTeam in Builder.matrix)
            {
                bLess = true;
                foreach (Team tTeam2 in Builder.matrix)
                {
                    if (tTeam2.ScoreList[tTeam.franchiseID] > 0)
                    {
                        bLess = false;
                        continue;
                    }
                }

                if (bLess)
                {
                    if (!bFirst)
                        sbOut.Append($", {tTeam.abbreviation}");
                    else
                    {
                        sbOut.Append(tTeam.abbreviation);
                        bFirst = false;
                    }
                }
            }

            sbOut.Append("\n<br>\n<br><b>These teams have no surviving BeatWins:</b><br>\n");
            bFirst = true;
            foreach (Team tTeam in Builder.matrix)
            {
                bLess = true;
                foreach (var dScore in tTeam.ScoreList)
                {
                    if (dScore.Value > 0)
                    {
                        bLess = false;
                        continue;
                    }
                }

                if (bLess)
                {
                    if (!bFirst)
                        sbOut.Append($", {tTeam.abbreviation}");
                    else
                    {
                        sbOut.Append(tTeam.abbreviation);
                        bFirst = false;
                    }
                }
            }

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
            sbTop5.Append($@"<a href=""graphs.php?league={league}&method={method}""><div class='top5header'>{method.ToString()}<div class='toptop'> Top 5</div></div></a>");
            sbTop5.Append("\n<div class='top5row'><div class='top5subhead'>#</div><div class='top5subhead'>Team</div><div class='top5subhead'>Score</div></div>\n");

            // TODO: What am I doing here? Is this necessary?
            twOut.Write(sbOut.ToString());
            sbOut.Replace("\n", Environment.NewLine);
            sbOut = new StringBuilder();

            // Write the score table for the left side of the page
            int rank = 0, stored = 0;
            double lastScore = 0;
            for (int i = 0; i < Builder.matrix.Count; i++)
            {
                // Find the index of the team with the highest score
                int maxindex = GetMaxIndex();

                sbOut.Append("\n<div class='scorerow'>");
                if (double.Parse(Builder.matrix[maxindex].score) == lastScore)
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
                sbOut.Append($"<div class='scoresamplecell'><img src='{Helpers.GetImage(Builder.matrix[maxindex].franchiseID, league, season)}'></div>");
                sbOut.Append($"<div class='scoresamplecell'>{string.Format("{0:N2}", Math.Round(double.Parse(Builder.matrix[maxindex].score), 2))}</div>");
                sbOut.Append($"<div class='scoresamplecell'>{nFormat(Math.Round(Builder.winPoints[maxindex], 2))}</div>");
                sbOut.Append($"<div class='scoresamplecell'>{nFormat(Math.Round(Builder.lossPoints[maxindex], 2))}</div></div>");

                // Make a copy of this row if it's one of the top 5 teams.
                if (i < 5)
                {
                    sbTop5.Append($"<div class='top5row'><div class='top5cell'>{i + 1}</div>");
                    sbTop5.Append($"<div class='top5cell top5midcell'><img src='{Helpers.GetImage(Builder.matrix[maxindex].franchiseID, league, season)}' /></div>");
                    sbTop5.Append($"<div class='top5cell'>{string.Format("{0:N2}", Math.Round(double.Parse(Builder.matrix[maxindex].score), 2))}</div></div>");
                }

                lastScore = double.Parse(Builder.matrix[maxindex].score); // Track this team's score to see if the next team has the same score
                Builder.matrix[maxindex].score = "-9999999999999"; // Set this team's score impossibly low to mark it as "used"
                // TODO: Perhaps add a boolean for "scoreUsed" to eliminate this ugly magic number
            }

            // Close out the files and write them to disk.
            sbOut.Append($"</div></div>\n");
            sbOut.Append($"<div class='grapharea'>\n");
            sbOut.Append($"<img src='{league}/{method.ToString()[0]}/{season}/{week}.png' />\n");
            sbOut.Append($"</div></div></div>\n");

            sbTop5.Append("\n</div>");
            sbTop5 = sbTop5.Replace("\n", Environment.NewLine);
            TextWriter tw5Out = new StreamWriter($"{filePath}/{league}_{method.ToString()}.php", false);
            tw5Out.Write(sbTop5.ToString());
            tw5Out.Close();

            twOut.Write(sbOut.ToString());
            sbOut.Replace("\n", Environment.NewLine);
            twOut.Close();
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
            TextWriter twOut = new StreamWriter($"{filePath}GraphOut.txt", false);
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

            // Recalculate scores as they've been crushed by the web file writer
            // TODO: If the boolean is created, maybe this won't be necessary anymore?
            Builder.CalculateScores();

            // Add each team's properties to the file
            foreach (Team tTeam in Builder.matrix)
            {
                scores.Add(double.Parse(tTeam.score));
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

                sbOut.Append($"\n\t\"{tTeam.abbreviation.Replace(" ", "")}\" [fillcolor=\"{divColor}\"][color=\"{confColor}\"][label=<<TABLE border='0' cellpadding='0' cellspacing='0'><TR><TD><IMG SRC='{filePath}{Helpers.GetImage(tTeam.franchiseID, league, season)}'/></TD></TR><TR><TD>{tTeam.abbreviation}</TD></TR></TABLE>>];");
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
                Builder.matrix.FindAll(delegate (Team t) { return double.Parse(t.score) == minScore; }).ForEach(team => 
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
                        // TODO: lamda?
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
            twOut.Write(sbOut.ToString());
            twOut.Close();
        }

        /// <summary>
        /// Upload the created files to the web site.
        /// </summary>
        private static void FtpFiles(string league, string season, Method method, string week)
        {
            // Build directory structure on the web site if necessary.
            try
            {
                form.Log("Creating League Directory", LogLevel.verbose);
                Helpers.FtpCreateDirectory(league);
                form.Log("Creating Method Directory", LogLevel.verbose);
                Helpers.FtpCreateDirectory($"{league}/{method.ToString()}");
                form.Log("Creating Season Directory", LogLevel.verbose);
                Helpers.FtpCreateDirectory($"{league}/{method.ToString()}/{season}");
            }
            catch (Exception ex)
            {
                form.Log($"Error building ftp directory tree: {ex.Message}", LogLevel.error);
                return;
            }

            // Send the files
            string uploadPath = $"{filePath}/{league}/{method.ToString()}/{season}/{week}";
            ftpFile(filePath, $"{league}/{method.ToString()}/{season}/{week}", ".php");
            ftpFile(filePath, $"{league}/{method.ToString()}/{season}/{week}", ".png");

            // TODO: Missing Top5 upload?
            // TODO: Test the above, if it works, the below can be deleted.
            //try
            //{
            //    // Copy the contents of the web file to the request stream.
            //    if (File.Exists($"{uploadPath}.php"))
            //    {
            //        form.Log("Uploading Web File", LogLevel.verbose);

            //        byte[] fileContents = File.ReadAllBytes($"{uploadPath}.php");
            //    }
            //    else
            //    {
            //        form.Log($"Web File could not be found.  Upload failed.", LogLevel.error);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    form.Log($"Error uploading web file: {ex.Message}");
            //}

            //try
            //{
            //    // Copy the contents of the graph file to the request stream.
            //    if (File.Exists($"{uploadPath}.png"))
            //    {
            //        form.Log("Uploading Graph File", LogLevel.verbose);

            //        byte[] fileContents = File.ReadAllBytes($"{uploadPath}.png");
            //    }
            //    else
            //    {
            //        form.Log($"Graph File could not be found.  Upload failed.", LogLevel.error);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    form.Log($"Error uploading graph file: {ex.Message}");
            //}
        }

        /// <summary>
        /// Responsible for setting up the FTP call
        /// </summary>
        /// <param name="directory">The base directory of the file on disk</param>
        /// <param name="path">The path of the file in the tree</param>
        /// <param name="extension"></param>
        private static void ftpFile(string directory, string path, string extension)
        {
            try
            {
                // Copy the contents of the file to the request stream.
                if (File.Exists($"{directory}{path}.{extension}"))
                {
                    form.Log($"Uploading {path}.{extension}", LogLevel.verbose);

                    byte[] fileContents = File.ReadAllBytes($"{path}.{extension}");
                    Helpers.FtpUploadFile($"{path}.{extension}", fileContents);
                }
                else
                {
                    form.Log($"File could not be found. Upload failed.", LogLevel.error);
                }
            }
            catch (Exception ex)
            {
                form.Log($"Error uploading file: {ex.Message}");
            }
        }

        /// <summary>
        /// Finds the index of the team with the highest score
        /// </summary>
        private static int GetMaxIndex()
        {
            int maxIndex = 0;
            double maxScore = double.Parse(Builder.matrix[maxIndex].score);

            // TODO: Can we do this through a lambda expression?.. should be able to eliminate maxScore even if not, especially if we change score to double
            for (int i = 1; i < Builder.matrix.Count; i++)
            {
                if (double.Parse(Builder.matrix[i].score) > maxScore)
                {
                    maxIndex = i;
                    maxScore = double.Parse(Builder.matrix[maxIndex].score);
                }
            }

            return maxIndex;
        }
    }
}
