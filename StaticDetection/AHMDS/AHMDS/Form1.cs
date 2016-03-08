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
using System.Threading;
using System.Security.Cryptography.X509Certificates;

namespace AHMDS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            System.Windows.Forms.Form.CheckForIllegalCrossThreadCalls = false;

            StaticAnalyzer sa = new StaticAnalyzer();
            sa.extractAPICalls(@"D:\Project\TA\bsa\BSA.EXE");
        }

        List<string> expandFolder(string alamat)
        {
            List<string> tmp = new List<string>();
            string[] sub = Directory.GetDirectories(alamat);

            tmp.AddRange(sub.ToList());

            foreach (string s in sub)
            {
                tmp.AddRange(expandFolder(s));
            }

            
            return tmp;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread scanthread = new Thread(scan);
            scanthread.Start();
        }

        private void scan()
        {
            StaticAnalyzer staticanalyzer = new StaticAnalyzer();
            List<string> antriFolder = expandFolder(@"D:\Project\AV\SAMPLES");
            long check = 0;

            foreach (string a in antriFolder)
            {
                string[] filePaths = Directory.GetFiles(a);
                foreach (string nama in filePaths)
                {
                    //if (nama.Substring(nama.Length - 4).ToLower().Equals(".exe"))
                    //{
                    
                        check++;

                        X509Certificate cert = WinTrust.GetVerifiedCert(nama);
                        if (cert == null)
                        {

                            label1.Text = "Scanning " + nama;

                            MalwareInfo result = staticanalyzer.Check(nama);

                            if (result.ResultCode == MalwareInfo.POSITIVE)
                            {
                                listBox1.Items.Add(nama + " --> " + result.ResultInformation);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Verified Program:\n" + nama + "\n\n" + cert.Subject + "\n\n" + cert.Handle);
                        }
                    //}
                }
            }
            MessageBox.Show("Scan completed! " + check + " file(s) scaned.");
        }
    }
}
