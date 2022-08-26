using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Options = BeatGraphs.SettingsRecord.Settings;

namespace BeatGraphs
{
    /// <summary>
    /// Form for keeping track of user settings.
    /// </summary>
    public partial class SettingsForm : Form
    {
        /// <summary>
        /// Set initial states to defaults from respective areas
        /// </summary>
        public SettingsForm()
        {
            InitializeComponent();
            setVerbose.Checked = Options.settings.verbose;
            setUpload.Checked = Options.settings.upload;
        }

        /// <summary>
        /// Update settings
        /// </summary>
        private void butSave_Click(object sender, EventArgs e)
        {
            Options.SaveSettings(setVerbose.Checked, setUpload.Checked);
            Close();
        }

        /// <summary>
        /// Ignore changes
        /// </summary>
        private void butCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
