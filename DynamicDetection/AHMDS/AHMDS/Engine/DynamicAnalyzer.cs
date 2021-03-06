﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace AHMDS.Engine
{
    class DynamicAnalyzer
    {
        private const int ANALYZE_DURATION = 300;
        private const string SBIE_BOX_LOC = @"C:\Sandbox\AHMDS\"; // must end with \ !!
        private const string SBIE_DLL_LOC = @"C:\Program Files\Sandboxie\32\SbieDll.dll";
        private const string SBIE_START_LOC = @"C:\Program Files\Sandboxie\Start.exe";
        private static string[] SBIE_BOX_NAMES = new string[3] { "Malware1", "Malware2", "Malware3" };

        private static Dictionary<string, bool> ACTIVE_SANDBOX = new Dictionary<string,bool>();
        private static Queue<DynamicObject> QueueAnalysis = new Queue<DynamicObject>();
        private static Sandboxie sbx = new Sandboxie(SBIE_DLL_LOC);

        public delegate void ResultHandler(DynamicObject sender, MalwareInfo result);
        public delegate void StatusHandler(DynamicObject sender);

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
                    DynamicObject obj = QueueAnalysis.Dequeue();
                    obj.Start(sandbox);
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
        public class DynamicObject
        {
            // bagian yang berkaitan dengan status engine
            public const int NOT_STARTED = 0;
            public const int WAITING = 1;
            public const int ANALYZING = 2;
            public const int FINISHED = 3;
            
            private int status;
            private string image_address = "";
            private string box;
            private event ResultHandler OnFinished;
            private event StatusHandler OnStatusChanged;
            private Thread analysisThread;

            // hasil analisis
            private List<string> scannedDirectories;
            private List<string> scannedFiles;
            private List<string> apiCalls;
            private Dictionary<string, List<string>> registries;

            public Object storage; // storage yang dapat digunakan untuk menyimpan referensi (untuk update GUI).

            public string Box
            {
                get { return box; }
            }

            public int Status
            {
                get { return status; }
            }

            public DynamicObject(string image_address, ResultHandler resultHandler, StatusHandler statusHandler)
            {
                this.image_address = image_address;
                this.status = NOT_STARTED;
                this.OnFinished += resultHandler;
                this.OnStatusChanged += statusHandler;
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
                analysisThread.Abort();
                sbx.KillAll(box);
                aWindow[box].unsubscribeHandler(apiHandler);
                updateStatus(FINISHED);
                ACTIVE_SANDBOX[box] = false;
                ProcessQueue();
            }

            private void updateStatus(int status)
            {
                this.status = status;
                OnStatusChanged(this);
            }

            private void apiHandler(string apiCall)
            {
                this.apiCalls.Add(apiCall);
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
                
                OnFinished(this, result);
                updateStatus(FINISHED);
                ACTIVE_SANDBOX[box] = false;
                ProcessQueue();
            }

            private MalwareInfo Analyze()
            {
                // TODO: perform analysis
                string folderAddress = Path.GetDirectoryName(this.image_address) + @"\Report of " + Path.GetFileName(this.image_address);
                if (! Directory.Exists(folderAddress)) Directory.CreateDirectory(folderAddress);

                File.WriteAllLines(folderAddress + @"\apicalls.txt", this.apiCalls);
                File.WriteAllLines(folderAddress + @"\directories.txt", this.scannedDirectories);
                File.WriteAllLines(folderAddress + @"\files.txt", this.scannedFiles);

                using (StreamWriter file = new StreamWriter(folderAddress + @"\registries.txt"))
                {
                    foreach (KeyValuePair<string, List<string>> entry in this.registries)
                    {
                        file.Write('[');
                        file.Write(entry.Key);
                        file.WriteLine(']');
                        
                        foreach (string konten in entry.Value)
                        {
                            file.WriteLine(konten);
                        }

                        file.WriteLine();
                    }
                }

                
                return new MalwareInfo(MalwareInfo.NEGATIVE, "Program doesn't contain malicious behaviour.");
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
                this.registries = new Dictionary<string, List<string>>();

                // copy file RegHive
                bool fileLocked = true;
                while (fileLocked)
                {
                    try
                    {
                        File.Copy(SBIE_BOX_LOC + box + "\\RegHive", box + "_hive", true);
                        fileLocked = false;
                    }
                    catch (Exception x)
                    {
                        Thread.Sleep(200); // file masih dikunci (karena masih ditulis oleh Sandboxie)
                    }
                }


                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "regexport.dll";
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
                                    catch (ArgumentException x)
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
