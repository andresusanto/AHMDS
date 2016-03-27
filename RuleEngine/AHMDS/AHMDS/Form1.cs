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
        }
    }
}
