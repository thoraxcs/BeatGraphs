using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BeatGraphs.Modules;
using Options = BeatGraphs.SettingsRecord.Settings;

namespace BeatGraphs
{
    /// <summary>
    /// Main form for the application
    /// </summary>
    public partial class BeatGraphForm : Form
    {
        private readonly int FIRSTYEAR = 1970; // Lowest limit for year
        private List<string> buildLeagues = new List<string>(); // Selected leagues for builder
        private List<Method> buildMethods = new List<Method>(); // Selected methods for builder
        private bool loading = false;

        /// <summary>
        /// Constructor for the form, initializes.
        /// </summary>
        public BeatGraphForm()
        {
            loading = true;
            InitializeComponent();
            PopulateYears();
            Options.LoadSettings();
            Logger.Initialize(this);
            Helpers.Initialize(loadBar);
            clockTimer.Start();

            var lastRun = Options.settings.lastRun;
            var nextRun = Options.settings.nextRun;
            runMLB.Checked = Options.settings.mlbRun;
            runNFL.Checked = Options.settings.nflRun;
            runNBA.Checked = Options.settings.nbaRun;
            runNHL.Checked = Options.settings.nhlRun;
            runNext.Checked = Options.settings.activeRun;
            runStandard.Checked = Options.settings.standardRun;
            runIterative.Checked = Options.settings.iterativeRun;
            runWeighted.Checked = Options.settings.weightedRun;

            if (nextRun == DateTime.MinValue)
                nextRunTime.Value = DateTime.Parse($"{DateTime.Today.ToShortDateString()} 12:00:00 PM");
            else
                nextRunTime.Value = nextRun;

            lastDate.Text = lastRun == DateTime.MinValue ? "----" : lastRun.ToString("MMM dd, yyyy");
            lastTime.Text = lastRun == DateTime.MinValue ? "----" : lastRun.ToString("hh:mm:ss tt");

            LoadNewButtons();

            loading = false;
        }

        public void LoadNewButtons()
        {
            var seasonStatus = Helpers.GetSeasonStatus();
            Runner.LoadNewButton(newMLB, "MLB", seasonStatus["MLB"]);
            Runner.LoadNewButton(newNBA, "NBA", seasonStatus["NBA"]);
            Runner.LoadNewButton(newNFL, "NFL", seasonStatus["NFL"]);
            Runner.LoadNewButton(newNHL, "NHL", seasonStatus["NHL"]);
        }

        private void clockTimer_Tick(object sender, EventArgs e)
        {
            currDate.Text = DateTime.Now.ToString("MMM dd, yyyy");
            currTime.Text = DateTime.Now.ToString("hh:mm:ss tt");

            // If we've passed time to run the next scheduled run
            if (runNext.Checked && DateTime.Now > nextRunTime.Value && Options.settings.lastRun < nextRunTime.Value)
            {
                Options.RunTriggeredAt(DateTime.Now);
                nextRunTime.Value = nextRunTime.Value.AddDays(1);

                runNow_Click(sender, e);

                //if (systemTray.Visible == true)
                //{
                //    systemTray.BalloonTipText = "Ran the thing";
                //    systemTray.ShowBalloonTip(1000);
                //}
            }
        }

        /// <summary>
        /// Triggered when the Update button on the score updater tab is clicked. Calls the Updater with desired parameters.
        /// </summary>
        private void butUpdate_Click(object sender, EventArgs e)
        {
            var leagues = new List<string>();
            string message = "Updating ";

            // Add the selected leagues
            if (mlbBox.Checked)
                leagues.Add(mlbBox.Text);
            if (nbaBox.Checked)
                leagues.Add(nbaBox.Text);
            if (nflBox.Checked)
                leagues.Add(nflBox.Text);
            if (nhlBox.Checked)
                leagues.Add(nhlBox.Text);

            if (leagues.Count == 0)
                Log("No leagues selected", LogLevel.warning);
            else
            {
                // Send message to log
                leagues.ForEach(l => message += $"{l}, ");
                message = $"{message.Substring(0, message.Length - 2)} for {(cbAllYears.Checked ? $"1970 - {DateTime.Now.Year}." : cbUYears.Text)}";
                Log(message);

                // Run from 1970 to current year if checked, otherwise only the selected year.
                if (cbAllYears.Checked)
                {
                    for (int i = FIRSTYEAR; i <= DateTime.Now.Year; i++)
                        Updater.Run(leagues, i.ToString());
                }
                else
                    Updater.Run(leagues, cbUYears.Text);
            }
        }

        /// <summary>
        /// Updates the rich text box with logging information
        /// </summary>
        public void Log(string text, LogLevel level = LogLevel.info)
        {
            tbOutput.AppendText(text + Environment.NewLine, level);
            Application.DoEvents(); // Forces textbox to update even when app is busy elsewhere
        }

        /// <summary>
        /// Populates all year dropdowns with all years from 1970 to the current year
        /// </summary>
        public void PopulateYears()
        {
            for (int i = DateTime.Now.Year; i >= FIRSTYEAR; i--)
            {
                cbUYears.Items.Add(i);
                ddlFirstYear.Items.Add(i);
                ddlLastYear.Items.Add(i);
            }
            cbUYears.SelectedIndex = 0;
            ddlFirstYear.SelectedIndex = 0;
            ddlLastYear.SelectedIndex = 0;
        }

        /// <summary>
        /// Confirmation box when user selects to update scores for all years
        /// </summary>
        private void cbAllYears_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAllYears.Checked)
            {
                DialogResult result = MessageBox.Show($"Processing all years from 1970 to {DateTime.Now.Year} will take a long time. Are you sure?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result.Equals(DialogResult.No))
                {
                    cbAllYears.Checked = false;
                }
                else
                {
                    cbUYears.Enabled = false;
                }
            }
            else
            {
                cbUYears.Enabled = true;
            }
        }

        /// <summary>
        /// Toggle the availability of the LastYear selection based on the radio selection
        /// </summary>
        private void seasMulti_CheckedChanged(object sender, EventArgs e)
        {
            if (seasMulti.Checked)
            {
                labelFirstYear.Text = "First Year";
                labelLastYear.Visible = true;
                ddlLastYear.Visible = true;
            }
            else
            {
                labelFirstYear.Text = "Year";
                labelLastYear.Visible = false;
                ddlLastYear.Visible = false;
            }
            rbSpecific_CheckedChanged(sender, e);
        }

        /// <summary>
        /// Only allow specific week selection if other options are single league and single season.
        /// </summary>
        private void rbSpecific_CheckedChanged(object sender, EventArgs e)
        {
            ddlWeeks.Items.Clear();
            if (rbSpecific.Checked)
            {
                if (buildLeagues.Count > 1 || seasMulti.Checked)
                {
                    MessageBox.Show($"When running multiple leagues or seasons only the most current, specific weeks are not available.", "Restricted", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    rbCurrentOnly.Checked = true;
                }
                else
                {
                    butLoadWeeks.Visible = true;
                    ddlWeeks.Visible = true;
                }
            }
            else
            {
                butLoadWeeks.Visible = false;
                ddlWeeks.Visible = false;
            }
        }

        /// <summary>
        /// Loads the appropriate weeks to the dropdown for a specific week build request
        /// </summary>
        private void butLoadWeeks_Click(object sender, EventArgs e)
        {
            if (buildLeagues.Count != 1)
            {
                MessageBox.Show($"Exactly one league must be selected to use the load weeks functionality.", "Invalid Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get the weeks and populate the dropdown
            var weeks = Helpers.GetWeeks(int.Parse(ddlFirstYear.SelectedItem.ToString()), buildLeagues[0]);
            ddlWeeks.Items.Clear();
            weeks.ForEach(week => ddlWeeks.Items.Add(week));

            if (ddlWeeks.Items.Count > 0)
                ddlWeeks.SelectedIndex = 0;
            else
                MessageBox.Show($"No data is available for the selected season.", "No Data Available", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Forces the specific check based on a change to the first year selection
        /// </summary>
        private void ddlFirstYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            rbSpecific_CheckedChanged(sender, e);
        }

        /// <summary>
        /// Initiates the Build process by taking the selected parameters, validating them, and sending to the Builder.
        /// </summary>
        private void butBuild_Click(object sender, EventArgs e)
        {
            if (buildLeagues.Count == 0)
                MessageBox.Show($"Please select at least one league.", "Invalid Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (buildMethods.Count == 0)
                MessageBox.Show($"Please select at least one method.", "Invalid Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (seasMulti.Checked && int.Parse(ddlFirstYear.SelectedItem.ToString()) > int.Parse(ddlLastYear.SelectedItem.ToString()))
                MessageBox.Show($"First year must be prior to the last year.", "Invalid Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                var seasons = new List<string>();
                int weeks;

                // Set the seasons to be used based on selections
                if (seasMulti.Checked)
                {
                    for (int i = int.Parse(ddlFirstYear.SelectedItem.ToString()); i <= int.Parse(ddlLastYear.SelectedItem.ToString()); i++)
                    {
                        seasons.Add(i.ToString());
                    }
                }
                else
                {
                    seasons.Add(ddlFirstYear.SelectedItem.ToString());
                }

                // Set week flag based on selection: -1 = Current Only, 0 = All Weeks Up To Current, # = Specific Week
                if (rbCurrentOnly.Checked)
                    weeks = -1;
                else if (rbUpToCurrent.Checked)
                    weeks = 0;
                else
                    weeks = int.Parse(ddlWeeks.SelectedItem.ToString());

                // Execute Builder
                Builder.Run(buildLeagues, seasons, buildMethods, weeks);
            }
        }

        #region Form Minimization to System Tray
        /// <summary>
        /// Pops up settings form when menu item selected
        /// </summary>
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frm = new SettingsForm();
            frm.Location = this.Location;
            frm.StartPosition = FormStartPosition.Manual;
            frm.Show();
        }

        /// <summary>
        /// If the user hits the X in the top right to close the app, minimize it to the system tray instead.
        /// </summary>
        private void MinimizeOnClose(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                SendToTray();
                e.Cancel = true;
            }
        }

        /// <summary>
        /// When the user minimizes the form, send it to the system tray.
        /// </summary>
        public void MinimizeForm(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                SendToTray();
            }
        }

        /// <summary>
        /// Send the application to the system tray
        /// </summary>
        private void SendToTray()
        {
            Hide();
            systemTray.Visible = true;
        }

        /// <summary>
        /// When the user double clicks on the item on the system tray, restore to normal
        /// </summary>
        public void systemTray_Click(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            systemTray.Visible = false;
        }
        #endregion

        #region Builder Checkboxes
        /// <summary>
        /// Track status of Standard Method build request
        /// </summary>
        private void methStandard_CheckedChanged(object sender, EventArgs e)
        {
            if (methStandard.Checked && !buildMethods.Contains(Method.Standard))
                buildMethods.Add(Method.Standard);
            if (!methStandard.Checked && buildMethods.Contains(Method.Standard))
                buildMethods.Remove(Method.Standard);
        }

        /// <summary>
        /// Track status of Iterative Method build request
        /// </summary>
        private void methIterative_CheckedChanged(object sender, EventArgs e)
        {
            if (methIterative.Checked && !buildMethods.Contains(Method.Iterative))
                buildMethods.Add(Method.Iterative);
            if (!methIterative.Checked && buildMethods.Contains(Method.Iterative))
                buildMethods.Remove(Method.Iterative);
        }

        /// <summary>
        /// Track status of Weighted Method build request
        /// </summary>
        private void methWeighted_CheckedChanged(object sender, EventArgs e)
        {
            if (methWeighted.Checked && !buildMethods.Contains(Method.Weighted))
                buildMethods.Add(Method.Weighted);
            if (!methWeighted.Checked && buildMethods.Contains(Method.Weighted))
                buildMethods.Remove(Method.Weighted);
        }

        /// <summary>
        /// Tracks status of MLB build request
        /// </summary>
        private void mlbBoxB_CheckedChanged(object sender, EventArgs e)
        {
            if (mlbBoxB.Checked && !buildLeagues.Contains("MLB"))
                buildLeagues.Add("MLB");
            if (!mlbBoxB.Checked && buildLeagues.Contains("MLB"))
                buildLeagues.Remove("MLB");

            rbSpecific_CheckedChanged(sender, e);
        }

        /// <summary>
        /// Tracks status of NBA build request
        /// </summary>
        private void nbaBoxB_CheckedChanged(object sender, EventArgs e)
        {
            if (nbaBoxB.Checked && !buildLeagues.Contains("NBA"))
                buildLeagues.Add("NBA");
            if (!nbaBoxB.Checked && buildLeagues.Contains("NBA"))
                buildLeagues.Remove("NBA");

            rbSpecific_CheckedChanged(sender, e);
        }

        /// <summary>
        /// Tracks status of NFL build request
        /// </summary>
        private void nflBoxB_CheckedChanged(object sender, EventArgs e)
        {
            if (nflBoxB.Checked && !buildLeagues.Contains("NFL"))
                buildLeagues.Add("NFL");
            if (!nflBoxB.Checked && buildLeagues.Contains("NFL"))
                buildLeagues.Remove("NFL");

            rbSpecific_CheckedChanged(sender, e);
        }

        /// <summary>
        /// Tracks status of NHL build request
        /// </summary>
        private void nhlBoxB_CheckedChanged(object sender, EventArgs e)
        {
            if (nhlBoxB.Checked && !buildLeagues.Contains("NHL"))
                buildLeagues.Add("NHL");
            if (!nhlBoxB.Checked && buildLeagues.Contains("NHL"))
                buildLeagues.Remove("NHL");

            rbSpecific_CheckedChanged(sender, e);
        }
        #endregion

        /// <summary>
        /// Since using X to close the app only minimizes it, users must use this to actually quit the program.
        /// </summary>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void schedule_CheckedChanged(object sender, EventArgs e)
        {
            if (!loading)
                Options.SaveSchedule(runMLB.Checked, runNBA.Checked, runNFL.Checked, runNHL.Checked, 
                    runStandard.Checked, runIterative.Checked, runWeighted.Checked, runNext.Checked, nextRunTime.Value);
        }

        private void runNow_Click(object sender, EventArgs e)
        {
            var leagues = new List<string>();
            var methods = new List<Method>();

            // Add the selected leagues
            if (runMLB.Checked)
                leagues.Add(runMLB.Text);
            if (runNBA.Checked)
                leagues.Add(runNBA.Text);
            if (runNFL.Checked)
                leagues.Add(runNFL.Text);
            if (runNHL.Checked)
                leagues.Add(runNHL.Text);

            // Add the selected methods
            if (runStandard.Checked)
                methods.Add(Method.Standard);
            if (runIterative.Checked)
                methods.Add(Method.Iterative);
            if (runWeighted.Checked)
                methods.Add(Method.Weighted);

            if (leagues.Count == 0 || methods.Count == 0)
            {
                Logger.Log("Could not initiate scheduled run due to unselected parameters.", LogLevel.error);
            }
            else
            {
                Runner.Run(leagues, methods);
                lastDate.Text = DateTime.Now.ToString("MMM dd, yyyy");
                lastTime.Text = DateTime.Now.ToString("hh:mm:ss tt");
                Options.RunTriggeredAt(DateTime.Now);
            }
        }

        private void NewSeason_Click(object sender, EventArgs e)
        {
            Helpers.InsertSeason(((Button)sender).Name.Substring(3, 3).ToUpper());
            LoadNewButtons();
        }

        private void Playoffs_Click(object sender, EventArgs e)
        {
            Runner.BuildPlayoffs(((Button)sender).Name.Substring(0, 3).ToUpper());
        }
    }
}
