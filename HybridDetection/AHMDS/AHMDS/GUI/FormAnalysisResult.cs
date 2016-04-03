using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AHMDS.Engine;

namespace AHMDS.GUI
{
    public partial class FormAnalysisResult : Form
    {
        public FormAnalysisResult(string fileName, MalwareInfo malwareInfo)
        {
            InitializeComponent();
            lblFilename.Text = fileName;
            lblAnalysisResult.Text = malwareInfo.ResultInformation;
            
            switch(malwareInfo.ResultCode)
            {
                case MalwareInfo.TRUSTED:
                    lblIndicator.Text = "TRUSTED";
                    lblIndicator.ForeColor = Color.DarkBlue;
                    break;
                case MalwareInfo.NEGATIVE:
                    lblIndicator.Text = "SAFE";
                    lblIndicator.ForeColor = Color.DarkGreen;
                    break;
                case MalwareInfo.POSITIVE:
                    lblIndicator.Text = "DANGER";
                    lblIndicator.ForeColor = Color.DarkRed;
                    break;

            }

            lblThreatScore.Text = malwareInfo.Score.ToString();
            if (malwareInfo.Score > Properties.Settings.Default.MalwareScoreThreshold)
                lblThreatScore.ForeColor = Color.DarkRed;
            else if (malwareInfo.Score > Properties.Settings.Default.APICallScoreThreshold)
                lblThreatScore.ForeColor = Color.DarkOrange;

            if (malwareInfo.Explanation == null) return;

            foreach (string explanation in malwareInfo.Explanation)
            {
                txtExplanations.AppendText(explanation);
                txtExplanations.AppendText("\r\n");
            }
        }
    }
}
