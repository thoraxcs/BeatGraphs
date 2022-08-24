namespace BeatGraphs
{
    partial class Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
            this.setVerbose = new System.Windows.Forms.CheckBox();
            this.butSave = new System.Windows.Forms.Button();
            this.butCancel = new System.Windows.Forms.Button();
            this.setUpload = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // setVerbose
            // 
            this.setVerbose.AutoSize = true;
            this.setVerbose.Location = new System.Drawing.Point(12, 12);
            this.setVerbose.Name = "setVerbose";
            this.setVerbose.Size = new System.Drawing.Size(95, 17);
            this.setVerbose.TabIndex = 1;
            this.setVerbose.Text = "Verbose Mode";
            this.setVerbose.UseVisualStyleBackColor = true;
            // 
            // butSave
            // 
            this.butSave.Location = new System.Drawing.Point(14, 79);
            this.butSave.Name = "butSave";
            this.butSave.Size = new System.Drawing.Size(75, 23);
            this.butSave.TabIndex = 2;
            this.butSave.Text = "Save";
            this.butSave.UseVisualStyleBackColor = true;
            this.butSave.Click += new System.EventHandler(this.butSave_Click);
            // 
            // butCancel
            // 
            this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.butCancel.Location = new System.Drawing.Point(95, 79);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(75, 23);
            this.butCancel.TabIndex = 3;
            this.butCancel.Text = "Cancel";
            this.butCancel.UseVisualStyleBackColor = true;
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // setUpload
            // 
            this.setUpload.AutoSize = true;
            this.setUpload.Location = new System.Drawing.Point(12, 35);
            this.setUpload.Name = "setUpload";
            this.setUpload.Size = new System.Drawing.Size(144, 17);
            this.setUpload.TabIndex = 4;
            this.setUpload.Text = "Enable File Upload (FTP)";
            this.setUpload.UseVisualStyleBackColor = true;
            // 
            // Settings
            // 
            this.AcceptButton = this.butSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.butCancel;
            this.ClientSize = new System.Drawing.Size(182, 114);
            this.Controls.Add(this.setUpload);
            this.Controls.Add(this.butCancel);
            this.Controls.Add(this.butSave);
            this.Controls.Add(this.setVerbose);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox setVerbose;
        private System.Windows.Forms.Button butSave;
        private System.Windows.Forms.Button butCancel;
        private System.Windows.Forms.CheckBox setUpload;
    }
}