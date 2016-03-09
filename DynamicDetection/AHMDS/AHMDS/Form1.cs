using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using AHMDS.Engine;

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
            // receiver log
            AHMDSWindow.Handler wnd1 = delegate(string apiCall)
            {
                textBox1.AppendText(apiCall);
                textBox1.AppendText("\n");
            };

            AHMDSWindow.Handler wnd2 = delegate(string apiCall)
            {
                textBox2.AppendText(apiCall);
                textBox2.AppendText("\n");
            };

            AHMDSWindow.Handler wnd3 = delegate(string apiCall)
            {
                textBox3.AppendText(apiCall);
                textBox3.AppendText("\n");
            };

            cw1 = new AHMDSWindow("Malware1", "Malware1", wnd1);

            cw2 = new AHMDSWindow("Malware2", "Malware2", wnd2);

            cw3 = new AHMDSWindow("Malware3", "Malware3", wnd3);

            
        }

        AHMDSWindow cw1;
        AHMDSWindow cw2;
        AHMDSWindow cw3;

        private void button1_Click(object sender, EventArgs e)
        {
            cw1.Dispose();
        }
    }
}
