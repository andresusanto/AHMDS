using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AHMDS.DB.ahmdsDataSetTableAdapters;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace AHMDS.Engine
{
    public class StaticAnalyzer
    {
        private static readonly object dbLock = new object();
        private signatureTableAdapter SignatureTAdapter;
        private verifiedTableAdapter VerifiedTAdapter;
        private MD5 Md5;

        // fungsi inisiasi pendeteksian statis
        public StaticAnalyzer()
        {
            SignatureTAdapter = new signatureTableAdapter();
            VerifiedTAdapter = new verifiedTableAdapter();
            Md5 = MD5.Create();
        }

        public bool Verify(string fileName)
        {
            X509Certificate cert = WinTrust.GetVerifiedCert(fileName);

            if (cert != null)
            {
                AHMDS.DB.ahmdsDataSet.verifiedDataTable result = VerifiedTAdapter.GetDataBySubjectIssuer(cert.Subject, cert.Issuer);
                if (result.Count > 0) return true;
            }

            return false;
        }

        public MalwareInfo Check(string FileName)
        {
            MalwareInfo dbResult = CheckToDb(FileName);
            if (dbResult.ResultCode == MalwareInfo.POSITIVE) return dbResult; // jika terdeteksi pada database langsung kembalikan

            // bagian untuk "memecah" file PE berdasarkan tiap sections yang dimiliki untuk dianalisis
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = Properties.Settings.Default.PerlLocation;
            p.StartInfo.Arguments = "Libs\\SE.dll " + FileName; // menggunakan library SectionExtractor
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

            return new MalwareInfo(MalwareInfo.NEGATIVE, "No known malicious code detected. Program's API Calls threat is under specified threshold.", 0, null);


            // lakukan pemeriksaan terhadap sekuens API Call program
            //List<string> apiCalls = extractAPICalls(FileName);
            //RuleEngine.CalculationResult apiResult = RuleEngine.CalculateAPICalls(apiCalls.ToArray());

            //if (apiResult.Score > Properties.Settings.Default.APICallScoreThreshold)
            //{
            //    return new MalwareInfo(MalwareInfo.POSITIVE, "Malicious API Calls detected", apiResult.Score, apiResult.Explanation);
            //}
            //else
            //{
            //    return new MalwareInfo(MalwareInfo.NEGATIVE, "No known malicious code detected. Program's API Calls threat is under specified threshold.", apiResult.Score, apiResult.Explanation);
            //}
        }

        private List<string> extractAPICalls(string FileName)
        {
            List<string> result = new List<string>();
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "Libs\\hapi.dll";
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
                catch
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
            catch  // jika file tidak ditemukan
            {
                return new MalwareInfo(MalwareInfo.NEGATIVE, "Path not found", 0, null);
            }

            StringBuilder sb = new StringBuilder();
            AHMDS.DB.ahmdsDataSet.signatureDataTable resultTable;

            lock (dbLock)
            {
                byte[] hashbyte = Md5.ComputeHash(file);
                foreach (byte b in hashbyte) sb.Append(b.ToString("x2").ToLower());
                file.Close();

                string tes = sb.ToString();
                try
                {
                    resultTable = SignatureTAdapter.GetDataByHash(tes);

                    if (resultTable.Count > 0)
                    {
                        return new MalwareInfo(MalwareInfo.POSITIVE, "Detected as " + resultTable.Rows[0][1].ToString(), 999, null);
                    }
                    else
                    {
                        return new MalwareInfo(MalwareInfo.NEGATIVE, "Nothing found in database", 0, null);
                    }
                }
                catch
                {
                    return new MalwareInfo(MalwareInfo.NEGATIVE, "Nothing found in database (N).", 0, null);
                }
            }
        }
    }
}
