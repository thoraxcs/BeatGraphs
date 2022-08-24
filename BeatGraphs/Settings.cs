using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BeatGraphs
{
    /// <summary>
    /// Form for keeping track of user settings.
    /// </summary>
    public partial class Settings : Form
    {
        /// <summary>
        /// Set initial states to defaults from respective areas
        /// </summary>
        public Settings()
        {
            InitializeComponent();
            setVerbose.Checked = RichTextBoxExtensions.verbose;
            setUpload.Checked = Modules.Writer.ftpEnabled;
        }

        /// <summary>
        /// Update settings
        /// </summary>
        private void butSave_Click(object sender, EventArgs e)
        {
            RichTextBoxExtensions.verbose = setVerbose.Checked;
            Modules.Writer.ftpEnabled = setUpload.Checked;
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
