namespace AHMDS.GUI
{
    partial class FormAnalysisResult
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAnalysisResult));
            this.lblIndicator = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblFilename = new System.Windows.Forms.Label();
            this.lblThreatScore = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblAnalysisResult = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtExplanations = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblIndicator
            // 
            this.lblIndicator.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIndicator.ForeColor = System.Drawing.Color.Green;
            this.lblIndicator.Location = new System.Drawing.Point(496, 47);
            this.lblIndicator.Name = "lblIndicator";
            this.lblIndicator.Size = new System.Drawing.Size(174, 46);
            this.lblIndicator.TabIndex = 1;
            this.lblIndicator.Text = "SAFE";
            this.lblIndicator.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblAnalysisResult);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.lblThreatScore);
            this.groupBox1.Controls.Add(this.lblFilename);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.lblIndicator);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(676, 122);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "File Status";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "File Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Threat Score";
            // 
            // lblFilename
            // 
            this.lblFilename.AutoSize = true;
            this.lblFilename.Location = new System.Drawing.Point(115, 29);
            this.lblFilename.Name = "lblFilename";
            this.lblFilename.Size = new System.Drawing.Size(35, 13);
            this.lblFilename.TabIndex = 4;
            this.lblFilename.Text = "label4";
            // 
            // lblThreatScore
            // 
            this.lblThreatScore.AutoSize = true;
            this.lblThreatScore.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblThreatScore.Location = new System.Drawing.Point(115, 58);
            this.lblThreatScore.Name = "lblThreatScore";
            this.lblThreatScore.Size = new System.Drawing.Size(41, 13);
            this.lblThreatScore.TabIndex = 5;
            this.lblThreatScore.Text = "label5";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(22, 88);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Analysis Result";
            // 
            // lblAnalysisResult
            // 
            this.lblAnalysisResult.AutoSize = true;
            this.lblAnalysisResult.Location = new System.Drawing.Point(115, 88);
            this.lblAnalysisResult.Name = "lblAnalysisResult";
            this.lblAnalysisResult.Size = new System.Drawing.Size(35, 13);
            this.lblAnalysisResult.TabIndex = 7;
            this.lblAnalysisResult.Text = "label7";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtExplanations);
            this.groupBox2.Location = new System.Drawing.Point(12, 140);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(676, 362);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Explanations";
            // 
            // txtExplanations
            // 
            this.txtExplanations.BackColor = System.Drawing.Color.White;
            this.txtExplanations.Location = new System.Drawing.Point(15, 21);
            this.txtExplanations.Multiline = true;
            this.txtExplanations.Name = "txtExplanations";
            this.txtExplanations.ReadOnly = true;
            this.txtExplanations.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtExplanations.Size = new System.Drawing.Size(648, 323);
            this.txtExplanations.TabIndex = 0;
            // 
            // FormAnalysisResult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 514);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormAnalysisResult";
            this.Text = "Analysis Result";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblIndicator;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblThreatScore;
        private System.Windows.Forms.Label lblFilename;
        private System.Windows.Forms.Label lblAnalysisResult;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtExplanations;
    }
}