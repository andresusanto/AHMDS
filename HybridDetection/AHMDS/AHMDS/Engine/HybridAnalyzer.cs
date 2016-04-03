using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AHMDS.Engine
{
    public class HybridAnalyzer
    {

        // kelas dari objek yang sedang dianalisis
        public class HybridObject
        {
            public const int NOT_STARTED = 0;
            public const int VERIFYING = 1;
            public const int STATIC_ANALYZING = 2;
            public const int DYNAMIC_ANALYZING = 3;
            public const int FINISHED = 4;

            private int status;
            private string image_address = "";
            

        }
    }
}
