namespace BeatGraphs
{
    /// <summary>
    /// This object represents a BeatWin. It contains a winning team, a losing team, and the weight of the link between them.
    /// </summary>
    public class Game
    {
        public int winner;
        public int loser;
        public double weight;

        /// <summary>
        /// Constructor which assigns parameters to properties
        /// </summary>
        public Game(int win, int lose, double wght)
        {
            winner = win;
            loser = lose;
            weight = wght;
        }
    }
}
