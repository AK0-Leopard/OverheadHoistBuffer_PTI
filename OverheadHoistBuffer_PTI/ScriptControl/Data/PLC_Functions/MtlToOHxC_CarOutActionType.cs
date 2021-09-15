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
    class MtlToOHxC_CarOutActionType : PLC_FunBase
    {
        [PLCElement(ValueName = "MTL_TO_OHXC_U2D_CAR_OUT_ACTION_TYPE_1")]
        public bool ActionType1;
        [PLCElement(ValueName = "MTL_TO_OHXC_U2D_CAR_OUT_ACTION_TYPE_2")]
        public bool ActionType2;
        [PLCElement(ValueName = "MTL_TO_OHXC_U2D_CAR_OUT_ACTION_TYPE_3")]
        public bool ActionType3;
        [PLCElement(ValueName = "MTL_TO_OHXC_U2D_CAR_OUT_ACTION_TYPE_4")]
        public bool ActionType4;
        [PLCElement(ValueName = "MTL_TO_OHXC_U2D_CAR_OUT_ACTION_TYPE_5")]
        public bool ActionType5;
    }

    class MtlToOHxC_CarOutActionType_PH2 : PLC_FunBase
    {
        [PLCElement(ValueName = "MTL_TO_OHXC_U2D_CAR_OUT_ACTION_TYPE_1_PH2")]
        public bool ActionType1;
        [PLCElement(ValueName = "MTL_TO_OHXC_U2D_CAR_OUT_ACTION_TYPE_2_PH2")]
        public bool ActionType2;
        [PLCElement(ValueName = "MTL_TO_OHXC_U2D_CAR_OUT_ACTION_TYPE_3_PH2")]
        public bool ActionType3;
        [PLCElement(ValueName = "MTL_TO_OHXC_U2D_CAR_OUT_ACTION_TYPE_4_PH2")]
        public bool ActionType4;
        [PLCElement(ValueName = "MTL_TO_OHXC_U2D_CAR_OUT_ACTION_TYPE_5_PH2")]
        public bool ActionType5;
    }


}
