using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AHMDS.Engine;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.IO;

namespace AHMDS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            System.Windows.Forms.Form.CheckForIllegalCrossThreadCalls = false;

            Thread scanthread = new Thread(scan);
            scanthread.Start();
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

            tmp.Add(alamat);

            return tmp;
        }

        private void scan()
        {
            StaticAnalyzer staticanalyzer = new StaticAnalyzer();
            //List<string> antriFolder = expandFolder(@"D:\Project\AV\SAMPLES");
            List<string> antriFolder = expandFolder(@"D:\Project\AV\trusted");
            long check = 0;

            foreach (string a in antriFolder)
            {
                string[] filePaths = Directory.GetFiles(a);
                foreach (string nama in filePaths)
                {
                    //if (nama.Substring(nama.Length - 4).ToLower().Equals(".exe"))
                    //{

                    check++;


                    if (staticanalyzer.Verify(nama))
                    {
                        textBox1.AppendText(nama);
                        textBox1.AppendText("\r\n");
                    }
                    else
                    {
                        textBox2.AppendText(nama);
                        textBox2.AppendText("\r\n");
                    }

                    //X509Certificate cert = WinTrust.GetVerifiedCert(nama);
                    //if (cert == null)
                    //{

                    //    label1.Text = "Scanning " + nama;

                    //    MalwareInfo result = staticanalyzer.Check(nama);

                    //    if (result.ResultCode == MalwareInfo.POSITIVE)
                    //    {
                    //        listBox1.Items.Add(nama + " --> " + result.ResultInformation);
                    //    }
                    //}
                    //else
                    //{
                    //    textBox1.AppendText(cert.Subject);
                    //    textBox1.AppendText("\r\n");

                    //    textBox2.AppendText(cert.Issuer);
                    //    textBox2.AppendText("\r\n");
                    //    //MessageBox.Show("Verified Program:\n" + nama + "\n\n" + cert.Subject + "\n\n" + cert.Handle);
                    //}
                    //}
                }
            }
            MessageBox.Show("Scan completed! " + check + " file(s) scaned.");
        }
    }
}
