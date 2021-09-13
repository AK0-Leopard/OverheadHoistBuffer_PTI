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
    class MtsToOHxC_RequestCarInDataCheck : PLC_FunBase
    {
        public DateTime Timestamp;
        [PLCElement(ValueName = "MTS_TO_OHXC_REQUEST_CAR_IN_DATA_CHECK_MTS_STATION_ID")]
        public UInt16 MTSStationID;
        [PLCElement(ValueName = "MTS_TO_OHXC_REQUEST_CAR_IN_DATA_CHECK_CAR_ID")]
        public UInt16 CarID;
        [PLCElement(ValueName = "MTS_TO_OHXC_REQUEST_CAR_IN_DATA_CHECK_HS")]
        public UInt16 Handshake;
    }

    class OHxCToMts_ReplyCarInDataCheck : PLC_FunBase
    {
        public DateTime Timestamp;
        [PLCElement(ValueName = "OHXC_TO_MTS_REPLY_CAR_IN_DATA_CHECK_RETURN_CODE")]
        public UInt16 ReturnCode;
        [PLCElement(ValueName = "OHXC_TO_MTS_REPLY_CAR_IN_DATA_CHECK_HS",IsHandshakeProp = true)]
        public UInt16 Handshake;
    }

}
