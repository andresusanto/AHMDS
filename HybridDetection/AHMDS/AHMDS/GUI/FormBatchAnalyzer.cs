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

namespace AHMDS
{
    public partial class FormBatchAnalyzer : Form
    {
        public FormBatchAnalyzer()
        {
            InitializeComponent();
            FormBatchAnalyzer.CheckForIllegalCrossThreadCalls = false;
        }

        private void updateUI(Analyzer.AnalyzedObject asender)
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

                //case HybridAnalyzer.HybridObject.FINISHED:
                //    item.SubItems[2].ForeColor = Color.DarkGreen;
                //    item.SubItems[2].Text = "Finished";
                //    break;
            }
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            btnScan.Enabled = false;

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
                HybridAnalyzer.ResultHandler act = delegate(Analyzer.AnalyzedObject dsender, MalwareInfo result)
                {
                    Console.WriteLine("Skor: " + result.Score);
                };
                HybridAnalyzer.HybridObject obj = new HybridAnalyzer.HybridObject((string)item.Tag, updateResult, updateUI);
                obj.storage = item;
                HybridAnalyzer.AddQueue(obj);
            }

        }

        private void updateResult(Analyzer.AnalyzedObject dsender, MalwareInfo result)
        {
            HybridAnalyzer.HybridObject sender = (HybridAnalyzer.HybridObject)dsender;
            ListViewItem item = (ListViewItem)sender.storage;

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

    }
}
