using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using AHMDS.Engine;
using AHMDS.GUI;

namespace AHMDS
{
    public partial class FormBatchAnalyzer : Form
    {
        private List<HybridAnalyzer.HybridObject> currentAnalyzed;

        public FormBatchAnalyzer()
        {
            InitializeComponent();
            FormBatchAnalyzer.CheckForIllegalCrossThreadCalls = false;
            currentAnalyzed = new List<HybridAnalyzer.HybridObject>();

        }

        private void updateResult(Analyzer.AnalyzedObject dsender, MalwareInfo result)
        {
            HybridAnalyzer.HybridObject sender = (HybridAnalyzer.HybridObject)dsender;
            ListViewItem item = (ListViewItem)sender.storage;
            item.Tag = result;
            currentAnalyzed.Remove(sender);
            switch (result.ResultCode)
            {
                case MalwareInfo.POSITIVE:
                    item.SubItems[2].ForeColor = Color.Red;
                    item.SubItems[2].Text = "[F] Malware";
                    break;

                case MalwareInfo.NEGATIVE:
                    item.SubItems[2].ForeColor = Color.DarkGreen;
                    item.SubItems[2].Text = "[F] Clean";
                    break;

                case MalwareInfo.TRUSTED:
                    item.SubItems[2].ForeColor = Color.DarkGreen;
                    item.SubItems[2].Text = "[F] Trusted";
                    break;

            }
        }

        private void updateStatus(Analyzer.AnalyzedObject asender)
        {
            HybridAnalyzer.HybridObject sender = (HybridAnalyzer.HybridObject)asender;
            ListViewItem item = (ListViewItem) sender.storage;

            switch (sender.Status)
            {
                case HybridAnalyzer.HybridObject.VERIFYING:
                    item.SubItems[2].ForeColor = Color.DarkBlue;
                    item.SubItems[2].Text = "Verifying";
                    break;

                case HybridAnalyzer.HybridObject.STATIC_ANALYZING:
                    item.SubItems[2].ForeColor = Color.DarkBlue;
                    item.SubItems[2].Text = "[SA] Analyzing";
                    break;

                case HybridAnalyzer.HybridObject.DYNAMIC_INITIALIZED:
                    item.SubItems[2].ForeColor = Color.DarkBlue;
                    item.SubItems[2].Text = "[DA] Queued";
                    break;

                case HybridAnalyzer.HybridObject.DYNAMIC_WAITING:
                    item.SubItems[1].ForeColor = Color.Black;
                    item.SubItems[2].ForeColor = Color.DarkRed;
                    item.SubItems[1].Text = sender.Box;
                    item.SubItems[2].Text = "[DA] Waiting";
                    break;

                case HybridAnalyzer.HybridObject.DYNAMIC_ANALYZING:
                    item.SubItems[2].ForeColor = Color.DarkBlue;
                    item.SubItems[2].Text = "[DA] Analyzing";
                    break;

            }
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            if (btnScan.Text == "SCAN")
            {
                lstAnalyze.Items.Clear();
                btnScan.Text = "STOP";
                beginScan();
            }
            else
            {
                stopScan();
                btnScan.Text = "SCAN";
            }
            
        }

        private void beginScan()
        {
            string alamat = txtAddr.Text;
            Queue<string> tmpScan = new Queue<string>(Directory.GetDirectories(alamat));

            addFile(Directory.GetFiles(alamat));

            while (tmpScan.Count > 0)
            {
                string currentDir = tmpScan.Dequeue();
                string[] subDirs = Directory.GetDirectories(currentDir);

                foreach (string subDir in subDirs)
                    tmpScan.Enqueue(subDir);

                addFile(Directory.GetFiles(currentDir));
            }


            foreach (ListViewItem item in lstAnalyze.Items)
            {
                HybridAnalyzer.HybridObject obj = new HybridAnalyzer.HybridObject((string)item.Tag, updateResult, updateStatus);
                obj.storage = item;

                currentAnalyzed.Add(obj);
                HybridAnalyzer.AddQueue(obj);
            }
        }

        private void stopScan()
        {
            foreach (HybridAnalyzer.HybridObject obj in currentAnalyzed)
            {
                obj.Terminate();
            }

            currentAnalyzed.Clear();
            DynamicAnalyzer.ClearQueue();
            HybridAnalyzer.ClearQueue();
        }

        private void addFile(string[] files)
        {
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                if (fileName.EndsWith(".exe"))
                {
                    ListViewItem item = lstAnalyze.Items.Add(fileName);
                    item.Tag = file;
                    item.UseItemStyleForSubItems = false;

                    item.SubItems.Add("N/A").ForeColor = Color.Gray;
                    item.SubItems.Add("Not started").ForeColor = Color.Gray;

                }
            }
        }

        private void FormBatchAnalyzer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnScan.Text == "STOP"){
                if (MessageBox.Show("A scan is being done. Are you sure want to cancel it?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.No)
                    e.Cancel = true;
                else
                    stopScan();
            }

        }

        private void lstAnalyze_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstAnalyze.SelectedItems[0].Tag.GetType() == typeof(string))
                MessageBox.Show("AHMDS can't show analysis result for unanalyzed file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else
            {
                MalwareInfo malwareinfo = (MalwareInfo)lstAnalyze.SelectedItems[0].Tag;
                FormAnalysisResult analysisResult = new FormAnalysisResult(lstAnalyze.SelectedItems[0].Text, malwareinfo);
                analysisResult.Show();
            }
        }

    }
}
