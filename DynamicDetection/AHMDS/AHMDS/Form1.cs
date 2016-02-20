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

        //private class Handler : CustomWindow.IWndProc
        //{
        //    private TextBox textbox;

        //    public Handler(TextBox textbox)
        //    {
        //        this.textbox = textbox;
        //    }

        //    public void WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        //    {
        //        if (msg == 0x004A)
        //        {
        //            CustomWindow.COPYDATASTRUCT cds = (CustomWindow.COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(CustomWindow.COPYDATASTRUCT));
        //            if (cds.cbData > 0)
        //            {
        //                byte[] data = new byte[cds.cbData];
        //                Marshal.Copy(cds.lpData, data, 0, cds.cbData);
        //                Encoding unicodeStr = Encoding.ASCII;
        //                char[] myString = unicodeStr.GetChars(data);
        //                string returnText = new string(myString);

        //                textbox.AppendText(returnText);
        //                textbox.AppendText("\n");

        //            }
        //        }
        //    }
        //}
        
        private void Form1_Load(object sender, EventArgs e)
        {
            // receiver log
            CustomWindow.Handler wnd = delegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
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

            
            CustomWindow cw = new CustomWindow("TFormBSA", "Buster Sandbox Analyzer", wnd);
            

        }
    }
}
