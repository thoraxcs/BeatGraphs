using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace BeatGraphs
{
    /// <summary>
    /// The helper class is used to handle interactions outside of the application space such as database access and FTP requests
    /// </summary>
    public static class Helpers
    {
        private static readonly string ftpPath = ConfigurationManager.AppSettings.Get("ftpPath");
        private static readonly string ftpUser = ConfigurationManager.AppSettings.Get("ftpUser");
        private static readonly string ftpPass = ConfigurationManager.AppSettings.Get("ftpPass");
        private static readonly string gvPath = ConfigurationManager.AppSettings.Get("graphvizPath");
        private static readonly string filePath = ConfigurationManager.AppSettings.Get("filePath");
        private static readonly string settingsPath = Process.GetCurrentProcess().MainModule.FileName.Substring(0, Process.GetCurrentProcess().MainModule.FileName.LastIndexOf(@"\"));


        #region File
        public static void InitializeDirectory(string league, string method, string season)
        {
            if (!Directory.Exists($"{filePath}\\{league}"))
                Directory.CreateDirectory($"{filePath}\\{league}");
            if (!Directory.Exists($"{filePath}\\{league}\\{method}"))
                Directory.CreateDirectory($"{filePath}\\{league}\\{method}");
            if (!Directory.Exists($"{filePath}\\{league}\\{method}\\{season}"))
                Directory.CreateDirectory($"{filePath}\\{league}\\{method}\\{season}");
        }

        public static void WriteFile(BasePath basePath, string file, string text)
        {
            File.WriteAllText($"{GetPath(basePath)}{file}", text);
        }

        public static string ReadFile(BasePath basePath, string file)
        {
            if (!File.Exists($"{GetPath(basePath)}{file}"))
            {
                throw new Exception($"File {GetPath(basePath)}{file} does not exist to be read.");
            }

            return File.ReadAllText($"{GetPath(basePath)}{file}");
        }

        private static string GetPath(BasePath basePath)
        {
            switch (basePath)
            {
                case BasePath.file:
                    return filePath;
                case BasePath.settings:
                    return settingsPath;
                default:
                    return "";
            }
        }
        #endregion

        #region GraphViz
        public static void GenerateGraph(string imgPath)
        {
            string command = string.Concat(new object[] { "-Tpng -o\"", $"{filePath}{imgPath}", "\" -Kdot \"", $"{filePath}GraphOut.txt\"" });

            Process pGraphVis = new Process();
            pGraphVis.StartInfo.FileName = gvPath;
            pGraphVis.StartInfo.Arguments = command;
            pGraphVis.StartInfo.RedirectStandardOutput = true;
            pGraphVis.StartInfo.UseShellExecute = false;
            pGraphVis.StartInfo.CreateNoWindow = true;
            pGraphVis.Start();
            pGraphVis.WaitForExit();
        }
        #endregion

        #region SQL
        /// <summary>
        /// Clear all games for the season.
        /// </summary>
        public static void ClearSeason(int seasonID)
        {
            SQLDatabaseAccess SQLDBA = new SQLDatabaseAccess();
            SqlParameter[] sqlParam = new SqlParameter[1];

            sqlParam[0] = SQLDBA.CreateParameter("@SeasonID", SqlDbType.Int, 64, ParameterDirection.Input, seasonID);

            SQLDBA.Open();
            SQLDBA.ExecuteSqlSP("Clear_Season", sqlParam);

            SQLDBA.Close();
            SQLDBA.Dispose();
        }

        /// <summary>
        /// Insert game data into the database
        /// </summary>
        public static void InsertGame(int seasonID, int week, int awayID, int awayScore, int homeID, int homeScore)
        {
            SQLDatabaseAccess SQLDBA = new SQLDatabaseAccess();
            SqlParameter[] sqlParam = new SqlParameter[6];

            sqlParam[0] = SQLDBA.CreateParameter("@SeasonID", SqlDbType.Int, 64, ParameterDirection.Input, seasonID);
            sqlParam[1] = SQLDBA.CreateParameter("@WeekID", SqlDbType.Int, 64, ParameterDirection.Input, week);
            sqlParam[2] = SQLDBA.CreateParameter("@AwayID", SqlDbType.Int, 64, ParameterDirection.Input, awayID);
            sqlParam[3] = SQLDBA.CreateParameter("@AwayScore", SqlDbType.Int, 64, ParameterDirection.Input, awayScore);
            sqlParam[4] = SQLDBA.CreateParameter("@HomeID", SqlDbType.Int, 64, ParameterDirection.Input, homeID);
            sqlParam[5] = SQLDBA.CreateParameter("@HomeScore", SqlDbType.Int, 64, ParameterDirection.Input, homeScore);

            SQLDBA.Open();
            SQLDBA.ExecuteSqlSP("Insert_Game", sqlParam);

            SQLDBA.Close();
            SQLDBA.Dispose();
        }

        /// <summary>
        /// Inserts a playoff matchup into the tracking table for use in building playoff history page
        /// </summary>
        public static void InsertPlayoffs(int seasonID, KeyValuePair<Tuple<int, int>, int> matchup, int winnerID)
        {
            SQLDatabaseAccess SQLDBA = new SQLDatabaseAccess();
            SqlParameter[] sqlParam = new SqlParameter[5];

            sqlParam[0] = SQLDBA.CreateParameter("@SeasonID", SqlDbType.Int, 64, ParameterDirection.Input, seasonID);
            sqlParam[1] = SQLDBA.CreateParameter("@Range", SqlDbType.Int, 64, ParameterDirection.Input, matchup.Value);
            sqlParam[2] = SQLDBA.CreateParameter("@WinnerID", SqlDbType.Int, 64, ParameterDirection.Input, winnerID);
            sqlParam[3] = SQLDBA.CreateParameter("@AwayID", SqlDbType.Int, 64, ParameterDirection.Input, matchup.Key.Item1);
            sqlParam[4] = SQLDBA.CreateParameter("@HomeID", SqlDbType.Int, 64, ParameterDirection.Input, matchup.Key.Item2);

            SQLDBA.Open();
            SQLDBA.ExecuteSqlSP("Insert_Playoff", sqlParam);

            SQLDBA.Close();
            SQLDBA.Dispose();
        }

        /// <summary>
        /// Gets the ID for the season represented by the given league and year
        /// </summary>
        public static int FindSeason(int year, string league)
        {
            SQLDatabaseAccess SQLDBA = new SQLDatabaseAccess();
            SqlParameter[] sqlParam = new SqlParameter[2];
            SqlDataReader sqlDR;
            int iSeasonID;

            sqlParam[0] = SQLDBA.CreateParameter("@Year", SqlDbType.Int, 64, ParameterDirection.Input, year);
            sqlParam[1] = SQLDBA.CreateParameter("@League", SqlDbType.NVarChar, 50, ParameterDirection.Input, league);

            SQLDBA.Open();
            SQLDBA.ExecuteSqlSP("Select_SeasonID", sqlParam, out sqlDR);

            if (sqlDR.HasRows)
            {
                sqlDR.Read();
                iSeasonID = int.Parse(sqlDR.GetSqlValue(0).ToString());
            }
            else
                iSeasonID = -1;

            SQLDBA.Close();
            sqlDR.Close();
            SQLDBA.Dispose();
            sqlDR.Dispose();

            return iSeasonID;
        }

        /// <summary>
        /// Gets the ID for the team participating in the given league for the season by the name provided
        /// </summary>
        /// <param name="team"></param>
        /// <param name="league"></param>
        /// <param name="season"></param>
        /// <returns></returns>
        public static int GetTeamID(string team, string league, string season)
        {
            SQLDatabaseAccess SQLDBA = new SQLDatabaseAccess();
            SqlParameter[] sqlParam = new SqlParameter[3];
            SqlDataReader sqlDR;
            int iTeam = -1;

            SQLDBA.Open();
            sqlParam[0] = SQLDBA.CreateParameter("@League", SqlDbType.NVarChar, 50, ParameterDirection.Input, league);
            sqlParam[1] = SQLDBA.CreateParameter("@TeamName", SqlDbType.NVarChar, 50, ParameterDirection.Input, team);
            sqlParam[2] = SQLDBA.CreateParameter("@Season", SqlDbType.NVarChar, 50, ParameterDirection.Input, season);

            // We check MLB by Baseball-Reference's team abbreviation because they sometimes use D'backs which is unofficial
            if (league == "MLB")
                SQLDBA.ExecuteSqlSP("Select_TeamID_By_RefAbbr", sqlParam, out sqlDR);
            else
                SQLDBA.ExecuteSqlSP("Select_TeamID_By_Full_Name", sqlParam, out sqlDR);

            if (sqlDR.HasRows)
            {
                sqlDR.Read();
                iTeam = int.Parse(sqlDR.GetSqlValue(0).ToString());
            }

            SQLDBA.Close();
            sqlDR.Close();
            SQLDBA.Dispose();
            sqlDR.Dispose();

            return iTeam;
        }

        /// <summary>
        /// Gets a list of weeks logged for the give league year
        /// </summary>
        public static List<int> GetWeeks(int year, string league)
        {
            SQLDatabaseAccess SQLDBA = new SQLDatabaseAccess();
            SqlParameter[] sqlParam = new SqlParameter[2];
            SqlDataReader sqlDR;
            var weeks = new List<int>();

            sqlParam[0] = SQLDBA.CreateParameter("@Year", SqlDbType.Int, 64, ParameterDirection.Input, year);
            sqlParam[1] = SQLDBA.CreateParameter("@League", SqlDbType.NVarChar, 50, ParameterDirection.Input, league);

            SQLDBA.Open();
            SQLDBA.ExecuteSqlSP("Select_Weeks", sqlParam, out sqlDR);

            if (sqlDR.HasRows)
            {
                while (sqlDR.Read())
                {
                    weeks.Add(int.Parse(SQLDBA.sqlGet(sqlDR, "RangeID")));
                }
            }

            SQLDBA.Close();
            sqlDR.Close();
            SQLDBA.Dispose();
            sqlDR.Dispose();

            return weeks;
        }

        /// <summary>
        /// Retrieves a team's information from the DB
        /// </summary>
        public static Team SelectTeam(int teamID, int year, string league)
        {
            var team = new Team();
            SQLDatabaseAccess SQLDBA = new SQLDatabaseAccess();
            SqlParameter[] sqlParam = new SqlParameter[3];
            SqlDataReader sqlDR;

            SQLDBA.Open();
            sqlParam[0] = SQLDBA.CreateParameter("@FranchiseID", SqlDbType.Int, 64, ParameterDirection.Input, teamID);
            sqlParam[1] = SQLDBA.CreateParameter("@Year", SqlDbType.Int, 64, ParameterDirection.Input, year);
            sqlParam[2] = SQLDBA.CreateParameter("@League", SqlDbType.NVarChar, 10, ParameterDirection.Input, league);

            SQLDBA.ExecuteSqlSP("Select_Team", sqlParam, out sqlDR);
            // Select the team's data from the database

            if (sqlDR.HasRows)
            {
                // Populate retrieved data into the member variables
                sqlDR.Read();

                team.franchiseID = int.Parse(SQLDBA.sqlGet(sqlDR, "FranchiseID"));
                team.city = SQLDBA.sqlGet(sqlDR, "City");
                team.mascot = SQLDBA.sqlGet(sqlDR, "Mascot");
                team.abbreviation = SQLDBA.sqlGet(sqlDR, "Abbreviation");
                team.conference = SQLDBA.sqlGet(sqlDR, "Conference");
                team.division = SQLDBA.sqlGet(sqlDR, "Division");
            }

            SQLDBA.Close();
            sqlDR.Close();
            SQLDBA.Dispose();
            sqlDR.Dispose();

            return team;
        }

        /// <summary>
        /// Gets the most current week for each league
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, Tuple<int, int>> GetCurrentWeeks()
        {
            SQLDatabaseAccess SQLDBA = new SQLDatabaseAccess();
            SqlDataReader sqlDR;
            var weeks = new Dictionary<string, Tuple<int, int>>();

            SQLDBA.Open();
            SQLDBA.ExecuteSqlSP("Select_Current_Weeks", out sqlDR);

            if (sqlDR.HasRows)
            {
                while (sqlDR.Read())
                {
                    weeks.Add(SQLDBA.sqlGet(sqlDR, "League"),
                        new Tuple<int, int>(int.Parse(SQLDBA.sqlGet(sqlDR, "Year")),
                                            int.Parse(SQLDBA.sqlGet(sqlDR, "maxWeek"))));
                }
            }

            SQLDBA.Close();
            sqlDR.Close();
            SQLDBA.Dispose();
            sqlDR.Dispose();

            return weeks;
        }

        /// <summary>
        /// Gets the season status for each league
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, Tuple<int, int, int>> GetSeasonStatus()
        {
            SQLDatabaseAccess SQLDBA = new SQLDatabaseAccess();
            SqlDataReader sqlDR;
            var status = new Dictionary<string, Tuple<int, int, int>>();

            SQLDBA.Open();
            SQLDBA.ExecuteSqlSP("Select_Season_Status", out sqlDR);

            if (sqlDR.HasRows)
            {
                while (sqlDR.Read())
                {
                    status.Add(SQLDBA.sqlGet(sqlDR, "League"),
                        new Tuple<int, int, int>(int.Parse(SQLDBA.sqlGet(sqlDR, "Year")),
                                            int.Parse(SQLDBA.sqlGet(sqlDR, "RangeID")),
                                            int.Parse(SQLDBA.sqlGet(sqlDR, "ChampWins"))));
                }
            }

            SQLDBA.Close();
            sqlDR.Close();
            SQLDBA.Dispose();
            sqlDR.Dispose();

            return status;
        }

        /// <summary>
        /// Inserts the current year's season to the DB if not there
        /// </summary>
        /// <returns></returns>
        public static void InsertSeason(string league)
        {
            SQLDatabaseAccess SQLDBA = new SQLDatabaseAccess();
            SqlParameter[] sqlParam = new SqlParameter[1];

            SQLDBA.Open();
            sqlParam[0] = SQLDBA.CreateParameter("@League", SqlDbType.NVarChar, 50, ParameterDirection.Input, league);
            SQLDBA.ExecuteSqlSP("Insert_Season", sqlParam);

            SQLDBA.Close();
            SQLDBA.Dispose();
        }

        /// <summary>
        /// Gets a list of teams participating in a given league year
        /// </summary>
        public static List<int> GetTeams(string league, string season)
        {
            SQLDatabaseAccess SQLDBA = new SQLDatabaseAccess();
            SqlParameter[] sqlParam = new SqlParameter[2];
            SqlDataReader sqlDR;
            var teams = new List<int>();

            SQLDBA.Open();
            sqlParam[0] = SQLDBA.CreateParameter("@Year", SqlDbType.NVarChar, 50, ParameterDirection.Input, season);
            sqlParam[1] = SQLDBA.CreateParameter("@League", SqlDbType.NVarChar, 50, ParameterDirection.Input, league);
            SQLDBA.ExecuteSqlSP("Select_Teams_By_Year", sqlParam, out sqlDR);
            // Get all teams for the league in the specified year

            if (sqlDR.HasRows)
            {
                while (sqlDR.Read())
                {
                    teams.Add(int.Parse(SQLDBA.sqlGet(sqlDR, "FranchiseID")));
                }
            }

            SQLDBA.Close();
            sqlDR.Close();
            SQLDBA.Dispose();
            sqlDR.Dispose();

            return teams;
        }

        /// <summary>
        /// Returns all of the games logged in a given league season up to and including the given week.
        /// </summary>
        public static List<Game> GetGames(string league, string season, string week)
        {
            SQLDatabaseAccess SQLDBA = new SQLDatabaseAccess();
            SqlParameter[] sqlParam = new SqlParameter[3];
            SqlDataReader sqlDR;
            var games = new List<Game>();
            int iTeamAway, iTeamHome;
            double dScoreAway, dScoreHome;

            SQLDBA.Open();
            sqlParam[0] = SQLDBA.CreateParameter("@Year", SqlDbType.NVarChar, 50, ParameterDirection.Input, season);
            sqlParam[1] = SQLDBA.CreateParameter("@League", SqlDbType.NVarChar, 50, ParameterDirection.Input, league);
            sqlParam[2] = SQLDBA.CreateParameter("@Range", SqlDbType.NVarChar, 10, ParameterDirection.Input, week);
            SQLDBA.ExecuteSqlSP("Select_Games", sqlParam, out sqlDR);

            if (sqlDR.HasRows)
            {
                while (sqlDR.Read())
                {
                    iTeamAway = int.Parse(SQLDBA.sqlGet(sqlDR, "AwayID"));
                    iTeamHome = int.Parse(SQLDBA.sqlGet(sqlDR, "HomeID"));
                    dScoreAway = double.Parse(SQLDBA.sqlGet(sqlDR, "AwayScore"));
                    dScoreHome = double.Parse(SQLDBA.sqlGet(sqlDR, "HomeScore"));

                    if (dScoreAway > dScoreHome)
                        games.Add(new Game(iTeamAway, iTeamHome, dScoreAway - dScoreHome));
                    else
                        games.Add(new Game(iTeamHome, iTeamAway, dScoreHome - dScoreAway));
                }
            }

            SQLDBA.Close();
            sqlDR.Close();
            SQLDBA.Dispose();
            sqlDR.Dispose();

            return games;
        }

        /// <summary>
        /// Gets the icon for a team for a given league year
        /// </summary>
        public static string GetImage(int franchiseID, string league, string season, bool useFilePath = false)
        {
            SQLDatabaseAccess SQLDBA = new SQLDatabaseAccess();
            SqlParameter[] sqlParam = new SqlParameter[2];
            SqlDataReader sqlDR;
            string sImage = $@"images\{league}\";

            SQLDBA.Open();
            sqlParam[0] = SQLDBA.CreateParameter("@Year", SqlDbType.NVarChar, 50, ParameterDirection.Input, season);
            sqlParam[1] = SQLDBA.CreateParameter("@FranchiseID", SqlDbType.NVarChar, 50, ParameterDirection.Input, franchiseID.ToString());
            SQLDBA.ExecuteSqlSP("Select_Team_Image", sqlParam, out sqlDR);

            if (sqlDR.HasRows)
            {
                sqlDR.Read();
                sImage += SQLDBA.sqlGet(sqlDR, "IconURL");
            }

            sqlDR.Close();
            sqlDR.Dispose();
            SQLDBA.Close();
            SQLDBA.Dispose();

            if (useFilePath)
                return $"{filePath}/{sImage}.png";
            return $"{sImage}.png";

        }
        #endregion

        #region FTP
        /// <summary>
        /// Creates an FTP file directory at the given location if it doesn't yet exist
        /// </summary>
        /// <param name="path"></param>
        public static void FtpCreateDirectory(string path)
        {

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"{ftpPath}{path}");
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.Credentials = new NetworkCredential(ftpUser, ftpPass);
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            response.Close();
        }

        /// <summary>
        /// Uploads the given file to the web directory
        /// </summary>
        public static void FtpUploadFile(string path, byte[] file)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"{ftpPath}{path}");
            request.Method = WebRequestMethods.Ftp.UploadFile;

            request.Credentials = new NetworkCredential(ftpUser, ftpPass);

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(file, 0, file.Length);
            requestStream.Close();

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            response.Close();
        }
        #endregion

        #region HTML
        /// <summary>
        /// Read the HTML from a given URL. ErrorThrow allows the designation of how serious failure to find the page is.
        /// </summary>
        public static string GetHtml(string url, bool errorThrow = true)
        {
            string html = "";

            try
            {
                HttpWebRequest hwRequest = (HttpWebRequest)WebRequest.Create(url);
                using (HttpWebResponse hwResponse = (HttpWebResponse)hwRequest.GetResponse())
                {
                    using (StreamReader srReader = new StreamReader(hwResponse.GetResponseStream()))
                    {
                        html = srReader.ReadToEnd();
                    }
                }
            }
            catch
            {
                if (errorThrow)
                {
                    Logger.Log($"Requested page could not be accessed and may not yet exist: {url}", LogLevel.error);
                    throw;
                }
                else
                {
                    Logger.Log($"Requested page could not be accessed and may not yet exist: {url}", LogLevel.warning);
                }
            }
            return html;
        }
        #endregion
    }
}
