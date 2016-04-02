using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SbsSW.SwiPlCs;
using System.IO;

namespace AHMDS.Engine
{
    class RuleEngine
    {
        private const string SWI_HOME_DIR = @"C:\Program Files\swipl";
        private const string API_RULES_NAME = @"APICallRules.pl";
        private const string REG_START_RULES_NAME = @"RegistriesStart.txt";
        private const string REG_SUSPICIOUS_RULES_NAME = @"RegistriesSuspicious.txt";

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
                // baca registry registry startup malware
                startupRegistries = new Dictionary<string, List<string>>();
                StreamReader reader = new StreamReader(REG_START_RULES_NAME);
                do
                {
                    string[] content = reader.ReadLine().Split(',');

                    if (content.Length == 1)
                    {
                        startupRegistries.Add(content[0], null);
                    }
                    else
                    {
                        List<string> keyList = new List<string>();
                        for (int i = 1; i < content.Length; i++)
                            keyList.Add(content[i]);

                        startupRegistries.Add(content[0], keyList);
                    }
                    
                } while (reader.Peek() != -1);
                reader.Close();


                // baca registry-registry yang mencurigakan
                suspiciousRegistries = new Dictionary<string, List<string>>();
                reader = new StreamReader(REG_SUSPICIOUS_RULES_NAME);
                do{
                    string[] content = reader.ReadLine().Split(',');
                    List<string> keyList = new List<string>();
                    
                    if (content.Length == 2)
                    {
                        keyList.Add(content[0]);
                        suspiciousRegistries.Add(content[1], keyList);
                    }
                    else
                    {
                        for (int i = 2; i < content.Length; i++)
                            keyList.Add(content[i]);

                        keyList.Add(content[0]);
                        suspiciousRegistries.Add(content[1], keyList);
                    }
                }while(reader.Peek() != -1);
                reader.Close();
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
            Console.WriteLine(PlQuery.PlCall("consult('" + API_RULES_NAME + "')"));

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

        private static List<string> generateRelevantKeys(Dictionary<string, List<string>> list, string key)
        {
            List<string> relevant = new List<string>();

            foreach (KeyValuePair<string, List<string>> entry in list)
            {
                if (entry.Key.Equals(key)) 
                    relevant.Add(entry.Key);
                else if (key.EndsWith("\\") && entry.Key.StartsWith(key))
                    relevant.Add(entry.Key);
            }

            return relevant;
        }

        public static CalculationResult CalculateRegistries(Dictionary<string, List<string>> registries)
        {
            CalculationResult result = new CalculationResult(0, new List<string>());
            initRegistries();

            // melakukan perhitungan terhadap registry-registry startup malware
            foreach (KeyValuePair<string, List<string>> entry in startupRegistries)
            {
                List<string> relevant = generateRelevantKeys(registries, entry.Key);

                foreach (string reg in relevant)
                {
                    if (entry.Value == null)
                    {
                        result.Score += 100;
                        result.Explanation.Add("Startup registry detected at " + reg);
                    }
                    else
                    {
                        foreach (string key in entry.Value)
                        {
                            if (registries[reg].Contains(key))
                            {
                                result.Score += 100;
                                result.Explanation.Add("Startup registry detected at " + reg + ", " + key);
                            }
                        }
                    }
                }
            }

            // melakukan perhitungan terhadap registry-registry mencurigakan malware
            foreach (KeyValuePair<string, List<string>> entry in suspiciousRegistries)
            {
                List<string> relevant = generateRelevantKeys(registries, entry.Key);

                foreach (string reg in relevant)
                {
                    if (entry.Value.Count == 1)
                    {
                        result.Score += 150;
                        result.Explanation.Add(entry.Value[0]);
                    }
                    else
                    {
                        for (int i = 0; i < entry.Value.Count - 1; i++)
                        {
                            if (registries[reg].Contains(entry.Value[i]))
                            {
                                result.Score += 150;
                                result.Explanation.Add(entry.Value[entry.Value.Count - 1] + " (val:" + entry.Value[i] + ")");
                            }
                        }
                    }
                    
                }
            }
            return result;
        }

    }
}
