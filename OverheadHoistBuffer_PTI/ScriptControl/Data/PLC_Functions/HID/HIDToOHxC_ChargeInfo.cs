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
    public class HIDToOHxC_ChargeInfo : PLC_FunBase
    {
        public DateTime Timestamp;

        [PLCElement(ValueName = "HID_TO_OHXC_HID_ID")]
        public UInt16 HID_ID;
        [PLCElement(ValueName = "HID_TO_OHXC_V_UNIT")]
        public UInt16 V_Unit;
        [PLCElement(ValueName = "HID_TO_OHXC_V_DOT")]
        public UInt16 V_Dot;
        [PLCElement(ValueName = "HID_TO_OHXC_A_UNIT")]
        public UInt16 A_Unit;
        [PLCElement(ValueName = "HID_TO_OHXC_A_DOT")]
        public UInt16 A_Dot;
        [PLCElement(ValueName = "HID_TO_OHXC_W_UNIT")]
        public UInt16 W_Unit;
        [PLCElement(ValueName = "HID_TO_OHXC_W_DOT")]
        public UInt16 W_Dot;
        [PLCElement(ValueName = "HID_TO_OHXC_HOUR_UNIT")]
        public UInt16 Hour_Unit;
        [PLCElement(ValueName = "HID_TO_OHXC_HOUR_DOT")]
        public UInt16 Hour_Dot;
        [PLCElement(ValueName = "HID_TO_OHXC_HOUR_SIGMA_Hi_WORD")]
        public UInt16 Hour_Sigma_High_Word;
        [PLCElement(ValueName = "HID_TO_OHXC_HOUR_SIGMA_Lo_WORD")]
        public UInt16 Hour_Sigma_Low_Word;
        [PLCElement(ValueName = "HID_TO_OHXC_HOUR_POSITIVE_Hi_WORD")]
        public UInt16 Hour_Positive_High_Word;
        [PLCElement(ValueName = "HID_TO_OHXC_HOUR_POSITIVE_Lo_WORD")]
        public UInt16 Hour_Positive_Low_Word;
        [PLCElement(ValueName = "HID_TO_OHXC_HOUR_NEGATIVE_Hi_WORD")]
        public UInt16 Hour_Negative_High_Word;
        [PLCElement(ValueName = "HID_TO_OHXC_HOUR_NEGATIVE_Lo_WORD")]
        public UInt16 Hour_Negative_Low_Word;
        [PLCElement(ValueName = "HID_TO_OHXC_VR")]
        public UInt16 VR_Source;
        [PLCElement(ValueName = "HID_TO_OHXC_VS")]
        public UInt16 VS_Source;
        [PLCElement(ValueName = "HID_TO_OHXC_VT")]
        public UInt16 VT_Source;
        [PLCElement(ValueName = "HID_TO_OHXC_SIGMA_V")]
        public UInt16 Sigma_V_Source;
        [PLCElement(ValueName = "HID_TO_OHXC_AR")]
        public UInt16 AR_Source;
        [PLCElement(ValueName = "HID_TO_OHXC_AS")]
        public UInt16 AS_Source;
        [PLCElement(ValueName = "HID_TO_OHXC_AT")]
        public UInt16 AT_Source;
        [PLCElement(ValueName = "HID_TO_OHXC_SIGMA_A")]
        public UInt16 Sigma_A_Source;
        [PLCElement(ValueName = "HID_TO_OHXC_WR")]
        public UInt16 WR_Source;
        [PLCElement(ValueName = "HID_TO_OHXC_WS")]
        public UInt16 WS_Source;
        [PLCElement(ValueName = "HID_TO_OHXC_WT")]
        public UInt16 WT_Source;
        [PLCElement(ValueName = "HID_TO_OHXC_SIGMA_W")]
        public UInt16 Sigma_W_Source;

        public override string ToString()
        {
            string sJson = Newtonsoft.Json.JsonConvert.SerializeObject(this, JsHelper.jsBooleanArrayConverter, JsHelper.jsTimeConverter);
            sJson = sJson.Replace(nameof(Timestamp), "@timestamp");
            return sJson;
        }
    }
}
