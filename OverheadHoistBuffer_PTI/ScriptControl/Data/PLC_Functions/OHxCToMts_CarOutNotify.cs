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
    class OHxCToMts_CarOutNotify : PLC_FunBase
    {
        [PLCElement(ValueName = "OHXC_TO_MTS_CAR_OUT_NOTIFY_CAR_ID")]
        public UInt16 CarID;
        [PLCElement(ValueName = "OHXC_TO_MTS_CAR_OUT_NOTIFY_HS", IsHandshakeProp = true)]
        public UInt16 Handshake;
    }
    class MtsToOHxC_ReplyCarOutNotify : PLC_FunBase
    {
        [PLCElement(ValueName = "MTS_TO_OHXC_REPLY_OHXC_CAR_OUT_NOTIFY_RETURN_CODE")]
        public UInt16 ReturnCode;
        [PLCElement(ValueName = "MTS_TO_OHXC_REPLY_OHXC_CAR_OUT_NOTIFY_HS", IsHandshakeProp = true)]
        public UInt16 Handshake;
    }

}
