using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using BeatGraphs.Modules;

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

        /// <summary>
        /// Constructor for the form, initializes.
        /// </summary>
        public BeatGraphForm()
        {
            InitializeComponent();
            PopulateYears();
            Settings.LoadSettings();
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
                        Updater.Run(leagues, i.ToString(), this);
                }
                else
                    Updater.Run(leagues, cbUYears.Text, this);
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
            else if (int.Parse(ddlFirstYear.SelectedItem.ToString()) > int.Parse(ddlLastYear.SelectedItem.ToString()))
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
                Builder.Run(buildLeagues, seasons, buildMethods, weeks, this);
            }
        }

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
        /// Pops up settings form when menu item selected
        /// </summary>
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frm = new SettingsForm();
            frm.Location = this.Location;
            frm.StartPosition = FormStartPosition.Manual;
            frm.Show();
        }
    }
}
