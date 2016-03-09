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
            CustomWindow.Handler wnd1 = delegate(string apiCall)
            {
                textBox1.AppendText(apiCall);
                textBox1.AppendText("\n");
            };

            CustomWindow.Handler wnd2 = delegate(string apiCall)
            {
                textBox2.AppendText(apiCall);
                textBox2.AppendText("\n");
            };

            CustomWindow.Handler wnd3 = delegate(string apiCall)
            {
                textBox3.AppendText(apiCall);
                textBox3.AppendText("\n");
            };

            cw1 = new CustomWindow("Malware1", "Malware1", wnd1);

            cw2 = new CustomWindow("Malware2", "Malware2", wnd2);

            cw3 = new CustomWindow("Malware3", "Malware3", wnd3);

            
        }

        CustomWindow cw1;
        CustomWindow cw2;
        CustomWindow cw3;

        private void button1_Click(object sender, EventArgs e)
        {
            cw1.Dispose();
        }
    }
}
