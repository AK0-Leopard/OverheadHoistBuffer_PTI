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
    public class OHxCToMts_AlarmResetRequest : PLC_FunBase
    {
        [PLCElement(ValueName = "OHXC_TO_MTS_ALARM_RESET_REQUEST_HS", IsHandshakeProp = true)]
        public UInt16 Handshake;
    }
    public class MtsToOHxC_AlarmResetReply : PLC_FunBase
    {
        [PLCElement(ValueName = "MTS_TO_OHXC_REPLY_ALARM_RESET_HS", IsHandshakeProp = true)]
        public UInt16 Handshake;
    }

}
