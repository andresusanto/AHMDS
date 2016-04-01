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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RuleEngine.CalculationResult result = RuleEngine.CalculateAPICalls(new string[2] { "GetAsyncKeyState", "GetKeyState" });
            Console.Write("Skor: ");
            Console.WriteLine(result.Score);

            Console.WriteLine("Penjelasan :");

            foreach (String s in result.Explanation)
                Console.WriteLine(s);

            Dictionary<string, List<string>> registry = new Dictionary<string,List<string>>();

            List<string> entry = new List<string>();
            entry.Add("zXaxhIpd");

            registry.Add(@"user\current\software\microsoft\windows\currentversion\run", entry);


            entry = new List<string>();
            entry.Add("startup");
            entry.Add("common startup");

            registry.Add(@"user\current\software\microsoft\windows\currentversion\explorer\shell folders", entry);
            registry.Add(@"machine\software\microsoft\active setup\installed components\{123123132-123123}\component\run", entry);

            RuleEngine.CalculationResult resReg = RuleEngine.CalculateRegistries(registry);

            Console.Write("Skor: ");
            Console.WriteLine(resReg.Score);

            Console.WriteLine("Penjelasan :");

            foreach (String s in resReg.Explanation)
                Console.WriteLine(s);
        }
    }
}
