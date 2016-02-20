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
        }

        private void Form2_Load(object sender, EventArgs e)
        {

            // coba sandboxie
            DynamicAnalyzer.Handler act = delegate(MalwareInfo result)
            {
                Console.WriteLine(result.ResultInformation);
                //MessageBox.Show(result.ResultInformation);
            };

            DynamicAnalyzer.DynamicObject obj1 = new DynamicAnalyzer.DynamicObject(@"C:\Python27\python.exe", act);
            DynamicAnalyzer.DynamicObject obj2 = new DynamicAnalyzer.DynamicObject(@"C:\Python27\python.exe", act);
            DynamicAnalyzer.DynamicObject obj3 = new DynamicAnalyzer.DynamicObject(@"C:\Python27\python.exe", act);
            DynamicAnalyzer.DynamicObject obj4 = new DynamicAnalyzer.DynamicObject(@"C:\Python27\python.exe", act);
            DynamicAnalyzer.DynamicObject obj5 = new DynamicAnalyzer.DynamicObject(@"C:\Python27\python.exe", act);
            DynamicAnalyzer.DynamicObject obj6 = new DynamicAnalyzer.DynamicObject(@"C:\Python27\python.exe", act);


            DynamicAnalyzer.AddQueue(obj1);
            DynamicAnalyzer.AddQueue(obj2);
            DynamicAnalyzer.AddQueue(obj3);
            DynamicAnalyzer.AddQueue(obj4);
            DynamicAnalyzer.AddQueue(obj5);
            DynamicAnalyzer.AddQueue(obj6);


        }
    }
}
