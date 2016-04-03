using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Specialized;

namespace AHMDS.Engine
{
    public class DynamicAnalyzer : Analyzer
    {
        private const int ANALYZE_DURATION = 10;
        private static string SBIE_BOX_LOC = Properties.Settings.Default.SandboxieBoxLocation; //@"C:\Sandbox\AHMDS\"; // must end with \ !!
        private static string SBIE_DLL_LOC = Properties.Settings.Default.SandboxieDllLocation; //@"C:\Program Files\Sandboxie\32\SbieDll.dll";
        private static string SBIE_START_LOC = Properties.Settings.Default.SandboxieExeLocation; //@"C:\Program Files\Sandboxie\Start.exe";
        private static StringCollection SBIE_BOX_NAMES = Properties.Settings.Default.SandboxNames; //string[] SBIE_BOX_NAMES = new string[3] { "Malware1", "Malware2", "Malware3" };

        private static Dictionary<string, bool> ACTIVE_SANDBOX = new Dictionary<string,bool>();
        private static Queue<DynamicObject> QueueAnalysis = new Queue<DynamicObject>();
        private static Sandboxie sbx = new Sandboxie(SBIE_DLL_LOC);

        
        private static Dictionary<string, AHMDSWindow> aWindow = new Dictionary<string, AHMDSWindow>() {
            {SBIE_BOX_NAMES[0], new AHMDSWindow(SBIE_BOX_NAMES[0])},
            {SBIE_BOX_NAMES[1], new AHMDSWindow(SBIE_BOX_NAMES[1])},
            {SBIE_BOX_NAMES[2], new AHMDSWindow(SBIE_BOX_NAMES[2])} 
        };

        
        public static void ProcessQueue()
        {
            foreach (string sandbox in SBIE_BOX_NAMES)
            {
                if ((!ACTIVE_SANDBOX.ContainsKey(sandbox) || !ACTIVE_SANDBOX[sandbox]) && QueueAnalysis.Count > 0)
                {
                    QueueAnalysis.Dequeue().Start(sandbox);
                    break;
                }
            }
        }

        public static void AddQueue(DynamicObject obj)
        {
            QueueAnalysis.Enqueue(obj);
            ProcessQueue();
        }
        
        // kelas untuk menangani objek yang dianalisis
        public class DynamicObject : AnalyzedObject
        {
            // bagian yang berkaitan dengan status engine
            public const int NOT_STARTED = 0;
            public const int WAITING = 1;
            public const int ANALYZING = 2;
            public const int FINISHED = 3;
            
            // hasil analisis
            private List<string> scannedDirectories;
            private List<string> scannedFiles;
            private List<string> apiCalls;
            private RegistryList registries;


            public DynamicObject(string image_address, ResultHandler resultHandler, StatusHandler statusHandler) : base(image_address, resultHandler, statusHandler)
            {
                this.status = NOT_STARTED;
            }

            public void Start(string box) // box diletakan disini karena proses Queue yang akan mengassign box
            {
                this.box = box;
                ACTIVE_SANDBOX[box] = true;

                // bagian cleanup sandbox sebelum digunakan
                sbx.KillAll(box);
                Process.Start(SBIE_START_LOC, "/nosbiectrl /silent /box:" + box + " delete_sandbox_silent exit 9").WaitForExit();

                this.apiCalls = new List<string>();
                aWindow[box].subscribeHandler(apiHandler);

                // bagian eksekusi program
                Process.Start(SBIE_START_LOC, "/nosbiectrl /hide_window /silent /elevate /box:" + box + " \"" + image_address + "\""); //: /hide_window

                analysisThread = new Thread(Analyzer);
                analysisThread.Start();
            }

            public void Terminate()
            {
                if (status == NOT_STARTED) return; // analisis belum dimulai

                analysisThread.Abort();
                sbx.KillAll(box);
                aWindow[box].unsubscribeHandler(apiHandler);
                updateStatus(FINISHED);
                ACTIVE_SANDBOX[box] = false;
                //ProcessQueue();
            }


            private void apiHandler(string apiCall)
            {
                string[] apiDetails = apiCall.Split('(');
                if (!this.apiCalls.Contains(apiDetails[0])) this.apiCalls.Add(apiDetails[0]);
            }

            private void Analyzer()
            {

                updateStatus(WAITING);
                Thread.Sleep(1000 * ANALYZE_DURATION);
                
                // tidak melakukan apapun sampai selesai waiting. berikan kesempatan malware beraksi.

                sbx.KillAll(box);
                aWindow[box].unsubscribeHandler(apiHandler);
                updateStatus(ANALYZING);

                this.Scan();
                this.DumpRegistries();
                MalwareInfo result = this.Analyze();

                updateFinish(result);
                updateStatus(FINISHED);
                ACTIVE_SANDBOX[box] = false;
                ProcessQueue();
            }

            private MalwareInfo Analyze()
            {
                // lakukan perhitungan dengan masing-masing rule
                RuleEngine.CalculationResult calculateAPI = RuleEngine.CalculateAPICalls(this.apiCalls.ToArray());
                RuleEngine.CalculationResult calculateREG  = RuleEngine.CalculateRegistries(this.registries);
                RuleEngine.CalculationResult calculateFile = RuleEngine.CalculateFiles(this.scannedFiles);

                // gabungkan hasil perhitungan beserta penjelasannya
                int totalScore = calculateREG.Score + calculateFile.Score;
                //int totalScore = calculateAPI.Score + calculateREG.Score + calculateFile.Score;
                List<string> explanation = new List<string>();

                //explanation.AddRange(calculateAPI.Explanation);
                explanation.AddRange(calculateREG.Explanation);
                explanation.AddRange(calculateFile.Explanation);

                // kembalikan hasil
                if (totalScore > Properties.Settings.Default.MalwareScoreThreshold)
                {
                    return new MalwareInfo(MalwareInfo.POSITIVE, "Program contains Malicious Behaviors", totalScore, explanation);
                }
                else
                {
                    return new MalwareInfo(MalwareInfo.NEGATIVE, "Program threat level is below Malware Threshold", totalScore, explanation);
                }
            }

            // scan subdirs and files
            private void Scan()
            {
                scannedDirectories = new List<string>();
                scannedFiles = new List<string>();

                string alamat = SBIE_BOX_LOC + box;
                Queue<string> tmpScan = new Queue<string>(Directory.GetDirectories(alamat));

                scannedDirectories.AddRange(tmpScan);
                //scannedFiles.AddRange(Directory.GetFiles(alamat)); // file-file yang ada di root folder sandbox, tidak perlu dianalisis karena milik sandbox

                while (tmpScan.Count > 0)
                {
                    string currentDir = tmpScan.Dequeue();
                    string[] subDirs = Directory.GetDirectories(currentDir);

                    foreach (string subDir in subDirs)
                        tmpScan.Enqueue(subDir);

                    scannedDirectories.AddRange(subDirs);
                    scannedFiles.AddRange(Directory.GetFiles(currentDir));
                }

                // hilangkan alamat sandbox
                scannedDirectories = scannedDirectories.Select(s => s.Substring(alamat.Length + 1)).ToList();
                scannedFiles = scannedFiles.Select(s => s.Substring(alamat.Length + 1)).ToList();
            }

            private void DumpRegistries()
            {
                this.registries = new RegistryList();

                // copy file RegHive
                bool fileLocked = true;
                while (fileLocked)
                {
                    try
                    {
                        File.Copy(SBIE_BOX_LOC + box + "\\RegHive", box + "_hive", true);
                        fileLocked = false;
                    }
                    catch
                    {
                        Thread.Sleep(200); // file masih dikunci (karena masih ditulis oleh Sandboxie)
                    }
                }


                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "Libs\\regexport.dll";
                p.StartInfo.Arguments = box + "_hive tmphive_" + box; // menggunakan library registry export
                
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                string extractResult = p.StandardOutput.ReadToEnd(); // baca hasil proses library
                p.WaitForExit();


                if (extractResult.Trim().Equals(""))
                {
                    string line = null;
                    StreamReader regFile = new StreamReader("tmphive_" + box);

                    string keyName = "";
                    List<string> keyContents = null;
                    StringBuilder tmpContent = new StringBuilder();
                    
                    while ((line = regFile.ReadLine()) != null)
                    {
                        if (keyName.Equals(""))
                        {
                            Regex rgx = new Regex(@"\[(.+)\]", RegexOptions.IgnoreCase);
                            Match match = rgx.Match(line);

                            if (match.Success)
                            {
                                GroupCollection groups = match.Groups;
                                keyName = groups[1].Value;
                                keyContents = new List<string>();
                            }
                        }
                        else
                        {
                            if (line.EndsWith(@"\"))  // registry cukup panjang (biasanya hex)
                            {
                                tmpContent.Append(line.Remove(line.Length - 1)); // hilangkan \
                            }
                            else if (tmpContent.Length > 0)
                            {
                                tmpContent.Append(line);
                                keyContents.Add(tmpContent.ToString());
                                tmpContent.Clear();
                            }
                            else
                            {
                                if (line.Trim().Equals("")) // spacing antar key dan value, mengindikasian sudah tidak ada value lagi pada key tsb
                                {
                                    try
                                    {
                                        this.registries.Add(keyName, keyContents);
                                    }
                                    catch
                                    {
                                        // terdapat key yang sama, seharusnya tidak terlalu penting
                                    }
                                    keyName = "";
                                }
                                else
                                {
                                    keyContents.Add(line);
                                }
                            }

                        }
                    }
                    regFile.Close();
                }
                else
                {
                    Console.Out.WriteLine(extractResult);
                    throw new Exception("Fail to dump Registry Hive file. Malware Box: " + box); // gagal dump reghive
                }

            }
        }
    }
}
