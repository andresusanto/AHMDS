using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AHMDS.Engine;

namespace AHMDS
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();

            StaticAnalyzer sa = new StaticAnalyzer();

            List<string> res = sa.extractAPICalls(@"D:\Project\AV\SAMPLES\tesfolder\coba");


            foreach (string api in res)
            {
                textBox1.AppendText(api);
                textBox1.AppendText("\n");
            }
        }
    }
}
