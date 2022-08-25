using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;

namespace BeatGraphs
{
    /// <summary>
    /// The helper class is used to handle interactions outside of the application space such as database access and FTP requests
    /// </summary>
    // TODO: See about putting HTTP requests here and anything else external.
    public static class Helpers
    {
        private static readonly string ftpPath = ConfigurationManager.AppSettings.Get("ftpPath");
        private static readonly string ftpUser = ConfigurationManager.AppSettings.Get("ftpUser");
        private static readonly string ftpPass = ConfigurationManager.AppSettings.Get("ftpPass");

        #region File
        public static void WriteFile(string file, string text)
        {
            File.WriteAllText(file, text);
        }

        public static string ReadFile(string file)
        {
            if (!File.Exists(file))
            {
                throw new Exception($"File {file} does not exist to be read.");
            }

            return File.ReadAllText(file);
        }
        #endregion

        #region SQL
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
        public static string GetImage(int franchiseID, string league, string season)
        {
            SQLDatabaseAccess SQLDBA = new SQLDatabaseAccess();
            SqlParameter[] sqlParam = new SqlParameter[2];
            SqlDataReader sqlDR;
            string sImage = @"images\" + league + "\\";

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

            return sImage + ".png";
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
    }
}
