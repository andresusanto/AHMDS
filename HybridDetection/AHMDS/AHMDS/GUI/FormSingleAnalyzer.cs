using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AHMDS.Engine;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AHMDS.GUI
{
    public partial class FormSingleAnalyzer : Form
    {
        private MalwareInfo malwareInfo;
        private int secondsToGo = Properties.Settings.Default.DynamicAnalysisDuration;
        private bool isWaiting = false;

        //[DllImport("kernel32.dll", SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //static extern bool AllocConsole();

        public FormSingleAnalyzer(string FileName)
        {
            FormSingleAnalyzer.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            lblFilename.Text = FileName;

            DynamicAnalyzer.Initialize();
            //AllocConsole();
        }

        private void updateStatus(Analyzer.AnalyzedObject asender)
        {
            HybridAnalyzer.HybridObject sender = (HybridAnalyzer.HybridObject)asender;

            switch (sender.Status)
            {
                case HybridAnalyzer.HybridObject.VERIFYING:
                    lblStatus.ForeColor = Color.DarkBlue;
                    lblStatus.Text = "Verifying Vendor";
                    break;

                case HybridAnalyzer.HybridObject.STATIC_ANALYZING:
                    lblStatus.Text = "[Static Analysis] Analyzing";
                    break;

                case HybridAnalyzer.HybridObject.DYNAMIC_INITIALIZED:
                    lblStatus.Text = "[Dynamic Analysis] Queued";
                    break;

                case HybridAnalyzer.HybridObject.DYNAMIC_WAITING:
                    isWaiting = true;
                    lblSandbox.Text = sender.Box;
                    lblStatus.Text = "[Dynamic Analysis] Waiting";

                    break;

                case HybridAnalyzer.HybridObject.DYNAMIC_ANALYZING:
                    isWaiting = false;
                    lblStatus.Text = "[Dynamic Analysis] Analyzing";
                    break;

            }
        }

        private void updateResult(Analyzer.AnalyzedObject dsender, MalwareInfo result)
        {
            HybridAnalyzer.HybridObject sender = (HybridAnalyzer.HybridObject)dsender;

            malwareInfo = result;
            switch (result.ResultCode)
            {
                case MalwareInfo.POSITIVE:
                    lblInfo.Text = "This file is dangerous!";
                    lblInfo.ForeColor = Color.Red;
                    lblStatus.ForeColor = Color.Red;
                    lblStatus.Text = result.ResultInformation;

                    cmdReport.Enabled = true;
                    break;


                case MalwareInfo.TRUSTED:
                case MalwareInfo.NEGATIVE:
                    lblInfo.ForeColor = Color.DarkGreen;
                    lblInfo.Text = "This file is safe";
                    lblStatus.ForeColor = Color.DarkGreen;
                    lblStatus.Text = result.ResultInformation;

                    cmdReport.Enabled = true;
                    cmdRun.Enabled = true;
                    break;

            }
        }

        private void cmdReport_Click(object sender, EventArgs e)
        {
            FormAnalysisResult result = new FormAnalysisResult(lblFilename.Text, malwareInfo);
            result.ShowDialog();
        }

        private void cmdRun_Click(object sender, EventArgs e)
        {
            Process.Start(lblFilename.Text);
            Application.Exit();
        }

        private void tmrDuration_Tick(object sender, EventArgs e)
        {
            if (isWaiting)
            {
                secondsToGo -= 1;
                lblStatus.Text = "[Dynamic Analysis] Waiting for " + (secondsToGo / 60).ToString("D2") + ":" +  (secondsToGo % 60).ToString("D2");
            }
        }
        
        private void FormSingleAnalyzer_Load(object sender, EventArgs e)
        {
            HybridAnalyzer.HybridObject obj = new HybridAnalyzer.HybridObject(lblFilename.Text, updateResult, updateStatus);
            obj.Start();
        }

        private void FormSingleAnalyzer_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
