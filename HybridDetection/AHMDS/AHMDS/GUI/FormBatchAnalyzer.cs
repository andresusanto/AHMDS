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
            DynamicAnalyzer.DynamicObject sender = (DynamicAnalyzer.DynamicObject)asender;
            ListViewItem item = (ListViewItem) sender.storage;

            switch (sender.Status)
            {
                case DynamicAnalyzer.DynamicObject.WAITING:
                    item.SubItems[1].ForeColor = Color.Black;
                    item.SubItems[2].ForeColor = Color.DarkRed;
                    item.SubItems[1].Text = sender.Box;
                    item.SubItems[2].Text = "Waiting for malware";
                    break;

                case DynamicAnalyzer.DynamicObject.ANALYZING:
                    item.SubItems[2].ForeColor = Color.DarkBlue;
                    item.SubItems[2].Text = "Analyzing malware";
                    break;

                case DynamicAnalyzer.DynamicObject.FINISHED:
                    item.SubItems[2].ForeColor = Color.DarkGreen;
                    item.SubItems[2].Text = "Finished";
                    break;
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
                DynamicAnalyzer.ResultHandler act = delegate(Analyzer.AnalyzedObject dsender, MalwareInfo result){
                    Console.WriteLine("Skor: " + result.Score);
                };
                DynamicAnalyzer.DynamicObject obj = new DynamicAnalyzer.DynamicObject((string)item.Tag, act, updateUI);
                obj.storage = item;
                DynamicAnalyzer.AddQueue(obj);
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
