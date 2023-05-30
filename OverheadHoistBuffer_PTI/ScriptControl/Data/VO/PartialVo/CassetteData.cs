using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc
{
    public partial class CassetteData
    {
        public static ConcurrentDictionary<string, Stopwatch> RetryDeleteStopwatch { get; private set; } = new ConcurrentDictionary<string, Stopwatch>();

        public const string CassetteData_UNKNOWN_BOOKING_SCAN = "S";
        public const string CassetteData_NORMAL = "";

        public enum OHCV_STAGE
        {
            OHTtoPort = 0,  //入料進行中
            OP = 1,
            BP1,
            BP2,
            BP3,
            BP4,
            BP5,
            LP,
        }

        public CassetteData Clone()
        {
            return (CassetteData)this.MemberwiseClone();
        }

        public bool AtShelf { get => Int32.TryParse(Carrier_LOC, out var _) ? true : false; }
    }
}
