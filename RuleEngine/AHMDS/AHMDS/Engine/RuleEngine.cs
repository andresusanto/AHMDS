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

        private static void init()
        {
            String[] param = { "-q" };
            PlEngine.Initialize(param);
        }

        public static int CalculateAPICalls(string[] apiCalls)
        {
            init();
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
                foreach (PlQueryVariables v in q.SolutionVariables)
                    Console.WriteLine(v["X"].ToString());

            }

            PlEngine.PlCleanup();
            return 0;
        }

    }
}
