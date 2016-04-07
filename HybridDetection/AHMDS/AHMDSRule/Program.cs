using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AHMDSRule.Engine;

namespace AHMDSRule
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> apiCalls = new List<string>();
            string line;
            
            while ((line = Console.ReadLine()) != "")
            {
                apiCalls.Add(line);
            }

            AHMDSRule.Engine.RuleEngine.CalculationResult result = RuleEngine.CalculateAPICalls(apiCalls.ToArray());
            
            Console.WriteLine(result.Score);
            foreach (string explanation in result.Explanation)
                Console.WriteLine(explanation);
        }
    }
}
