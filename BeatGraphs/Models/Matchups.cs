using System;
using System.Collections.Generic;

namespace BeatGraphs
{
    /// <summary>
    /// The Matchups class is used to track matchups in playoff rounds
    /// </summary>
    class Matchups
    {
        Dictionary<Tuple<int, int>, int> matches;   // A set of all matchups. Tuple contains TeamID of participants, other int is the round.
        int startRange;                             // If this instance is going to figure out rounds on its own, start range is the first round.
        int firstRoundMatches;                      // Some second round participants have byes and the first round isn't full.
        int firstRoundRemaining;                    // When figuring out when to move to the second round, count down first round matches.

        /// <summary>
        /// Constructor for Matchups. Instantiates the match dictionary.
        /// </summary>
        /// <param name="sr">The starting round: Typicaly 501 represents the first round in modern playoffs.</param>
        /// <param name="frm">How many unique matchups there are in the "wild card round".</param>
        public Matchups(int sr, int frm = 0)
        {
            matches = new Dictionary<Tuple<int, int>, int>();
            startRange = sr;
            firstRoundMatches = firstRoundRemaining = frm;
        }

        /// <summary>
        /// Attempts to insert a new pair into the dictionary
        /// </summary>
        /// <param name="t1">One of the teams in the matchup</param>
        /// <param name="t2">The other team in the matchup</param>
        /// <param name="r">The round in which the matchup happened (OPTIONAL)</param>
        public void Add(int t1, int t2, int r = -1)
        {
            var tpl = new Tuple<int, int>(t1, t2);

            // If directly passed a round ID...
            if (r != -1)
            {
                // ...automatically insert the match at that round if it isn't there already.
                if (!Contains(tpl))
                    matches.Add(tpl, r);
            }
            // When not passed a round ID...
            else
            {
                // ...check to see if the match has already been recorded.
                r = GetRound(t1, t2);
                // If not...
                if (r == -1)
                {
                    // ...find the highest round a participant has played in already.
                    r = Math.Max(FindHighestRound(t1), FindHighestRound(t2));

                    // If one of the teams has played in a previous round, mark this matchup to the next round
                    if (r != -1)
                        matches.Add(tpl, r + 1);
                    else
                    {
                        // Otherwise, neither team has played before, check first round match count to see which round to put it at.
                        if (firstRoundMatches == 0 || firstRoundRemaining-- > 0)
                            matches.Add(tpl, startRange);
                        else
                            matches.Add(tpl, startRange + 1);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the latest round the passed in team has a recorded match
        /// </summary>
        /// <param name="team">Team to look up a match for</param>
        /// <returns></returns>
        private int FindHighestRound(int team)
        {
            int highest = -1;
            foreach (var match in matches)
            {
                // If the team we're looking for is in this match and the round is the latest one we've seen, record it for return.
                if ((match.Key.Item1 == team || match.Key.Item2 == team) && match.Value > highest)
                    highest = match.Value;
            }
            return highest;
        }

        /// <summary>
        /// Returns the actual match object when requested by a team and round. Null returned if no match exists.
        /// </summary>
        /// <param name="round"></param>
        /// <param name="teamID"></param>
        /// <returns></returns>
        public KeyValuePair<Tuple<int, int>, int>? GetMatch(int round, int teamID)
        {
            foreach (var match in matches)
            {
                if (match.Value == round && (match.Key.Item1 == teamID || match.Key.Item2 == teamID))
                    return match;
            }
            return null;
        }

        /// <summary>
        /// Get the round in which the passed in match occurred.
        /// </summary>
        /// <param name="t1">One of the teams in the matchup</param>
        /// <param name="t2">The other team in the matchup</param>
        /// <returns></returns>
        public int GetRound(int t1, int t2)
        {
            // Return the round in which this matchup occurs. We have to check both orders.
            var tpl = new Tuple<int, int>(t1, t2);
            if (matches.ContainsKey(tpl))
                return matches[tpl];
            tpl = new Tuple<int, int>(t2, t1);
            if (matches.ContainsKey(tpl))
                return matches[tpl];
            return -1;
        }

        /// <summary>
        /// Check to see if the matchup has already been logged.
        /// </summary>
        /// <param name="t1">One of the teams in the matchup</param>
        /// <param name="t2">The other team in the matchup</param>
        /// <returns></returns>
        public bool Contains(int t1, int t2)
        {
            var tpl = new Tuple<int, int>(t1, t2);
            if (matches.ContainsKey(tpl))
                return true;
            tpl = new Tuple<int, int>(t2, t1);
            if (matches.ContainsKey(tpl))
                return true;
            return false;
        }

        /// <summary>
        /// Check to see if the matchup has already been logged.
        /// </summary>
        /// <param name="tpl">The matchup to look up</param>
        /// <returns></returns>
        private bool Contains(Tuple<int, int> tpl)
        {
            return Contains(tpl.Item1, tpl.Item2);
        }

        /// <summary>
        /// Remove this matchup from the dictionary
        /// </summary>
        /// <param name="t1">One of the teams in the matchup</param>
        /// <param name="t2">The other team in the matchup</param>
        public void Remove(int t1, int t2)
        {
            var tpl = new Tuple<int, int>(t1, t2);
            if (Contains(tpl))
                matches.Remove(tpl);
            tpl = new Tuple<int, int>(t2, t1);
            if (Contains(tpl))
                matches.Remove(tpl);
        }

        /// <summary>
        /// Simply returns whether or not there is at least one match in the dictionary.
        /// </summary>
        /// <returns></returns>
        public bool HasMatches()
        {
            return matches.Count > 0;
        }
    }
}
