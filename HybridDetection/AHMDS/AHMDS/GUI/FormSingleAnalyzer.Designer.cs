namespace AHMDS.GUI
{
    partial class FormSingleAnalyzer
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSingleAnalyzer));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblInfo = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblFilename = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblSandbox = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cmdRun = new System.Windows.Forms.Button();
            this.cmdReport = new System.Windows.Forms.Button();
            this.tmrDuration = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::AHMDS.Properties.Resources.icon_109133;
            this.pictureBox1.Location = new System.Drawing.Point(113, 27);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(65, 68);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(184, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(250, 32);
            this.label1.TabIndex = 1;
            this.label1.Text = "AHMDS RunGuard";
            // 
            // lblInfo
            // 
            this.lblInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo.ForeColor = System.Drawing.Color.DarkRed;
            this.lblInfo.Location = new System.Drawing.Point(56, 116);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(467, 23);
            this.lblInfo.TabIndex = 2;
            this.lblInfo.Text = "Please wait, we\'re checking the file for malware ...";
            this.lblInfo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(110, 160);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Filename:";
            // 
            // lblFilename
            // 
            this.lblFilename.AutoSize = true;
            this.lblFilename.Location = new System.Drawing.Point(168, 160);
            this.lblFilename.Name = "lblFilename";
            this.lblFilename.Size = new System.Drawing.Size(60, 13);
            this.lblFilename.TabIndex = 4;
            this.lblFilename.Text = "FILENAME";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(110, 190);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Status:";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(168, 190);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(41, 13);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "Started";
            // 
            // lblSandbox
            // 
            this.lblSandbox.AutoSize = true;
            this.lblSandbox.Location = new System.Drawing.Point(168, 220);
            this.lblSandbox.Name = "lblSandbox";
            this.lblSandbox.Size = new System.Drawing.Size(27, 13);
            this.lblSandbox.TabIndex = 8;
            this.lblSandbox.Text = "N/A";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(110, 220);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Sandbox:";
            // 
            // cmdRun
            // 
            this.cmdRun.Enabled = false;
            this.cmdRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdRun.Location = new System.Drawing.Point(206, 271);
            this.cmdRun.Name = "cmdRun";
            this.cmdRun.Size = new System.Drawing.Size(75, 23);
            this.cmdRun.TabIndex = 9;
            this.cmdRun.Text = "Run File";
            this.cmdRun.UseVisualStyleBackColor = true;
            this.cmdRun.Click += new System.EventHandler(this.cmdRun_Click);
            // 
            // cmdReport
            // 
            this.cmdReport.Enabled = false;
            this.cmdReport.Location = new System.Drawing.Point(287, 271);
            this.cmdReport.Name = "cmdReport";
            this.cmdReport.Size = new System.Drawing.Size(75, 23);
            this.cmdReport.TabIndex = 10;
            this.cmdReport.Text = "View Report";
            this.cmdReport.UseVisualStyleBackColor = true;
            this.cmdReport.Click += new System.EventHandler(this.cmdReport_Click);
            // 
            // tmrDuration
            // 
            this.tmrDuration.Enabled = true;
            this.tmrDuration.Interval = 1000;
            this.tmrDuration.Tick += new System.EventHandler(this.tmrDuration_Tick);
            // 
            // FormSingleAnalyzer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(573, 315);
            this.Controls.Add(this.cmdReport);
            this.Controls.Add(this.cmdRun);
            this.Controls.Add(this.lblSandbox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblFilename);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSingleAnalyzer";
            this.Text = "AHMDS RunGuard";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormSingleAnalyzer_FormClosed);
            this.Load += new System.EventHandler(this.FormSingleAnalyzer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblFilename;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblSandbox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button cmdRun;
        private System.Windows.Forms.Button cmdReport;
        private System.Windows.Forms.Timer tmrDuration;
    }
}