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
    class OHxCToMtl_DateTimeSyncPH2 : PLC_FunBase
    {
        [PLCElement(ValueName = "OHXC_TO_MTL_DATE_TIME_SYNC_COMMAND_YEAR_PH2")]
        public uint Year;
        [PLCElement(ValueName = "OHXC_TO_MTL_DATE_TIME_SYNC_COMMAND_MONTH_PH2")]
        public uint Month;
        [PLCElement(ValueName = "OHXC_TO_MTL_DATE_TIME_SYNC_COMMAND_DAY_PH2")]
        public uint Day;
        [PLCElement(ValueName = "OHXC_TO_MTL_DATE_TIME_SYNC_COMMAND_HOUR_PH2")]
        public uint Hour;
        [PLCElement(ValueName = "OHXC_TO_MTL_DATE_TIME_SYNC_COMMAND_MINUTE_PH2")]
        public uint Min;
        [PLCElement(ValueName = "OHXC_TO_MTL_DATE_TIME_SYNC_COMMAND_SECOND_PH2")]
        public uint Sec;
        [PLCElement(ValueName = "OHXC_TO_MTL_DATE_TIME_SYNC_COMMAND_INDEX_PH2", IsIndexProp = true)]
        public uint Index;
    }


}
