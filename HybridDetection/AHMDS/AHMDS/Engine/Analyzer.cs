using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AHMDS.Engine
{
    public class Analyzer
    {
        public delegate void ResultHandler(AnalyzedObject sender, MalwareInfo result);
        public delegate void StatusHandler(AnalyzedObject sender);

        public class AnalyzedObject
        {
            protected int status;
            protected string image_address = "";

            protected event ResultHandler OnFinished;
            protected event StatusHandler OnStatusChanged;

            public Object storage; // storage yang dapat digunakan untuk menyimpan referensi (untuk update GUI).

            protected void updateStatus(int status)
            {
                this.status = status;
                OnStatusChanged(this);
            }

            protected void updateFinish(MalwareInfo result)
            {
                OnFinished(this, result);
            }
        }
    }
}
