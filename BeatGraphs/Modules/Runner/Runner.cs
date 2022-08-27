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

                var season = new List<string>();
                season.Add(info[league].Item1.ToString()); // Only send the most current season for the league
                var leagues = inLeagues.Where(l => l == league).ToList(); // Updater takes a list of leagues, so make the desired league into a list

                // Call the Updater and Builder to process the most current data
                Updater.Run(leagues, info[league].Item1.ToString());
                Builder.Run(leagues, season, methods, -1); // -1 tells the Builder to run the most current week only

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
    }
}
