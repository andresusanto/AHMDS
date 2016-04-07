using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace AHMDS.Engine
{
    class RuleEngine
    {
        private static readonly object prologLock = new object();
        private static string SWI_HOME_DIR = Properties.Settings.Default.SWIPath;

        private static Analyzer.RegistryList startupRegistries; // menyimpan daftar registry yang biasa digunakan oleh malware untuk startup
        private static Analyzer.RegistryList suspiciousRegistries; // meyimpan daftar registry yang biasa digunakan oleh malware untuk melakukan kegiatan malicious. elemen List<string> [ count ] adalah penjelasan
        private static List<FilesystemRule> suspiciousFiles;

        private static void initRegistries()
        {
            if (startupRegistries == null)
            {
                // baca registry registry startup malware
                startupRegistries = new Analyzer.RegistryList();
                StreamReader reader = new StreamReader("Rules\\RegistriesStart.txt");
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
                suspiciousRegistries = new Analyzer.RegistryList();
                reader = new StreamReader("Rules\\RegistriesSuspicious.txt");
                do
                {
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
                } while (reader.Peek() != -1);
                reader.Close();
            }
        }

        private static void initFiles()
        {
            if (suspiciousFiles == null)
            {
                suspiciousFiles = new List<FilesystemRule>();
                StreamReader reader = new StreamReader("Rules\\FilesystemRules.txt");
                do
                {
                    string[] content = reader.ReadLine().Split(',');
                    suspiciousFiles.Add(new FilesystemRule(Int32.Parse(content[0]), content[1], content[2]));
                } while (reader.Peek() != -1);
                reader.Close();
            }
        }

        private static List<string> generateRelevantKeys(Dictionary<string, List<string>> list, string key)
        {
            List<string> relevant = new List<string>();

            foreach (KeyValuePair<string, List<string>> entry in list)
            {
                string cKey = entry.Key.ToLower();
                if (cKey.Equals(key))
                    relevant.Add(entry.Key);
                else if (key.EndsWith("\\") && cKey.StartsWith(key))
                    relevant.Add(entry.Key);
                
            }

            return relevant;
        }

        private class FilesystemRule
        {
            public int score;
            public string pattern;
            public string explanation;

            public FilesystemRule(int score, string explanation, string pattern)
            {
                this.score = score;
                this.explanation = explanation;
                this.pattern = pattern;
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
            lock (prologLock)
            {
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = @"APIRule.dll";
                p.StartInfo.CreateNoWindow = true;
                p.Start();

                foreach (string apiCall in apiCalls)
                    p.StandardInput.WriteLine(apiCall.Trim());
                
                p.StandardInput.WriteLine("");
                string extractResult = p.StandardOutput.ReadToEnd(); // baca hasil proses library
                p.WaitForExit();

                string[] result = extractResult.Split('\n');
                List<string> explanation = new List<string>();
                for (int i = 1; i < result.Length; i++)
                    explanation.Add(result[i]);

                return new CalculationResult(Int32.Parse(result[0]), explanation);
            }
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
                        result.Score += 400;
                        result.Explanation.Add("Startup registry detected at " + reg);
                    }
                    else
                    {
                        foreach (string key in entry.Value)
                        {
                            if (registries[reg].Contains(key))
                            {
                                result.Score += 400;
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
                        result.Score += 450;
                        result.Explanation.Add(entry.Value[0]);
                    }
                    else
                    {
                        for (int i = 0; i < entry.Value.Count - 1; i++)
                        {
                            if (registries[reg].Contains(entry.Value[i]))
                            {
                                result.Score += 450;
                                result.Explanation.Add(entry.Value[entry.Value.Count - 1] + " (val:" + entry.Value[i] + ")");
                            }
                        }
                    }

                }
            }
            return result;
        }

        public static CalculationResult CalculateFiles(List<string> fileNames)
        {
            CalculationResult result = new CalculationResult(0, new List<string>());
            initFiles();

            foreach (string fileName in fileNames)
            {
                foreach (FilesystemRule rule in suspiciousFiles)
                {
                    if (new Regex(rule.pattern, RegexOptions.IgnoreCase).IsMatch(fileName))
                    {
                        result.Score += rule.score;
                        result.Explanation.Add(rule.explanation + " (Found at: " + fileName + ")");
                    }
                }
            }

            return result;
        }

    }
}
