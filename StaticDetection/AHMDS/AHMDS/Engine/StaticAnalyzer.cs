using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AHMDS.avDataSetTableAdapters;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;

namespace AHMDS.Engine
{
    public class StaticAnalyzer
    {
        private const string PerlLocation = @"D:\Project\AHMDS\StaticDetection\perl\bin\perl.exe";

        private signatureTableAdapter SignatureTAdapter;
        private MD5 Md5;

        // fungsi inisiasi pendeteksian statis
        public StaticAnalyzer()
        {
            SignatureTAdapter = new signatureTableAdapter();
            Md5 = MD5.Create();
        }

        public MalwareInfo Check(string FileName)
        {
            // bagian untuk "memecah" file PE berdasarkan tiap sections yang dimiliki untuk dianalisis
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = PerlLocation;
            p.StartInfo.Arguments = "SectionExtractor.pl " + FileName; // menggunakan library SectionExtractor
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            string extractResult = p.StandardOutput.ReadToEnd(); // baca hasil proses library
            p.WaitForExit();


            if (!extractResult.Contains("AHMDS_ERROR_STATIC_PESectionExtractor_ENGINE:")) // jika library berhasil memproses file tersebut, maka proses hasil pecahan
            {
                string[] extractedFileNames = extractResult.Split('\n'); // file-file yang dihasilkan
                foreach (string extractedFile in extractedFileNames)
                {
                    String extractedTrimmed = extractedFile.Trim();
                    if (extractedTrimmed.Length == 0) continue;

                    MalwareInfo detect = CheckToDb(extractedTrimmed);
                    if (detect.ResultCode == MalwareInfo.POSITIVE)
                    {
                        CleanUp(extractedFileNames); // hapus file-file yang dihasilkan library
                        return detect;
                    }
                }
                CleanUp(extractedFileNames); // hapus file-file yang dihasilkan library
            }
            return CheckToDb(FileName);
        }

        public List<string> extractAPICalls(string FileName)
        {
            List<string> result = new List<string>();
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "hapi.dll";
            p.StartInfo.Arguments = FileName; // menggunakan library registry export

            p.StartInfo.CreateNoWindow = true;
            p.Start();
            string extractResult = p.StandardOutput.ReadToEnd(); // baca hasil proses library
            p.WaitForExit();

            string[] apiCalls = extractResult.Split('\n');

            foreach (string apiCall in apiCalls)
            {
                result.Add(apiCall.Trim());
            }

            return result;
        }

        private void CleanUp(string[] FileNames)
        {
            foreach (string fileName in FileNames)
            {
                try
                {
                    File.Delete(fileName.Trim());
                }
                catch (Exception x)
                {
                    // do not care ...
                }
            }
        }

        private MalwareInfo CheckToDb(string FileName)
        {
            FileStream file;

            try
            {
                file = File.OpenRead(FileName);
            }
            catch (Exception x) // jika file tidak ditemukan
            {
                return new MalwareInfo(MalwareInfo.NEGATIVE, "Path not found");
            }

            StringBuilder sb = new StringBuilder();
            avDataSet.signatureDataTable resultTable;

            byte[] hashbyte = Md5.ComputeHash(file);
            foreach (byte b in hashbyte) sb.Append(b.ToString("x2").ToLower());
            file.Close();

            resultTable = SignatureTAdapter.GetDataBy(sb.ToString());

            if (resultTable.Count > 0)
            {
                return new MalwareInfo(MalwareInfo.POSITIVE, "Detected as " + resultTable.Rows[0][1].ToString());
            }
            else
            {
                return new MalwareInfo(MalwareInfo.NEGATIVE, "Nothing found in database");
            }
        }
    }
}
