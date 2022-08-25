using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BeatGraphs
{
    public static class Settings
    {
        private static Dictionary<string, bool> settings; 
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
                settings = JsonConvert.DeserializeObject<Dictionary<string, bool>>(settingsText);
            }
            catch
            {
                // If the file read fails, it didn't exist. Create the object with defaults
                settings = new Dictionary<string, bool>();
                settings.Add("verbose", false);
                settings.Add("upload", false);
                
                // Save the defaults to file so going forward this isn't necessary
                var settingsText = JsonConvert.SerializeObject(settings);
                Helpers.WriteFile(BasePath.settings, $@"\{settingsFileName}", settingsText);
            }
        }

        /// <summary>
        /// Triggered when the user saves through the settings form. Save settings to file.
        /// </summary>
        public static void SaveSettings(bool verbose, bool upload)
        {
            // Update the settings object to be used throughout the app
            settings["verbose"] = verbose;
            settings["upload"] = upload;

            // Save the settings options to file
            var settingsText = JsonConvert.SerializeObject(settings);
            Helpers.WriteFile(BasePath.settings, $@"\{settingsFileName}", settingsText);
        }

        /// <summary>
        /// Accessor to retrive a key setting
        /// </summary>
        public static bool Get(string key)
        {
            if (!settings.ContainsKey(key))
                throw new Exception($"No setting '{key}' found.");

            return settings[key];
        }
    }
}
