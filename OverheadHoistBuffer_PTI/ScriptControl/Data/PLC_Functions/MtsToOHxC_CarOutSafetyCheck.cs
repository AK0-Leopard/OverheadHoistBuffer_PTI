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
    class MtsToOHxC_CarOutSafetyCheck : PLC_FunBase
    {
        [PLCElement(ValueName = "MTS_TO_OHXC_U2D_SAFETY_CHECK")]
        public bool SafetyCheck;
    }


}
