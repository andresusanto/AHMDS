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

        public RuleEngine()
        {
            String[] param = { "-q" };  // suppressing informational and banner messages
            //Environment.SetEnvironmentVariable("SWI_HOME_DIR", SWI_HOME_DIR);
            PlEngine.Initialize(param);
        }

        public void tes()
        {
            
            PlQuery.PlCall("assert(father(martin, inka))");
            PlQuery.PlCall("assert(father(uwe, gloria))");
            PlQuery.PlCall("assert(father(uwe, melanie))");
            PlQuery.PlCall("assert(father(uwe, ayala))");
            using (var q = new PlQuery("father(P, C), atomic_list_concat([P,' is_father_of ',C], L)"))
            {
                foreach (PlQueryVariables v in q.SolutionVariables)
                    Console.WriteLine(v["L"].ToString());

                Console.WriteLine("all children from uwe:");
                q.Variables["P"].Unify("uwe");
                foreach (PlQueryVariables v in q.SolutionVariables)
                    Console.WriteLine(v["C"].ToString());
            }
            PlEngine.PlCleanup();
            Console.WriteLine("finshed!");
        }
    }
}
