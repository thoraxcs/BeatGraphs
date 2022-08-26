using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BeatGraphs.Modules
{
    public static class Runner
    {
        public static void Run(List<string> inLeagues, List<Method> methods)
        {
            foreach (var league in inLeagues)
            {
                var info = Helpers.GetCurrentWeeks();

                var season = new List<string>();
                season.Add(info[league].Item1.ToString());
                var leagues = inLeagues.Where(l => l == league).ToList();

                Updater.Run(leagues, info[league].Item1.ToString());
                Builder.Run(leagues, season, methods, -1);
            }

            Logger.Log($"Scheduled run complete: {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}", LogLevel.special);
        }

        public static void LoadNewButton(Button button, string league, Tuple<int, int, int> info)
        {
            var season = info.Item1;
            var week = info.Item2;
            var wins = info.Item3;

            if (week == 504)
            {
                if (wins == 4 || (league == "NFL" && wins == 1))
                {
                    button.Enabled = true;
                    button.Text = $"Insert {season + 1} {league} Season";
                    return;
                }
            }

            button.Enabled = false;
            button.Text = week == 0 ? $"{league} Season Not Started" : $"{league} Season In Week {week}";
        }
    }
}
