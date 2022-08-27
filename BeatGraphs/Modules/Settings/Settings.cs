using Newtonsoft.Json;
using System;

namespace BeatGraphs
{
    public class SettingsRecord
    {
        [JsonProperty]
        public bool verbose { get; private set; }
        [JsonProperty]
        public bool upload { get; private set; }
        [JsonProperty]
        public bool mlbRun { get; private set; }
        [JsonProperty]
        public bool nbaRun { get; private set; }
        [JsonProperty]
        public bool nflRun { get; private set; }
        [JsonProperty]
        public bool nhlRun { get; private set; }
        [JsonProperty]
        public bool standardRun { get; private set; }
        [JsonProperty]
        public bool iterativeRun { get; private set; }
        [JsonProperty]
        public bool weightedRun { get; private set; }
        [JsonProperty]
        public bool activeRun { get; private set; }
        [JsonProperty]
        public DateTime nextRun { get; private set; }
        [JsonProperty]
        public DateTime lastRun { get; private set; }

        public static class Settings
        {
            public static SettingsRecord settings { get; private set; }
            static readonly string settingsFileName = "bg.config";

            /// <summary>
            /// Triggered at application load, gets settings from a file
            /// </summary>
            public static void LoadSettings()
            {
                try
                {
                    // Read settings from file and apply them
                    string settingsText = Helpers.ReadFile(BasePath.settings, $@"\{settingsFileName}");
                    settings = JsonConvert.DeserializeObject<SettingsRecord>(settingsText);
                }
                catch
                {
                    // If the file read fails, it didn't exist. Create the object with defaults
                    settings = new SettingsRecord();
                    settings.verbose = false;
                    settings.upload = false;
                    settings.mlbRun = false;
                    settings.nbaRun = false;
                    settings.nflRun = false;
                    settings.nhlRun = false;
                    settings.activeRun = false;
                    settings.nextRun = DateTime.MinValue;
                    settings.lastRun = DateTime.MinValue;
                    Save();
                }
            }

            /// <summary>
            /// Triggered when the user saves through the settings form. Save settings to file.
            /// </summary>
            public static void SaveSettings(bool verbose, bool upload)
            {
                // Update the settings object to be used throughout the app
                settings.verbose = verbose;
                settings.upload = upload;
                Save();
            }

            public static void SaveSchedule(bool mlb, bool nba, bool nfl, bool nhl, bool standard, bool iterative, bool weighted, bool active, DateTime next)
            {
                settings.mlbRun = mlb;
                settings.nbaRun = nba;
                settings.nflRun = nfl;
                settings.nhlRun = nhl;
                settings.standardRun = standard;
                settings.iterativeRun = iterative;
                settings.weightedRun = weighted;
                settings.activeRun = active;
                settings.nextRun = next;
                Save();
            }

            public static void RunTriggeredAt(DateTime runTime)
            {
                settings.lastRun = runTime;
                Save();
            }

            private static void Save()
            {
                // Save the settings options to file
                var settingsText = JsonConvert.SerializeObject(settings);
                Helpers.WriteFile(BasePath.settings, $@"\{settingsFileName}", settingsText);
            }
        }
    }
}
