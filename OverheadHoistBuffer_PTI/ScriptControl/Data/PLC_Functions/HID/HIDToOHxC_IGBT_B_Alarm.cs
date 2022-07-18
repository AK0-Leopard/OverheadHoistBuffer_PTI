using com.mirle.ibg3k0.sc.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.PLC_Functions
{
    public class HIDToOHxC_IGBT_B_Alarm : PLC_FunBase
    {
        public string EQ_ID;
        [PLCElement(ValueName = "HID_TO_OHXC_IGBT_B_ALARM")]
        public bool IGBT_B_AlarmHappend;
    }
}
