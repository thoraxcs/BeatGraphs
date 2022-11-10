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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BeatGraphForm));
            this.tbOutput = new System.Windows.Forms.RichTextBox();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.RunPlayoffs = new System.Windows.Forms.GroupBox();
            this.nhlPlayoffs = new System.Windows.Forms.Button();
            this.nflPlayoffs = new System.Windows.Forms.Button();
            this.nbaPlayoffs = new System.Windows.Forms.Button();
            this.mlbPlayoffs = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.mlbBox = new System.Windows.Forms.CheckBox();
            this.nbaBox = new System.Windows.Forms.CheckBox();
            this.nflBox = new System.Windows.Forms.CheckBox();
            this.nhlBox = new System.Windows.Forms.CheckBox();
            this.cbAllYears = new System.Windows.Forms.CheckBox();
            this.butUpdate = new System.Windows.Forms.Button();
            this.cbUYears = new System.Windows.Forms.ComboBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.newNHL = new System.Windows.Forms.Button();
            this.newNFL = new System.Windows.Forms.Button();
            this.newNBA = new System.Windows.Forms.Button();
            this.newMLB = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.clockLabel = new System.Windows.Forms.Label();
            this.currTime = new System.Windows.Forms.Label();
            this.currDate = new System.Windows.Forms.Label();
            this.lastDate = new System.Windows.Forms.Label();
            this.lastRun = new System.Windows.Forms.Label();
            this.lastTime = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.runNow = new System.Windows.Forms.Button();
            this.nextRunTime = new System.Windows.Forms.DateTimePicker();
            this.runNext = new System.Windows.Forms.CheckBox();
            this.runStandard = new System.Windows.Forms.CheckBox();
            this.runIterative = new System.Windows.Forms.CheckBox();
            this.runMLB = new System.Windows.Forms.CheckBox();
            this.runWeighted = new System.Windows.Forms.CheckBox();
            this.runNBA = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.runNFL = new System.Windows.Forms.CheckBox();
            this.runNHL = new System.Windows.Forms.CheckBox();
            this.systemTray = new System.Windows.Forms.NotifyIcon(this.components);
            this.clockTimer = new System.Windows.Forms.Timer(this.components);
            this.loadBar = new System.Windows.Forms.ProgressBar();
            this.menuStrip.SuspendLayout();
            this.tabBuilder.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.moduleSelect.SuspendLayout();
            this.tabUpdater.SuspendLayout();
            this.RunPlayoffs.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbOutput
            // 
            this.tbOutput.BackColor = System.Drawing.SystemColors.WindowText;
            this.tbOutput.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.tbOutput.Location = new System.Drawing.Point(12, 218);
            this.tbOutput.Name = "tbOutput";
            this.tbOutput.Size = new System.Drawing.Size(602, 232);
            this.tbOutput.TabIndex = 1;
            this.tbOutput.Text = "";
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(626, 24);
            this.menuStrip.TabIndex = 2;
            this.menuStrip.Text = "menuStrip1";
            // 
            // menuToolStripMenuItem
            // 
            this.menuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            this.menuToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.menuToolStripMenuItem.Text = "Menu";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
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
            this.moduleSelect.Controls.Add(this.tabPage1);
            this.moduleSelect.Location = new System.Drawing.Point(12, 27);
            this.moduleSelect.Name = "moduleSelect";
            this.moduleSelect.SelectedIndex = 0;
            this.moduleSelect.Size = new System.Drawing.Size(604, 185);
            this.moduleSelect.TabIndex = 0;
            // 
            // tabUpdater
            // 
            this.tabUpdater.Controls.Add(this.RunPlayoffs);
            this.tabUpdater.Controls.Add(this.groupBox5);
            this.tabUpdater.Controls.Add(this.cbAllYears);
            this.tabUpdater.Controls.Add(this.butUpdate);
            this.tabUpdater.Controls.Add(this.cbUYears);
            this.tabUpdater.Location = new System.Drawing.Point(4, 22);
            this.tabUpdater.Name = "tabUpdater";
            this.tabUpdater.Padding = new System.Windows.Forms.Padding(3);
            this.tabUpdater.Size = new System.Drawing.Size(596, 159);
            this.tabUpdater.TabIndex = 0;
            this.tabUpdater.Text = "Update Scores";
            this.tabUpdater.UseVisualStyleBackColor = true;
            // 
            // RunPlayoffs
            // 
            this.RunPlayoffs.Controls.Add(this.nhlPlayoffs);
            this.RunPlayoffs.Controls.Add(this.nflPlayoffs);
            this.RunPlayoffs.Controls.Add(this.nbaPlayoffs);
            this.RunPlayoffs.Controls.Add(this.mlbPlayoffs);
            this.RunPlayoffs.Location = new System.Drawing.Point(390, 6);
            this.RunPlayoffs.Name = "RunPlayoffs";
            this.RunPlayoffs.Size = new System.Drawing.Size(196, 144);
            this.RunPlayoffs.TabIndex = 8;
            this.RunPlayoffs.TabStop = false;
            this.RunPlayoffs.Text = "Run Playoffs";
            // 
            // nhlPlayoffs
            // 
            this.nhlPlayoffs.Location = new System.Drawing.Point(6, 106);
            this.nhlPlayoffs.Name = "nhlPlayoffs";
            this.nhlPlayoffs.Size = new System.Drawing.Size(184, 23);
            this.nhlPlayoffs.TabIndex = 3;
            this.nhlPlayoffs.Text = "Generate NHL Playoffs";
            this.nhlPlayoffs.UseVisualStyleBackColor = true;
            this.nhlPlayoffs.Click += new System.EventHandler(this.Playoffs_Click);
            // 
            // nflPlayoffs
            // 
            this.nflPlayoffs.Location = new System.Drawing.Point(6, 77);
            this.nflPlayoffs.Name = "nflPlayoffs";
            this.nflPlayoffs.Size = new System.Drawing.Size(184, 23);
            this.nflPlayoffs.TabIndex = 2;
            this.nflPlayoffs.Text = "Generate NFL Playoffs";
            this.nflPlayoffs.UseVisualStyleBackColor = true;
            this.nflPlayoffs.Click += new System.EventHandler(this.Playoffs_Click);
            // 
            // nbaPlayoffs
            // 
            this.nbaPlayoffs.Location = new System.Drawing.Point(6, 48);
            this.nbaPlayoffs.Name = "nbaPlayoffs";
            this.nbaPlayoffs.Size = new System.Drawing.Size(184, 23);
            this.nbaPlayoffs.TabIndex = 1;
            this.nbaPlayoffs.Text = "Generate NBA Playoffs";
            this.nbaPlayoffs.UseVisualStyleBackColor = true;
            this.nbaPlayoffs.Click += new System.EventHandler(this.Playoffs_Click);
            // 
            // mlbPlayoffs
            // 
            this.mlbPlayoffs.Location = new System.Drawing.Point(6, 19);
            this.mlbPlayoffs.Name = "mlbPlayoffs";
            this.mlbPlayoffs.Size = new System.Drawing.Size(184, 23);
            this.mlbPlayoffs.TabIndex = 0;
            this.mlbPlayoffs.Text = "Generate MLB Playoffs";
            this.mlbPlayoffs.UseVisualStyleBackColor = true;
            this.mlbPlayoffs.Click += new System.EventHandler(this.Playoffs_Click);
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
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.newNHL);
            this.tabPage1.Controls.Add(this.newNFL);
            this.tabPage1.Controls.Add(this.newNBA);
            this.tabPage1.Controls.Add(this.newMLB);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Controls.Add(this.groupBox6);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(596, 159);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Schedule Runs";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // newNHL
            // 
            this.newNHL.Location = new System.Drawing.Point(430, 112);
            this.newNHL.Name = "newNHL";
            this.newNHL.Size = new System.Drawing.Size(149, 23);
            this.newNHL.TabIndex = 13;
            this.newNHL.Text = "Insert NHL Season";
            this.newNHL.UseVisualStyleBackColor = true;
            this.newNHL.Click += new System.EventHandler(this.NewSeason_Click);
            // 
            // newNFL
            // 
            this.newNFL.Location = new System.Drawing.Point(430, 83);
            this.newNFL.Name = "newNFL";
            this.newNFL.Size = new System.Drawing.Size(149, 23);
            this.newNFL.TabIndex = 12;
            this.newNFL.Text = "Insert NFL Season";
            this.newNFL.UseVisualStyleBackColor = true;
            this.newNFL.Click += new System.EventHandler(this.NewSeason_Click);
            // 
            // newNBA
            // 
            this.newNBA.Location = new System.Drawing.Point(430, 54);
            this.newNBA.Name = "newNBA";
            this.newNBA.Size = new System.Drawing.Size(149, 23);
            this.newNBA.TabIndex = 11;
            this.newNBA.Text = "Insert NBA Season";
            this.newNBA.UseVisualStyleBackColor = true;
            this.newNBA.Click += new System.EventHandler(this.NewSeason_Click);
            // 
            // newMLB
            // 
            this.newMLB.Location = new System.Drawing.Point(430, 25);
            this.newMLB.Name = "newMLB";
            this.newMLB.Size = new System.Drawing.Size(149, 23);
            this.newMLB.TabIndex = 10;
            this.newMLB.Text = "Insert MLB Season";
            this.newMLB.UseVisualStyleBackColor = true;
            this.newMLB.Click += new System.EventHandler(this.NewSeason_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.clockLabel);
            this.panel1.Controls.Add(this.currTime);
            this.panel1.Controls.Add(this.currDate);
            this.panel1.Controls.Add(this.lastDate);
            this.panel1.Controls.Add(this.lastRun);
            this.panel1.Controls.Add(this.lastTime);
            this.panel1.Location = new System.Drawing.Point(212, 11);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 142);
            this.panel1.TabIndex = 9;
            // 
            // clockLabel
            // 
            this.clockLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clockLabel.Location = new System.Drawing.Point(3, 0);
            this.clockLabel.Name = "clockLabel";
            this.clockLabel.Size = new System.Drawing.Size(194, 18);
            this.clockLabel.TabIndex = 0;
            this.clockLabel.Text = "Current Time";
            this.clockLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // currTime
            // 
            this.currTime.Location = new System.Drawing.Point(3, 40);
            this.currTime.Name = "currTime";
            this.currTime.Size = new System.Drawing.Size(194, 23);
            this.currTime.TabIndex = 8;
            this.currTime.Text = "current time";
            this.currTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // currDate
            // 
            this.currDate.Location = new System.Drawing.Point(3, 20);
            this.currDate.Name = "currDate";
            this.currDate.Size = new System.Drawing.Size(194, 23);
            this.currDate.TabIndex = 5;
            this.currDate.Text = "current date";
            this.currDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lastDate
            // 
            this.lastDate.Location = new System.Drawing.Point(3, 84);
            this.lastDate.Name = "lastDate";
            this.lastDate.Size = new System.Drawing.Size(194, 23);
            this.lastDate.TabIndex = 6;
            this.lastDate.Text = "last date";
            this.lastDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lastRun
            // 
            this.lastRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lastRun.Location = new System.Drawing.Point(3, 64);
            this.lastRun.Name = "lastRun";
            this.lastRun.Size = new System.Drawing.Size(194, 17);
            this.lastRun.TabIndex = 3;
            this.lastRun.Text = "Last Run";
            this.lastRun.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lastTime
            // 
            this.lastTime.Location = new System.Drawing.Point(3, 104);
            this.lastTime.Name = "lastTime";
            this.lastTime.Size = new System.Drawing.Size(194, 23);
            this.lastTime.TabIndex = 7;
            this.lastTime.Text = "last time";
            this.lastTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.runNow);
            this.groupBox6.Controls.Add(this.nextRunTime);
            this.groupBox6.Controls.Add(this.runNext);
            this.groupBox6.Controls.Add(this.runStandard);
            this.groupBox6.Controls.Add(this.runIterative);
            this.groupBox6.Controls.Add(this.runMLB);
            this.groupBox6.Controls.Add(this.runWeighted);
            this.groupBox6.Controls.Add(this.runNBA);
            this.groupBox6.Controls.Add(this.label1);
            this.groupBox6.Controls.Add(this.runNFL);
            this.groupBox6.Controls.Add(this.runNHL);
            this.groupBox6.Location = new System.Drawing.Point(6, 6);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(200, 147);
            this.groupBox6.TabIndex = 2;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Repeating Schedule";
            // 
            // runNow
            // 
            this.runNow.Location = new System.Drawing.Point(90, 113);
            this.runNow.Name = "runNow";
            this.runNow.Size = new System.Drawing.Size(96, 23);
            this.runNow.TabIndex = 10;
            this.runNow.Text = "Run Now";
            this.runNow.UseVisualStyleBackColor = true;
            this.runNow.Click += new System.EventHandler(this.runNow_Click);
            // 
            // nextRunTime
            // 
            this.nextRunTime.CustomFormat = "hh:mm tt";
            this.nextRunTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.nextRunTime.Location = new System.Drawing.Point(90, 19);
            this.nextRunTime.Name = "nextRunTime";
            this.nextRunTime.ShowUpDown = true;
            this.nextRunTime.Size = new System.Drawing.Size(96, 20);
            this.nextRunTime.TabIndex = 10;
            this.nextRunTime.ValueChanged += new System.EventHandler(this.schedule_CheckedChanged);
            // 
            // runNext
            // 
            this.runNext.AutoSize = true;
            this.runNext.Location = new System.Drawing.Point(67, 22);
            this.runNext.Name = "runNext";
            this.runNext.Size = new System.Drawing.Size(15, 14);
            this.runNext.TabIndex = 13;
            this.runNext.UseVisualStyleBackColor = true;
            this.runNext.CheckedChanged += new System.EventHandler(this.schedule_CheckedChanged);
            // 
            // runStandard
            // 
            this.runStandard.AutoSize = true;
            this.runStandard.Location = new System.Drawing.Point(90, 48);
            this.runStandard.Name = "runStandard";
            this.runStandard.Size = new System.Drawing.Size(69, 17);
            this.runStandard.TabIndex = 10;
            this.runStandard.Text = "Standard";
            this.runStandard.UseVisualStyleBackColor = true;
            this.runStandard.CheckedChanged += new System.EventHandler(this.schedule_CheckedChanged);
            // 
            // runIterative
            // 
            this.runIterative.AutoSize = true;
            this.runIterative.Location = new System.Drawing.Point(90, 71);
            this.runIterative.Name = "runIterative";
            this.runIterative.Size = new System.Drawing.Size(64, 17);
            this.runIterative.TabIndex = 11;
            this.runIterative.Text = "Iterative";
            this.runIterative.UseVisualStyleBackColor = true;
            this.runIterative.CheckedChanged += new System.EventHandler(this.schedule_CheckedChanged);
            // 
            // runMLB
            // 
            this.runMLB.AutoSize = true;
            this.runMLB.Location = new System.Drawing.Point(11, 48);
            this.runMLB.Name = "runMLB";
            this.runMLB.Size = new System.Drawing.Size(48, 17);
            this.runMLB.TabIndex = 4;
            this.runMLB.Text = "MLB";
            this.runMLB.UseVisualStyleBackColor = true;
            this.runMLB.CheckedChanged += new System.EventHandler(this.schedule_CheckedChanged);
            // 
            // runWeighted
            // 
            this.runWeighted.AutoSize = true;
            this.runWeighted.Location = new System.Drawing.Point(90, 94);
            this.runWeighted.Name = "runWeighted";
            this.runWeighted.Size = new System.Drawing.Size(72, 17);
            this.runWeighted.TabIndex = 12;
            this.runWeighted.Text = "Weighted";
            this.runWeighted.UseVisualStyleBackColor = true;
            this.runWeighted.CheckedChanged += new System.EventHandler(this.schedule_CheckedChanged);
            // 
            // runNBA
            // 
            this.runNBA.AutoSize = true;
            this.runNBA.Location = new System.Drawing.Point(11, 71);
            this.runNBA.Name = "runNBA";
            this.runNBA.Size = new System.Drawing.Size(48, 17);
            this.runNBA.TabIndex = 5;
            this.runNBA.Text = "NBA";
            this.runNBA.UseVisualStyleBackColor = true;
            this.runNBA.CheckedChanged += new System.EventHandler(this.schedule_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Next Run: ";
            // 
            // runNFL
            // 
            this.runNFL.AutoSize = true;
            this.runNFL.Location = new System.Drawing.Point(11, 94);
            this.runNFL.Name = "runNFL";
            this.runNFL.Size = new System.Drawing.Size(46, 17);
            this.runNFL.TabIndex = 6;
            this.runNFL.Text = "NFL";
            this.runNFL.UseVisualStyleBackColor = true;
            this.runNFL.CheckedChanged += new System.EventHandler(this.schedule_CheckedChanged);
            // 
            // runNHL
            // 
            this.runNHL.AutoSize = true;
            this.runNHL.Location = new System.Drawing.Point(11, 117);
            this.runNHL.Name = "runNHL";
            this.runNHL.Size = new System.Drawing.Size(48, 17);
            this.runNHL.TabIndex = 7;
            this.runNHL.Text = "NHL";
            this.runNHL.UseVisualStyleBackColor = true;
            this.runNHL.CheckedChanged += new System.EventHandler(this.schedule_CheckedChanged);
            // 
            // systemTray
            // 
            this.systemTray.Icon = ((System.Drawing.Icon)(resources.GetObject("systemTray.Icon")));
            this.systemTray.Text = "BeatGraphs";
            this.systemTray.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.systemTray_Click);
            // 
            // clockTimer
            // 
            this.clockTimer.Tick += new System.EventHandler(this.clockTimer_Tick);
            // 
            // loadBar
            // 
            this.loadBar.Location = new System.Drawing.Point(12, 456);
            this.loadBar.Maximum = 30;
            this.loadBar.Name = "loadBar";
            this.loadBar.Size = new System.Drawing.Size(600, 23);
            this.loadBar.TabIndex = 3;
            // 
            // BeatGraphForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(626, 491);
            this.Controls.Add(this.loadBar);
            this.Controls.Add(this.tbOutput);
            this.Controls.Add(this.moduleSelect);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BeatGraphForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BeatGraphs";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MinimizeOnClose);
            this.Resize += new System.EventHandler(this.MinimizeForm);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
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
            this.RunPlayoffs.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RichTextBox tbOutput;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
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
        private System.Windows.Forms.NotifyIcon systemTray;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label clockLabel;
        private System.Windows.Forms.Timer clockTimer;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckBox runStandard;
        private System.Windows.Forms.CheckBox runIterative;
        private System.Windows.Forms.CheckBox runMLB;
        private System.Windows.Forms.CheckBox runWeighted;
        private System.Windows.Forms.CheckBox runNBA;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox runNFL;
        private System.Windows.Forms.CheckBox runNHL;
        private System.Windows.Forms.Label lastRun;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label currTime;
        private System.Windows.Forms.Label currDate;
        private System.Windows.Forms.Label lastDate;
        private System.Windows.Forms.Label lastTime;
        private System.Windows.Forms.DateTimePicker nextRunTime;
        private System.Windows.Forms.CheckBox runNext;
        private System.Windows.Forms.Button runNow;
        private System.Windows.Forms.Button newNHL;
        private System.Windows.Forms.Button newNFL;
        private System.Windows.Forms.Button newNBA;
        private System.Windows.Forms.Button newMLB;
        private System.Windows.Forms.GroupBox RunPlayoffs;
        private System.Windows.Forms.Button nhlPlayoffs;
        private System.Windows.Forms.Button nflPlayoffs;
        private System.Windows.Forms.Button nbaPlayoffs;
        private System.Windows.Forms.Button mlbPlayoffs;
        private System.Windows.Forms.ProgressBar loadBar;
    }
}

