using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AHMDS.Engine
{
    public class HybridAnalyzer : Analyzer
    {
        private static Queue<HybridObject> QueueStatic = new Queue<HybridObject>();  //queue yang digunakan untuk menampung antrian analisis statis
        private static bool isBusy = false; // agar tidak memakan banyak memory (mungkin dapat mengakibatkan masalah baru), hanya satu analisis statik yang dijalankan dalam satu waktu
        private static StaticAnalyzer staticAnalyzer = new StaticAnalyzer();

        public static void ProcessQueue()
        {
            if (!isBusy && QueueStatic.Count > 0)
            {
                QueueStatic.Dequeue().Analyze();
            }
        }

        public static void AddQueue(HybridObject obj)
        {
            //QueueStatic.Enqueue(obj);
            //ProcessQueue();
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

            public HybridObject (string image_address, ResultHandler resultHandler, StatusHandler statusHandler) : base(image_address, resultHandler, statusHandler)
            {
                this.status = NOT_STARTED;
            }

            public void Analyze()
            {
                isBusy = true;
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

                // jika program tersebut lolos dari tahap static analysis, lakukan dynamic
                updateStatus(DYNAMIC_INITIALIZED);

                isBusy = false; // izinkan analisis statik lainnya dimulai
                ProcessQueue();

                DynamicAnalyzer.DynamicObject dynamicObject = new DynamicAnalyzer.DynamicObject(image_address, dynamicFinished, dynamicProgressWatcher);
            }

            private void dynamicProgressWatcher(Analyzer.AnalyzedObject asender)
            {
                DynamicAnalyzer.DynamicObject sender = (DynamicAnalyzer.DynamicObject)asender;
                switch (sender.Status)
                {
                    case DynamicAnalyzer.DynamicObject.WAITING:
                        updateStatus(DYNAMIC_WAITING);
                        break;

                    case DynamicAnalyzer.DynamicObject.ANALYZING:
                        updateStatus(DYNAMIC_ANALYZING);
                        break;
                }
            }

            private void dynamicFinished(Analyzer.AnalyzedObject dsender, MalwareInfo result)
            {
                updateAndFinish(result);
            }

            private void updateAndFinish(MalwareInfo result)
            {
                updateFinish(result);
                updateStatus(FINISHED);
                ProcessQueue();
            }
        }
    }
}
