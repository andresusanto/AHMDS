using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AHMDS.GUI;
using System.IO;

namespace AHMDS
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Directory.SetCurrentDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
            MessageBox.Show(Directory.GetCurrentDirectory());
            if (args.Length > 0)
            {
                Application.Run(new FormSingleAnalyzer(args[0]));
            }
            else
            {
                Application.Run(new FormBatchAnalyzer());
            }
            
        }
    }
}
