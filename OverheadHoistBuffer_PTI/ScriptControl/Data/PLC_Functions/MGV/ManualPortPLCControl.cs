using com.mirle.ibg3k0.sc.Data.PLC_Functions.MGV.Enums;
using System;

namespace com.mirle.ibg3k0.sc.Data.PLC_Functions.MGV
{
    public abstract class ManualPortPLCControl : PLC_FunBase
    {

    }

    public class ManualPortPLCControl_Reset : ManualPortPLCControl
    {
        [PLCElement(ValueName = "OHxC_TO_MGV_RESET")]
        public bool IsResetOn;
    }

    public class ManualPortPLCControl_BuzzerStop : ManualPortPLCControl
    {
        [PLCElement(ValueName = "OHxC_TO_MGV_BUZZERSTOP")]
        public bool IsBuzzerStop;
    }

    public class ManualPortPLCControl_Run : ManualPortPLCControl
    {
        [PLCElement(ValueName = "OHxC_TO_MGV_RUN")]
        public bool IsSetRun;

        [PLCElement(ValueName = "OHxC_TO_MGV_STOP")]
        public bool IsSetStop;
    }

    public class ManualPortPLCControl_Commanding : ManualPortPLCControl
    {
        [PLCElement(ValueName = "OHxC_TO_MGV_COMMANDING")]
        public bool IsCommanding;
    }

    public class ManualPortPLCControl_MoveBack : ManualPortPLCControl
    {
        [PLCElement(ValueName = "OHxC_TO_MGV_MOVEBACK")]
        public bool IsMoveBack;

        [PLCElement(ValueName = "OHxC_TO_MGV_MOVEBACKREASON")]
        public UInt16 MoveBackReason;

        public MoveBackReasons MoveBackReasons { get => GetMoveBackReason(); }

        private MoveBackReasons GetMoveBackReason()
        {
            switch (MoveBackReason)
            {
                case 0:
                    return MoveBackReasons.NotMoveBack;
                case 1:
                    return MoveBackReasons.TypeMismatch;
                case 2:
                    return MoveBackReasons.RejectedByMES;
                case 3:
                    return MoveBackReasons.MoveInTimedOut;
                default:
                    return MoveBackReasons.Other;
            }
        }
    }

    public class ManualPortPLCControl_InputPermission : ManualPortPLCControl
    {
        [PLCElement(ValueName = "OHxC_TO_MGV_INPUT_PERMISSION")]
        public bool IsInputPermission;

        [PLCElement(ValueName = "OHxC_TO_MGV_INPUT_PERMISSION_FAIL")]
        public bool IsInputPermissionFailed;
    }

    public class ManualPortPLCControl_ChangeDirection : ManualPortPLCControl
    {
        [PLCElement(ValueName = "OHxC_TO_MGV_INMODE")]
        public bool IsChangeToInMode;

        [PLCElement(ValueName = "OHxC_TO_MGV_OUTMODE")]
        public bool IsChangeToOutMode;
    }

    public class ManualPortPLCControl_ErrorIndex : ManualPortPLCControl
    {
        [PLCElement(ValueName = "OHxC_TO_MGV_ERRORINDEX")]
        public UInt16 OhbcErrorIndex;
    }

    public class ManualPortPLCControl_ReadyToWaitoutCarrier : ManualPortPLCControl
    {
        [PLCElement(ValueName = "OHxC_TO_MGV_READY_TO_WAITOUT_CARRIERID_1")]
        public string ReadyToWaitOutCarrierId1;

        [PLCElement(ValueName = "OHxC_TO_MGV_READY_TO_WAITOUT_CARRIERID_2")]
        public string ReadyToWaitOutCarrierId2;
    }

    public class ManualPortPLCControl_ComingOutCarrier : ManualPortPLCControl
    {
        [PLCElement(ValueName = "OHxC_TO_MGV_COMING_OUT_CARRIERID")]
        public string ComingOutCarrierId;
    }

    public class ManualPortPLCControl_TimeCalibration : ManualPortPLCControl
    {
        [PLCElement(ValueName = "TIME_CALIBRATION_BCD_YEAR_MONTH")]
        public UInt16 TimeCalibrationBcdYearMonth;

        [PLCElement(ValueName = "TIME_CALIBRATION_BCD_DAY_HOUR")]
        public UInt16 TimeCalibrationBcdDayHour;

        [PLCElement(ValueName = "TIME_CALIBRATION_BCD_MINUTE_SECOND")]
        public UInt16 TimeCalibrationBcdMinuteSecond;

        [PLCElement(ValueName = "TIME_CALIBRATION_INDEX")]
        public UInt16 TimeCalibrationIndex;
    }
}
