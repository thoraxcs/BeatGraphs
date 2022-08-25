using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeatGraphs
{
    public class Team
    {
        public int franchiseID;         // This team's FranchiseID according to the database
        public string city;             // The team's city
        public string mascot;           // The team's nickname
        public string abbreviation;     // The team's abbreviation to be displayed on the graphs
        public string conference;       // The conference the team is a member of (used to determine the color of the border of the cell on the graph)
        public string division;         // The division the team is a member of (used to determine the fill color of the cell on the graph)
        public double score;            // The team's raw score to determine rankings
        public Dictionary<int, double> ScoreList; // Lists the total weight of all wins against each team
        public int index;               // Track the team's own index for quick reference

        /// <summary>
        /// This Constructor requires a franchise ID and a league marker that will allow us to 
        /// look up data about the team from the database
        /// </summary>
        /// <param name="teamID">The team's Franchise ID.</param>
        /// <param name="cbLeague">The league this team is a member of</param>
        /// <param name="year">The league year (season) to look up</param>
        public Team(int teamID, string cbLeague, int year)
        {
            // TODO: Move SQL to helper
            SQLDatabaseAccess SQLDBA = new SQLDatabaseAccess();
            SqlParameter[] sqlParam = new SqlParameter[3];
            SqlDataReader sqlDR;

            SQLDBA.Open();
            sqlParam[0] = SQLDBA.CreateParameter("@FranchiseID", SqlDbType.Int, 64, ParameterDirection.Input, teamID);
            sqlParam[1] = SQLDBA.CreateParameter("@Year", SqlDbType.Int, 64, ParameterDirection.Input, year);
            sqlParam[2] = SQLDBA.CreateParameter("@League", SqlDbType.NVarChar, 10, ParameterDirection.Input, cbLeague);

            SQLDBA.ExecuteSqlSP("Select_Team", sqlParam, out sqlDR);
            // Select the team's data from the database

            if (sqlDR.HasRows)
            {
                // Populate retrieved data into the member variables
                sqlDR.Read();

                franchiseID = int.Parse(SQLDBA.sqlGet(sqlDR, "FranchiseID"));
                city = SQLDBA.sqlGet(sqlDR, "City");
                mascot = SQLDBA.sqlGet(sqlDR, "Mascot");
                abbreviation = SQLDBA.sqlGet(sqlDR, "Abbreviation");
                conference = SQLDBA.sqlGet(sqlDR, "Conference");
                division = SQLDBA.sqlGet(sqlDR, "Division");
            }

            ScoreList = new Dictionary<int, double>();

            SQLDBA.Close();
            sqlDR.Close();
            SQLDBA.Dispose();
            sqlDR.Dispose();
        }

        /// <summary>
        /// When it is determined that this team has earned BeatWins (or points in the case of
        /// the Weighted method) over another team, the points are logged in the score list)
        /// </summary>
        /// <param name="teamIndex">The index within the ScoreList of the team to add points to.</param>
        /// <param name="score">The amount of points to add over the indicated team.</param>
        public void AddScore(int teamIndex, double score)
        {
            ScoreList[teamIndex] = ScoreList[teamIndex] + score;
        }

        /// <summary>
        /// Used to initialize the array list that contains the points this team has over every
        /// other team in the league.
        /// </summary>
        /// <param name="teams">The amount of teams in the league.</param>
        public void BuildScoreList(List<int> teams)
        {
            teams.ForEach(t => ScoreList.Add(t, 0));
        }
    }
}
