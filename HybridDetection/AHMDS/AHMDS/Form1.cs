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

        private void tes()
        {
            RuleEngine.CalculationResult result = RuleEngine.CalculateAPICalls(new string[2] { "GetAsyncKeyState", "GetKeyState" });
            result = RuleEngine.CalculateAPICalls(new string[2] { "GetAsyncKeyState", "GetKeyState" });

            foreach (String s in result.Explanation)
                textBox1.AppendText(s + "\n");

            Dictionary<string, List<string>> registry = new Dictionary<string, List<string>>();

            List<string> entry = new List<string>();
            entry.Add("zXaxhIpd");

            registry.Add(@"user\current\software\microsoft\windows\currentversion\run", entry);


            entry = new List<string>();
            entry.Add("startup");
            entry.Add("common startup");

            registry.Add(@"user\current\software\microsoft\windows\currentversion\explorer\shell folders", entry);
            registry.Add(@"machine\software\microsoft\active setup\installed components\{123123132-123123}\component\run", entry);


            registry.Add(@"machine\system\currentcontrolset\control\session manager\knowndlls", entry);

            entry = new List<string>();
            entry.Add("pendingfilerenameoperations");

            registry.Add(@"machine\system\currentcontrolset\control\session manager", entry);

            RuleEngine.CalculationResult resReg = RuleEngine.CalculateRegistries(registry);

            foreach (String s in resReg.Explanation)
                textBox1.AppendText(s + "\n");

            List<string> fileList = new List<string>();
            fileList.Add(@"drive\D\SAMPEL\theZoo-master\Ransomware.Locky\Locky.exe");
            fileList.Add(@"drive\C\Windows\System32\Ransomware.Locky\Locky.exe");
            fileList.Add(@"user\current\AppData\Local\Temp\svchost.exe");
            fileList.Add(@"user\current\AppData\Roaming\Microsoft\Windows\Cookies\index.dat");
            fileList.Add(@"user\current\AppData\Local\Microsoft\Windows\History\History.IE5\index.dat");
            fileList.Add(@"user\current\AppData\Local\Microsoft\Windows\Temporary Internet Files\Content.IE5\index.dat");
            fileList.Add(@"user\current\AppData\Local\Temp\mcwsazmq.exe");
            fileList.Add(@"user\current\AppData\Local\Temp\setup.dat");
            fileList.Add(@"user\current\AppData\Local\Temp\Winlogon.exe");
            fileList.Add(@"user\current\AppData\Local\Microsoft\Feeds Cache\jusched.exe");
            fileList.Add(@"user\current\AppData\Roaming\Microsoft\Windows\Cookies\CDProxyServ.exe");
            fileList.Add(@"user\current\AppData\Roaming\Microsoft\Windows\PrivacIE\index.dat");
            fileList.Add(@"user\current\AppData\Local\Microsoft\Feeds\{5588ACFD-6436-411B-A5CE-666AE6A92D3D}~\WebSlices~\Suggested Sites~.feed-ms");
            fileList.Add(@"user\current\AppData\Local\Microsoft\Feeds\{5588ACFD-6436-411B-A5CE-666AE6A92D3D}~\WebSlices~\Web Slice Gallery~.feed-ms");
            fileList.Add(@"user\current\AppData\Local\Microsoft\Windows\History\History.IE5\index.dat");
            fileList.Add(@"user\current\AppData\Local\Microsoft\Windows\Temporary Internet Files\Content.IE5\index.dat");
            fileList.Add(@"user\current\AppData\Local\Microsoft\Internet Explorer\Recovery\High\Active\RecoveryStore.{22C331BF-E6DA-11E5-AC19-00155D03580B}.dat");
            fileList.Add(@"user\current\AppData\Local\Microsoft\Internet Explorer\Recovery\High\Active\{22C331C0-E6DA-11E5-AC19-00155D03580B}.dat");
            fileList.Add(@"user\current\AppData\Local\Microsoft\Windows\History\History.IE5\MSHist012016031020160311\index.dat");
            fileList.Add(@"user\current\AppData\Local\Microsoft\Windows\Temporary Internet Files\Content.IE5\DO1V1PMJ\SN[1].png");
            fileList.Add(@"user\current\AppData\Local\Microsoft\Windows\Temporary Internet Files\Content.IE5\OVSH3WQL\favicon[2].png");
            fileList.Add(@"drive\C\Program Files (x86)\Windows Portable Devices\vwwtOaMP.exe");
            fileList.Add(@"user\current\AppData\Local\Microsoft\AbJcHdcZ.exe");
            fileList.Add(@"user\current\AppData\Local\Temp\NyQoIZFK.exe");
            fileList.Add(@"user\current\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup\rQdrzdRY.exe");

            RuleEngine.CalculationResult resFile = RuleEngine.CalculateFiles(fileList);

            foreach (String s in resFile.Explanation)
                textBox1.AppendText(s + "\n");

            textBox2.Text = "" + (result.Score + resReg.Score + resFile.Score);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread coba = new Thread(tes);
            coba.Start();
        }
    }
}
