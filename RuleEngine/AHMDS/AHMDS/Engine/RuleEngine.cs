using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SbsSW.SwiPlCs;

namespace AHMDS.Engine
{
    class RuleEngine
    {
        private const string SWI_HOME_DIR = @"C:\Program Files\swipl";
        private static Dictionary<string, List<string>> startupRegistries; // menyimpan daftar registry yang biasa digunakan oleh malware untuk startup
        private static Dictionary<string, List<string>> suspiciousRegistries; // meyimpan daftar registry yang biasa digunakan oleh malware untuk melakukan kegiatan malicious. elemen List<string> [ count ] adalah penjelasan

        private static void initAPI()
        {
            String[] param = { "-q" };
            PlEngine.Initialize(param);
        }

        private static void initRegistries()
        {
            if (startupRegistries == null)
            {

            }
        }

        public class CalculationResult
        {
            public int Score;
            public List<String> Explanation;

            public CalculationResult(int score, List<string> explanation)
            {
                this.Score = score;
                this.Explanation = explanation;
            }
        }

        public static CalculationResult CalculateAPICalls(string[] apiCalls)
        {
            int score = 0;
            List<string> explanation = new List<string>();

            initAPI();
            StringBuilder sb = new StringBuilder();
            Console.WriteLine(PlQuery.PlCall("consult('APICallRules.pl')"));

            PlTerm listApi = PlTerm.PlVar();
            PlTerm tailApi = PlTerm.PlTail(listApi);

            foreach (string api in apiCalls)
            {
                sb.Append('"'); sb.Append(api); sb.Append('"');
                tailApi.Append(PlTerm.PlString(sb.ToString()));
                sb.Clear();
            }
            tailApi.Close();

            sb.Append("score("); sb.Append(listApi.ToString()); sb.Append(", X)");

            using (var q = new PlQuery(sb.ToString()))
            {
                score = Int32.Parse(q.SolutionVariables.First()["X"].ToString());
            }

            using (var q = new PlQuery("explanation(X)"))
            {
                explanation.AddRange(q.SolutionVariables.First()["X"].ToListString());
            }

            PlEngine.PlCleanup();
            return new CalculationResult(score, explanation);
        }

        public static CalculationResult CalculateRegistries(Dictionary<string, List<string>> registries)
        {

        }

    }
}
