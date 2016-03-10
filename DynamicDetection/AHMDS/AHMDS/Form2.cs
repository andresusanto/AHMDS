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
            DynamicAnalyzer.ResultHandler act = delegate(DynamicAnalyzer.DynamicObject obj, MalwareInfo result)
            {
                //Dictionary<string, List<string>> hasil = obj1.registries;

                //foreach (KeyValuePair<string, List<string>> entry in hasil)
                //{
                //    textBox1.AppendText(entry.Key);
                //    textBox1.AppendText("\r\n=====================================\r\n");

                //    foreach (string konten in entry.Value)
                //    {
                //        textBox1.AppendText(konten);
                //        textBox1.AppendText("\r\n");
                //    }
                //    textBox1.AppendText("\r\n");
                //}

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

            //obj1 = new DynamicAnalyzer.DynamicObject(@"notepad.exe", act);
            //DynamicAnalyzer.DynamicObject obj2 = new DynamicAnalyzer.DynamicObject(@"C:\Python27\python.exe", act);
            //DynamicAnalyzer.DynamicObject obj3 = new DynamicAnalyzer.DynamicObject(@"C:\Python27\python.exe", act);
            //DynamicAnalyzer.DynamicObject obj4 = new DynamicAnalyzer.DynamicObject(@"C:\Python27\python.exe", act);
            //DynamicAnalyzer.DynamicObject obj5 = new DynamicAnalyzer.DynamicObject(@"C:\Python27\python.exe", act);
            //DynamicAnalyzer.DynamicObject obj6 = new DynamicAnalyzer.DynamicObject(@"C:\Python27\python.exe", act);

            
            //DynamicAnalyzer.AddQueue(obj1);


            //DynamicAnalyzer.AddQueue(obj2);
            //DynamicAnalyzer.AddQueue(obj3);
            //DynamicAnalyzer.AddQueue(obj4);
            //DynamicAnalyzer.AddQueue(obj5);
            //DynamicAnalyzer.AddQueue(obj6);


        }
    }
}
