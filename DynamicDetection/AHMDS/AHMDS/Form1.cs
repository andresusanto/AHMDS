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
            CustomWindow.Handler wnd1 = delegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
            {
                if (msg == 0x004A)
                {
                    CustomWindow.COPYDATASTRUCT cds = (CustomWindow.COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(CustomWindow.COPYDATASTRUCT));
                    if (cds.cbData > 0)
                    {
                        byte[] data = new byte[cds.cbData];
                        Marshal.Copy(cds.lpData, data, 0, cds.cbData);
                        Encoding unicodeStr = Encoding.ASCII;
                        char[] myString = unicodeStr.GetChars(data);
                        string returnText = new string(myString);

                        textBox1.AppendText(returnText);
                        textBox1.AppendText("\n");

                    }
                }
            };

            CustomWindow.Handler wnd2 = delegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
            {
                if (msg == 0x004A)
                {
                    CustomWindow.COPYDATASTRUCT cds = (CustomWindow.COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(CustomWindow.COPYDATASTRUCT));
                    if (cds.cbData > 0)
                    {
                        byte[] data = new byte[cds.cbData];
                        Marshal.Copy(cds.lpData, data, 0, cds.cbData);
                        Encoding unicodeStr = Encoding.ASCII;
                        char[] myString = unicodeStr.GetChars(data);
                        string returnText = new string(myString);

                        textBox2.AppendText(returnText);
                        textBox2.AppendText("\n");

                    }
                }
            };

            CustomWindow.Handler wnd3 = delegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
            {
                if (msg == 0x004A)
                {
                    CustomWindow.COPYDATASTRUCT cds = (CustomWindow.COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(CustomWindow.COPYDATASTRUCT));
                    if (cds.cbData > 0)
                    {
                        byte[] data = new byte[cds.cbData];
                        Marshal.Copy(cds.lpData, data, 0, cds.cbData);
                        Encoding unicodeStr = Encoding.ASCII;
                        char[] myString = unicodeStr.GetChars(data);
                        string returnText = new string(myString);

                        textBox3.AppendText(returnText);
                        textBox3.AppendText("\n");

                    }
                }
            };

            CustomWindow cw1 = new CustomWindow("AHMDSAP1", "Malware1", wnd1);

            CustomWindow cw2 = new CustomWindow("AHMDSAP2", "Malware2", wnd2);

            CustomWindow cw3 = new CustomWindow("AHMDSAP3", "Malware3", wnd3);
            
        }
    }
}
