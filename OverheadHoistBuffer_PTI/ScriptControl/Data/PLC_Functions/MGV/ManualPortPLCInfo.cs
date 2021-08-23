using com.mirle.ibg3k0.sc.Data.PLC_Functions.MGV.Enums;
using System;

namespace com.mirle.ibg3k0.sc.Data.PLC_Functions.MGV
{
    public class ManualPortPLCInfo : PLC_FunBase
    {
        [PLCElement(ValueName = "MGV_TO_OHxC_RUN")]
        public bool IsRun;

        [PLCElement(ValueName = "MGV_TO_OHxC_DOWN")]
        public bool IsDown;

        [PLCElement(ValueName = "MGV_TO_OHxC_FAULT")]
        public bool IsAlarm;

        [PLCElement(ValueName = "MGV_TO_OHxC_INMODE")]
        public bool IsInMode;

        [PLCElement(ValueName = "MGV_TO_OHxC_OUTMODE")]
        public bool IsOutMode;

        [PLCElement(ValueName = "MGV_TO_OHxC_PORTMODECHANGEABLE")]
        public bool IsDirectionChangable;

        [PLCElement(ValueName = "MGV_TO_OHxC_RUNENABLE")]
        public bool RunEnable;

        [PLCElement(ValueName = "MGV_TO_OHxC_WAITIN")]
        public bool IsWaitIn;

        [PLCElement(ValueName = "MGV_TO_OHxC_WAITOUT")]
        public bool IsWaitOut;

        [PLCElement(ValueName = "MGV_TO_OHxC_LOADOK")]
        public bool IsLoadOK;

        [PLCElement(ValueName = "MGV_TO_OHxC_UNLOADOK")]
        public bool IsUnloadOK;

        [PLCElement(ValueName = "MGV_TO_OHxC_LOADPRESENCE1")]
        public bool LoadPosition1;

        [PLCElement(ValueName = "MGV_TO_OHxC_LOADPRESENCE2")]
        public bool LoadPosition2;

        [PLCElement(ValueName = "MGV_TO_OHxC_LOADPRESENCE3")]
        public bool LoadPosition3;

        [PLCElement(ValueName = "MGV_TO_OHxC_LOADPRESENCE4")]
        public bool LoadPosition4;

        [PLCElement(ValueName = "MGV_TO_OHxC_LOADPRESENCE5")]
        public bool LoadPosition5;

        [PLCElement(ValueName = "MGV_TO_OHxC_BCRREADDONE")]
        public bool IsBcrReadDone;

        [PLCElement(ValueName = "MGV_TO_OHxC_TRANSFERCOMPLETE")]
        public bool IsTransferComplete;

        [PLCElement(ValueName = "MGV_TO_OHxC_REMOVECHECK")]
        public bool IsRemoveCheck;

        [PLCElement(ValueName = "MGV_TO_OHxC_DOOROPEN")]
        public bool IsDoorOpen;

        [PLCElement(ValueName = "MGV_TO_OHxC_ERRORINDEX")]
        public UInt16 ErrorIndex;

        [PLCElement(ValueName = "MGV_TO_OHxC_ERRORCODE")]
        public UInt16 AlarmCode;

        [PLCElement(ValueName = "MGV_TO_OHxC_STAGE1CARRIERID")]
        public string CarrierIdOfStage1;

        [PLCElement(ValueName = "MGV_TO_OHxC_BCRREADRESULT")]
        public string CarrierIdReadResult;

        [PLCElement(ValueName = "MGV_TO_OHxC_CSTTYPE")]
        public UInt16 CstTypes;

        public CstType CarrierType { get => GetCstType(); }

        private CstType GetCstType()
        {
            if (CstTypes == 1)
                return CstType.A;
            else
                return CstType.B;
        }

        public DirectionType Direction
        {
            get
            {
                if (IsInMode)
                {
                    return DirectionType.InMode;
                }
                else
                {
                    return DirectionType.OutMode;
                }
            }
        }
    }
}
