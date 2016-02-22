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
            Form2.CheckForIllegalCrossThreadCalls = false;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

            
            DynamicAnalyzer.DynamicObject obj1 = null;
            
            // coba sandboxie
            DynamicAnalyzer.Handler act = delegate(MalwareInfo result)
            {
                //textBox1.AppendText("Modified Dirs:\r\n");
                //foreach (string s in obj1.scannedDirectories)
                //{
                //    textBox1.AppendText(s);
                //    textBox1.AppendText("\r\n");
                //}

                //textBox1.AppendText("Modified Files:\r\n");
                //foreach (string s in obj1.scannedFiles)
                //{
                //    textBox1.AppendText(s);
                //    textBox1.AppendText("\r\n");
                //}

                //Console.WriteLine(result.ResultInformation);
                //MessageBox.Show(result.ResultInformation);
            };

            obj1 = new DynamicAnalyzer.DynamicObject(@"D:\Project\AV\SAMPLES\tesfolder\orion.exe", act);
            //DynamicAnalyzer.DynamicObject obj2 = new DynamicAnalyzer.DynamicObject(@"C:\Python27\python.exe", act);
            //DynamicAnalyzer.DynamicObject obj3 = new DynamicAnalyzer.DynamicObject(@"C:\Python27\python.exe", act);
            //DynamicAnalyzer.DynamicObject obj4 = new DynamicAnalyzer.DynamicObject(@"C:\Python27\python.exe", act);
            //DynamicAnalyzer.DynamicObject obj5 = new DynamicAnalyzer.DynamicObject(@"C:\Python27\python.exe", act);
            //DynamicAnalyzer.DynamicObject obj6 = new DynamicAnalyzer.DynamicObject(@"C:\Python27\python.exe", act);

            
            DynamicAnalyzer.AddQueue(obj1);
            //DynamicAnalyzer.AddQueue(obj2);
            //DynamicAnalyzer.AddQueue(obj3);
            //DynamicAnalyzer.AddQueue(obj4);
            //DynamicAnalyzer.AddQueue(obj5);
            //DynamicAnalyzer.AddQueue(obj6);


        }
    }
}
