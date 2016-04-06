using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AHMDS.Engine
{
    public class HybridAnalyzer : Analyzer
    {
        private static readonly object processLock = new object();
        private static readonly object startLock = new object();

        private static Queue<HybridObject> QueueStatic = new Queue<HybridObject>();  //queue yang digunakan untuk menampung antrian analisis statis
        private static bool isBusy = false; // agar tidak memakan banyak memory (mungkin dapat mengakibatkan masalah baru), hanya satu analisis statik yang dijalankan dalam satu waktu
        private static StaticAnalyzer staticAnalyzer = new StaticAnalyzer();

        public static void ProcessQueue()
        {
            lock (processLock)
            {
                if (!isBusy && QueueStatic.Count > 0)
                {
                    QueueStatic.Dequeue().Start();
                }
            }
        }

        public static void AddQueue(HybridObject obj)
        {
            QueueStatic.Enqueue(obj);
            ProcessQueue();
        }

        public static void ClearQueue()
        {
            QueueStatic.Clear();
        }

        // kelas dari objek yang sedang dianalisis
        public class HybridObject : AnalyzedObject
        {
            public const int NOT_STARTED = 0;
            public const int VERIFYING = 1;
            public const int STATIC_ANALYZING = 2;
            public const int DYNAMIC_INITIALIZED = 3;
            public const int DYNAMIC_WAITING = 4;
            public const int DYNAMIC_ANALYZING = 5;
            public const int FINISHED = 6;

            private DynamicAnalyzer.DynamicObject dynamicObject;
            private List<string> staticExplanations;
            private int staticScore;

            public HybridObject(string image_address, ResultHandler resultHandler, StatusHandler statusHandler)
                : base(image_address, resultHandler, statusHandler)
            {
                this.status = NOT_STARTED;
            }

            public void Start()
            {
                lock (startLock)
                {
                    isBusy = true;

                    analysisThread = new Thread(Analyzer);
                    analysisThread.Start();
                }
            }

            private void Analyzer()
            {
                MalwareInfo result;

                // pertama lakukan proses verifikasi
                updateStatus(VERIFYING);
                if (staticAnalyzer.Verify(image_address))
                {
                    // program adalah program yang terpercaya
                    result = new MalwareInfo(MalwareInfo.TRUSTED, "This program is made by trusted vendor", 0, null);
                    updateAndFinish(result);
                    return; // proses analisis selesai.
                }

                // jika program tersebut bukan terverifikasi, lakukan analisis statis
                updateStatus(STATIC_ANALYZING);
                result = staticAnalyzer.Check(image_address);
                if (result.ResultCode == MalwareInfo.POSITIVE)
                {
                    updateAndFinish(result);
                    return; // proses analisis selesai
                }
                staticExplanations = result.Explanation;
                staticScore = result.Score;

                // jika program tersebut lolos dari tahap static analysis, lakukan dynamic
                updateStatus(DYNAMIC_INITIALIZED);

                updateAndFinish(result);

                //isBusy = false; // izinkan analisis statik lainnya dimulai
                //ProcessQueue();

                //dynamicObject = new DynamicAnalyzer.DynamicObject(image_address, dynamicFinished, dynamicProgressWatcher);
                //DynamicAnalyzer.AddQueue(dynamicObject);
            }

            public void Terminate()
            {
                if (status == NOT_STARTED) return; // analisis belum dimulai

                if (dynamicObject != null) dynamicObject.Terminate();

                analysisThread.Abort();
                isBusy = false;


                //ProcessQueue();
            }

            private void dynamicProgressWatcher(Analyzer.AnalyzedObject asender)
            {
                DynamicAnalyzer.DynamicObject sender = (DynamicAnalyzer.DynamicObject)asender;
                switch (sender.Status)
                {
                    case DynamicAnalyzer.DynamicObject.WAITING:
                        this.box = sender.Box;
                        updateStatus(DYNAMIC_WAITING);
                        break;

                    case DynamicAnalyzer.DynamicObject.ANALYZING:
                        updateStatus(DYNAMIC_ANALYZING);
                        break;
                }
            }

            private void dynamicFinished(Analyzer.AnalyzedObject dsender, MalwareInfo result)
            {
                result.Score += staticScore;
                result.Explanation.AddRange(staticExplanations);
                //updateAndFinish(result);
                updateFinish(result);
                updateStatus(FINISHED);
            }

            private void updateAndFinish(MalwareInfo result)
            {
                updateFinish(result);
                updateStatus(FINISHED);
                isBusy = false;
                ProcessQueue();
            }
        }
    }
}
