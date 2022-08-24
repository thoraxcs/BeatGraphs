namespace BeatGraphs
{
    partial class BeatGraphForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BeatGraphForm));
            this.tbOutput = new System.Windows.Forms.RichTextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabBuilder = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.mlbBoxB = new System.Windows.Forms.CheckBox();
            this.nbaBoxB = new System.Windows.Forms.CheckBox();
            this.nflBoxB = new System.Windows.Forms.CheckBox();
            this.nhlBoxB = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.methStandard = new System.Windows.Forms.CheckBox();
            this.methIterative = new System.Windows.Forms.CheckBox();
            this.methWeighted = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbSpecific = new System.Windows.Forms.RadioButton();
            this.ddlWeeks = new System.Windows.Forms.ComboBox();
            this.rbUpToCurrent = new System.Windows.Forms.RadioButton();
            this.butLoadWeeks = new System.Windows.Forms.Button();
            this.rbCurrentOnly = new System.Windows.Forms.RadioButton();
            this.butBuild = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ddlLastYear = new System.Windows.Forms.ComboBox();
            this.ddlFirstYear = new System.Windows.Forms.ComboBox();
            this.labelLastYear = new System.Windows.Forms.Label();
            this.labelFirstYear = new System.Windows.Forms.Label();
            this.seasMulti = new System.Windows.Forms.RadioButton();
            this.seasSingle = new System.Windows.Forms.RadioButton();
            this.moduleSelect = new System.Windows.Forms.TabControl();
            this.tabUpdater = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.mlbBox = new System.Windows.Forms.CheckBox();
            this.nbaBox = new System.Windows.Forms.CheckBox();
            this.nflBox = new System.Windows.Forms.CheckBox();
            this.nhlBox = new System.Windows.Forms.CheckBox();
            this.cbAllYears = new System.Windows.Forms.CheckBox();
            this.butUpdate = new System.Windows.Forms.Button();
            this.cbUYears = new System.Windows.Forms.ComboBox();
            this.menuStrip1.SuspendLayout();
            this.tabBuilder.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.moduleSelect.SuspendLayout();
            this.tabUpdater.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbOutput
            // 
            this.tbOutput.BackColor = System.Drawing.SystemColors.WindowText;
            this.tbOutput.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.tbOutput.Location = new System.Drawing.Point(12, 218);
            this.tbOutput.Name = "tbOutput";
            this.tbOutput.Size = new System.Drawing.Size(602, 261);
            this.tbOutput.TabIndex = 1;
            this.tbOutput.Text = "";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(626, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // tabBuilder
            // 
            this.tabBuilder.Controls.Add(this.groupBox4);
            this.tabBuilder.Controls.Add(this.groupBox3);
            this.tabBuilder.Controls.Add(this.groupBox2);
            this.tabBuilder.Controls.Add(this.butBuild);
            this.tabBuilder.Controls.Add(this.groupBox1);
            this.tabBuilder.Location = new System.Drawing.Point(4, 22);
            this.tabBuilder.Name = "tabBuilder";
            this.tabBuilder.Padding = new System.Windows.Forms.Padding(3);
            this.tabBuilder.Size = new System.Drawing.Size(596, 159);
            this.tabBuilder.TabIndex = 1;
            this.tabBuilder.Text = "Build Graphs";
            this.tabBuilder.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.mlbBoxB);
            this.groupBox4.Controls.Add(this.nbaBoxB);
            this.groupBox4.Controls.Add(this.nflBoxB);
            this.groupBox4.Controls.Add(this.nhlBoxB);
            this.groupBox4.Location = new System.Drawing.Point(10, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(112, 115);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Leagues";
            // 
            // mlbBoxB
            // 
            this.mlbBoxB.AutoSize = true;
            this.mlbBoxB.Location = new System.Drawing.Point(6, 19);
            this.mlbBoxB.Name = "mlbBoxB";
            this.mlbBoxB.Size = new System.Drawing.Size(48, 17);
            this.mlbBoxB.TabIndex = 0;
            this.mlbBoxB.Text = "MLB";
            this.mlbBoxB.UseVisualStyleBackColor = true;
            this.mlbBoxB.CheckedChanged += new System.EventHandler(this.mlbBoxB_CheckedChanged);
            // 
            // nbaBoxB
            // 
            this.nbaBoxB.AutoSize = true;
            this.nbaBoxB.Location = new System.Drawing.Point(6, 42);
            this.nbaBoxB.Name = "nbaBoxB";
            this.nbaBoxB.Size = new System.Drawing.Size(48, 17);
            this.nbaBoxB.TabIndex = 1;
            this.nbaBoxB.Text = "NBA";
            this.nbaBoxB.UseVisualStyleBackColor = true;
            this.nbaBoxB.CheckedChanged += new System.EventHandler(this.nbaBoxB_CheckedChanged);
            // 
            // nflBoxB
            // 
            this.nflBoxB.AutoSize = true;
            this.nflBoxB.Location = new System.Drawing.Point(6, 65);
            this.nflBoxB.Name = "nflBoxB";
            this.nflBoxB.Size = new System.Drawing.Size(46, 17);
            this.nflBoxB.TabIndex = 2;
            this.nflBoxB.Text = "NFL";
            this.nflBoxB.UseVisualStyleBackColor = true;
            this.nflBoxB.CheckedChanged += new System.EventHandler(this.nflBoxB_CheckedChanged);
            // 
            // nhlBoxB
            // 
            this.nhlBoxB.AutoSize = true;
            this.nhlBoxB.Location = new System.Drawing.Point(6, 88);
            this.nhlBoxB.Name = "nhlBoxB";
            this.nhlBoxB.Size = new System.Drawing.Size(48, 17);
            this.nhlBoxB.TabIndex = 3;
            this.nhlBoxB.Text = "NHL";
            this.nhlBoxB.UseVisualStyleBackColor = true;
            this.nhlBoxB.CheckedChanged += new System.EventHandler(this.nhlBoxB_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.methStandard);
            this.groupBox3.Controls.Add(this.methIterative);
            this.groupBox3.Controls.Add(this.methWeighted);
            this.groupBox3.Location = new System.Drawing.Point(274, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(136, 115);
            this.groupBox3.TabIndex = 20;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Methods";
            // 
            // methStandard
            // 
            this.methStandard.AutoSize = true;
            this.methStandard.Location = new System.Drawing.Point(6, 19);
            this.methStandard.Name = "methStandard";
            this.methStandard.Size = new System.Drawing.Size(69, 17);
            this.methStandard.TabIndex = 7;
            this.methStandard.Text = "Standard";
            this.methStandard.UseVisualStyleBackColor = true;
            this.methStandard.CheckedChanged += new System.EventHandler(this.methStandard_CheckedChanged);
            // 
            // methIterative
            // 
            this.methIterative.AutoSize = true;
            this.methIterative.Location = new System.Drawing.Point(6, 42);
            this.methIterative.Name = "methIterative";
            this.methIterative.Size = new System.Drawing.Size(64, 17);
            this.methIterative.TabIndex = 8;
            this.methIterative.Text = "Iterative";
            this.methIterative.UseVisualStyleBackColor = true;
            this.methIterative.CheckedChanged += new System.EventHandler(this.methIterative_CheckedChanged);
            // 
            // methWeighted
            // 
            this.methWeighted.AutoSize = true;
            this.methWeighted.Location = new System.Drawing.Point(6, 65);
            this.methWeighted.Name = "methWeighted";
            this.methWeighted.Size = new System.Drawing.Size(72, 17);
            this.methWeighted.TabIndex = 9;
            this.methWeighted.Text = "Weighted";
            this.methWeighted.UseVisualStyleBackColor = true;
            this.methWeighted.CheckedChanged += new System.EventHandler(this.methWeighted_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbSpecific);
            this.groupBox2.Controls.Add(this.ddlWeeks);
            this.groupBox2.Controls.Add(this.rbUpToCurrent);
            this.groupBox2.Controls.Add(this.butLoadWeeks);
            this.groupBox2.Controls.Add(this.rbCurrentOnly);
            this.groupBox2.Location = new System.Drawing.Point(416, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(170, 115);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Weeks";
            // 
            // rbSpecific
            // 
            this.rbSpecific.AutoSize = true;
            this.rbSpecific.Location = new System.Drawing.Point(6, 65);
            this.rbSpecific.Name = "rbSpecific";
            this.rbSpecific.Size = new System.Drawing.Size(119, 17);
            this.rbSpecific.TabIndex = 2;
            this.rbSpecific.Text = "Specific Week Only";
            this.rbSpecific.UseVisualStyleBackColor = true;
            this.rbSpecific.CheckedChanged += new System.EventHandler(this.rbSpecific_CheckedChanged);
            // 
            // ddlWeeks
            // 
            this.ddlWeeks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlWeeks.FormattingEnabled = true;
            this.ddlWeeks.Location = new System.Drawing.Point(87, 86);
            this.ddlWeeks.Name = "ddlWeeks";
            this.ddlWeeks.Size = new System.Drawing.Size(75, 21);
            this.ddlWeeks.TabIndex = 18;
            this.ddlWeeks.Visible = false;
            // 
            // rbUpToCurrent
            // 
            this.rbUpToCurrent.AutoSize = true;
            this.rbUpToCurrent.Location = new System.Drawing.Point(6, 42);
            this.rbUpToCurrent.Name = "rbUpToCurrent";
            this.rbUpToCurrent.Size = new System.Drawing.Size(132, 17);
            this.rbUpToCurrent.TabIndex = 1;
            this.rbUpToCurrent.Text = "All Up To Most Current";
            this.rbUpToCurrent.UseVisualStyleBackColor = true;
            // 
            // butLoadWeeks
            // 
            this.butLoadWeeks.Location = new System.Drawing.Point(6, 85);
            this.butLoadWeeks.Name = "butLoadWeeks";
            this.butLoadWeeks.Size = new System.Drawing.Size(75, 23);
            this.butLoadWeeks.TabIndex = 17;
            this.butLoadWeeks.Text = "Load";
            this.butLoadWeeks.UseVisualStyleBackColor = true;
            this.butLoadWeeks.Visible = false;
            this.butLoadWeeks.Click += new System.EventHandler(this.butLoadWeeks_Click);
            // 
            // rbCurrentOnly
            // 
            this.rbCurrentOnly.AutoSize = true;
            this.rbCurrentOnly.Checked = true;
            this.rbCurrentOnly.Location = new System.Drawing.Point(6, 19);
            this.rbCurrentOnly.Name = "rbCurrentOnly";
            this.rbCurrentOnly.Size = new System.Drawing.Size(109, 17);
            this.rbCurrentOnly.TabIndex = 0;
            this.rbCurrentOnly.TabStop = true;
            this.rbCurrentOnly.Text = "Most Current Only";
            this.rbCurrentOnly.UseVisualStyleBackColor = true;
            // 
            // butBuild
            // 
            this.butBuild.Location = new System.Drawing.Point(10, 127);
            this.butBuild.Name = "butBuild";
            this.butBuild.Size = new System.Drawing.Size(75, 23);
            this.butBuild.TabIndex = 12;
            this.butBuild.Text = "Build";
            this.butBuild.UseVisualStyleBackColor = true;
            this.butBuild.Click += new System.EventHandler(this.butBuild_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ddlLastYear);
            this.groupBox1.Controls.Add(this.ddlFirstYear);
            this.groupBox1.Controls.Add(this.labelLastYear);
            this.groupBox1.Controls.Add(this.labelFirstYear);
            this.groupBox1.Controls.Add(this.seasMulti);
            this.groupBox1.Controls.Add(this.seasSingle);
            this.groupBox1.Location = new System.Drawing.Point(128, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(140, 115);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Seasons";
            // 
            // ddlLastYear
            // 
            this.ddlLastYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlLastYear.FormattingEnabled = true;
            this.ddlLastYear.Location = new System.Drawing.Point(7, 76);
            this.ddlLastYear.Name = "ddlLastYear";
            this.ddlLastYear.Size = new System.Drawing.Size(59, 21);
            this.ddlLastYear.TabIndex = 5;
            this.ddlLastYear.Visible = false;
            // 
            // ddlFirstYear
            // 
            this.ddlFirstYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlFirstYear.FormattingEnabled = true;
            this.ddlFirstYear.Location = new System.Drawing.Point(7, 47);
            this.ddlFirstYear.Name = "ddlFirstYear";
            this.ddlFirstYear.Size = new System.Drawing.Size(59, 21);
            this.ddlFirstYear.TabIndex = 4;
            this.ddlFirstYear.SelectedIndexChanged += new System.EventHandler(this.ddlFirstYear_SelectedIndexChanged);
            // 
            // labelLastYear
            // 
            this.labelLastYear.AutoSize = true;
            this.labelLastYear.Location = new System.Drawing.Point(72, 79);
            this.labelLastYear.Name = "labelLastYear";
            this.labelLastYear.Size = new System.Drawing.Size(52, 13);
            this.labelLastYear.TabIndex = 3;
            this.labelLastYear.Text = "Last Year";
            this.labelLastYear.Visible = false;
            // 
            // labelFirstYear
            // 
            this.labelFirstYear.AutoSize = true;
            this.labelFirstYear.Location = new System.Drawing.Point(72, 50);
            this.labelFirstYear.Name = "labelFirstYear";
            this.labelFirstYear.Size = new System.Drawing.Size(29, 13);
            this.labelFirstYear.TabIndex = 2;
            this.labelFirstYear.Text = "Year";
            // 
            // seasMulti
            // 
            this.seasMulti.AutoSize = true;
            this.seasMulti.Location = new System.Drawing.Point(75, 20);
            this.seasMulti.Name = "seasMulti";
            this.seasMulti.Size = new System.Drawing.Size(61, 17);
            this.seasMulti.TabIndex = 1;
            this.seasMulti.Text = "Multiple";
            this.seasMulti.UseVisualStyleBackColor = true;
            this.seasMulti.CheckedChanged += new System.EventHandler(this.seasMulti_CheckedChanged);
            // 
            // seasSingle
            // 
            this.seasSingle.AutoSize = true;
            this.seasSingle.Checked = true;
            this.seasSingle.Location = new System.Drawing.Point(7, 20);
            this.seasSingle.Name = "seasSingle";
            this.seasSingle.Size = new System.Drawing.Size(54, 17);
            this.seasSingle.TabIndex = 0;
            this.seasSingle.TabStop = true;
            this.seasSingle.Text = "Single";
            this.seasSingle.UseVisualStyleBackColor = true;
            // 
            // moduleSelect
            // 
            this.moduleSelect.Controls.Add(this.tabUpdater);
            this.moduleSelect.Controls.Add(this.tabBuilder);
            this.moduleSelect.Location = new System.Drawing.Point(12, 27);
            this.moduleSelect.Name = "moduleSelect";
            this.moduleSelect.SelectedIndex = 0;
            this.moduleSelect.Size = new System.Drawing.Size(604, 185);
            this.moduleSelect.TabIndex = 0;
            // 
            // tabUpdater
            // 
            this.tabUpdater.Controls.Add(this.groupBox5);
            this.tabUpdater.Controls.Add(this.cbAllYears);
            this.tabUpdater.Controls.Add(this.butUpdate);
            this.tabUpdater.Controls.Add(this.cbUYears);
            this.tabUpdater.Location = new System.Drawing.Point(4, 22);
            this.tabUpdater.Name = "tabUpdater";
            this.tabUpdater.Padding = new System.Windows.Forms.Padding(3);
            this.tabUpdater.Size = new System.Drawing.Size(768, 159);
            this.tabUpdater.TabIndex = 0;
            this.tabUpdater.Text = "Update Scores";
            this.tabUpdater.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.mlbBox);
            this.groupBox5.Controls.Add(this.nbaBox);
            this.groupBox5.Controls.Add(this.nflBox);
            this.groupBox5.Controls.Add(this.nhlBox);
            this.groupBox5.Location = new System.Drawing.Point(10, 6);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(140, 115);
            this.groupBox5.TabIndex = 7;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Leagues";
            // 
            // mlbBox
            // 
            this.mlbBox.AutoSize = true;
            this.mlbBox.Location = new System.Drawing.Point(6, 19);
            this.mlbBox.Name = "mlbBox";
            this.mlbBox.Size = new System.Drawing.Size(48, 17);
            this.mlbBox.TabIndex = 4;
            this.mlbBox.Text = "MLB";
            this.mlbBox.UseVisualStyleBackColor = true;
            // 
            // nbaBox
            // 
            this.nbaBox.AutoSize = true;
            this.nbaBox.Location = new System.Drawing.Point(6, 42);
            this.nbaBox.Name = "nbaBox";
            this.nbaBox.Size = new System.Drawing.Size(48, 17);
            this.nbaBox.TabIndex = 5;
            this.nbaBox.Text = "NBA";
            this.nbaBox.UseVisualStyleBackColor = true;
            // 
            // nflBox
            // 
            this.nflBox.AutoSize = true;
            this.nflBox.Location = new System.Drawing.Point(6, 65);
            this.nflBox.Name = "nflBox";
            this.nflBox.Size = new System.Drawing.Size(46, 17);
            this.nflBox.TabIndex = 6;
            this.nflBox.Text = "NFL";
            this.nflBox.UseVisualStyleBackColor = true;
            // 
            // nhlBox
            // 
            this.nhlBox.AutoSize = true;
            this.nhlBox.Location = new System.Drawing.Point(6, 88);
            this.nhlBox.Name = "nhlBox";
            this.nhlBox.Size = new System.Drawing.Size(48, 17);
            this.nhlBox.TabIndex = 7;
            this.nhlBox.Text = "NHL";
            this.nhlBox.UseVisualStyleBackColor = true;
            // 
            // cbAllYears
            // 
            this.cbAllYears.AutoSize = true;
            this.cbAllYears.Location = new System.Drawing.Point(156, 130);
            this.cbAllYears.Name = "cbAllYears";
            this.cbAllYears.Size = new System.Drawing.Size(134, 17);
            this.cbAllYears.TabIndex = 6;
            this.cbAllYears.Text = "Process 1970 - Current";
            this.cbAllYears.UseVisualStyleBackColor = true;
            this.cbAllYears.CheckedChanged += new System.EventHandler(this.cbAllYears_CheckedChanged);
            // 
            // butUpdate
            // 
            this.butUpdate.Location = new System.Drawing.Point(10, 127);
            this.butUpdate.Name = "butUpdate";
            this.butUpdate.Size = new System.Drawing.Size(75, 23);
            this.butUpdate.TabIndex = 5;
            this.butUpdate.Text = "Update";
            this.butUpdate.UseVisualStyleBackColor = true;
            this.butUpdate.Click += new System.EventHandler(this.butUpdate_Click);
            // 
            // cbUYears
            // 
            this.cbUYears.BackColor = System.Drawing.Color.White;
            this.cbUYears.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbUYears.FormattingEnabled = true;
            this.cbUYears.Location = new System.Drawing.Point(91, 128);
            this.cbUYears.Name = "cbUYears";
            this.cbUYears.Size = new System.Drawing.Size(59, 21);
            this.cbUYears.TabIndex = 4;
            // 
            // BeatGraphForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(626, 491);
            this.Controls.Add(this.tbOutput);
            this.Controls.Add(this.moduleSelect);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BeatGraphForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BeatGraphs";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabBuilder.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.moduleSelect.ResumeLayout(false);
            this.tabUpdater.ResumeLayout(false);
            this.tabUpdater.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RichTextBox tbOutput;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.TabPage tabBuilder;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox mlbBoxB;
        private System.Windows.Forms.CheckBox nbaBoxB;
        private System.Windows.Forms.CheckBox nflBoxB;
        private System.Windows.Forms.CheckBox nhlBoxB;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox methStandard;
        private System.Windows.Forms.CheckBox methIterative;
        private System.Windows.Forms.CheckBox methWeighted;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbSpecific;
        private System.Windows.Forms.ComboBox ddlWeeks;
        private System.Windows.Forms.RadioButton rbUpToCurrent;
        private System.Windows.Forms.Button butLoadWeeks;
        private System.Windows.Forms.RadioButton rbCurrentOnly;
        private System.Windows.Forms.Button butBuild;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox ddlLastYear;
        private System.Windows.Forms.ComboBox ddlFirstYear;
        private System.Windows.Forms.Label labelLastYear;
        private System.Windows.Forms.Label labelFirstYear;
        private System.Windows.Forms.RadioButton seasMulti;
        private System.Windows.Forms.RadioButton seasSingle;
        private System.Windows.Forms.TabControl moduleSelect;
        private System.Windows.Forms.TabPage tabUpdater;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox cbAllYears;
        private System.Windows.Forms.Button butUpdate;
        private System.Windows.Forms.ComboBox cbUYears;
        private System.Windows.Forms.CheckBox mlbBox;
        private System.Windows.Forms.CheckBox nbaBox;
        private System.Windows.Forms.CheckBox nflBox;
        private System.Windows.Forms.CheckBox nhlBox;
    }
}

