namespace BeatGraphs
{
    /// <summary>
    /// This class is used to track the series involve in a league's playoff year. It helps to build a tree
    /// where each series contains the info for a winner of the series and links to series identified by the
    /// participants in the series (home/away).
    /// </summary>
    public class Series
    {
        public int round, winnerID, homeID, awayID;
        public string iconURL;
        public Series awayTeam, homeTeam;

        /// <summary>
        /// Constructor to build the pieces of the series.
        /// </summary>
        public Series(int rnd, int winner, int home, int away)
        {
            round = rnd;
            winnerID = winner;
            homeID = home;
            awayID = away;
            iconURL = "";
            awayTeam = null;
            homeTeam = null;
        }
    }
}
