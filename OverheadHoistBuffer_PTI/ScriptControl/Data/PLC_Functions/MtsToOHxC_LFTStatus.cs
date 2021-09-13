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
    class MtsToOHxC_LFTStatus : PLC_FunBase
    {
        [PLCElement(ValueName = "MTS_TO_OHXC_LFT_STATUS_HAS_VEHICLE")]
        public bool HasVehicle;
        [PLCElement(ValueName = "MTS_TO_OHXC_LFT_STATUS_STOP_SINGLE")]
        public bool StopSingle;
        [PLCElement(ValueName = "MTS_TO_OHXC_LFT_MODE")]
        public UInt16 Mode;
        [PLCElement(ValueName = "MTS_TO_OHXC_LFT_LOCATION")]
        public UInt16 LFTLocation;
        [PLCElement(ValueName = "MTS_TO_OHXC_LFT_MOVING_STATUS")]
        public UInt16 LFTMovingStatus;
        [PLCElement(ValueName = "MTS_TO_OHXC_LFT_ENCODER")]
        public UInt32 LFTEncoder;
        [PLCElement(ValueName = "MTS_TO_OHXC_LFT_VEHICLE_IN_POSITION")]
        public UInt16 VhInPosition;
    }


}
