using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using coba.Engine;
using System.Security.Cryptography.X509Certificates;

namespace coba
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string oke;
            //bool okez = WinTrust.CheckSignature(this.Handle, @"D:\SETUP\android-ndk-r10e-windows-x86_64.exe", false, out oke);

            //if (okez)
            //    MessageBox.Show(okez.ToString());
            //else
            //    MessageBox.Show(oke);

            bool result = WinTrust.VerifyEmbeddedSignature(@"D:\SETUP\SourceTreeSetup_1.6.13.exe");
            //MessageBox.Show(result.ToString());

            if (result)
            {
                X509Certificate cert = X509Certificate.CreateFromSignedFile(@"D:\SETUP\SourceTreeSetup_1.6.13.exe");
                MessageBox.Show(cert.Subject);
            }
        }
    }
}
