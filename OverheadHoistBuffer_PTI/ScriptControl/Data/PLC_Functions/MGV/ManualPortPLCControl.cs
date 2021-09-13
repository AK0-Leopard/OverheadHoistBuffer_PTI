using com.mirle.ibg3k0.sc.Data.PLC_Functions.MGV.Enums;
using System;

namespace com.mirle.ibg3k0.sc.Data.PLC_Functions.MGV
{
    public class ManualPortPLCControl : PLC_FunBase
    {
        [PLCElement(ValueName = "OHxC_TO_MGV_RESET")]
        public bool IsResetOn;

        [PLCElement(ValueName = "OHxC_TO_MGV_BUZZERSTOP")]
        public bool IsBuzzerStop;

        [PLCElement(ValueName = "OHxC_TO_MGV_RUN")]
        public bool IsSetRun;

        [PLCElement(ValueName = "OHxC_TO_MGV_STOP")]
        public bool IsSetStop;

        [PLCElement(ValueName = "OHxC_TO_MGV_COMMANDING")]
        public bool IsCommanding;

        [PLCElement(ValueName = "OHxC_TO_MGV_MOVEBACK")]
        public bool IsMoveBack;

        [PLCElement(ValueName = "OHxC_TO_MGV_INPUT_PERMISSION")]
        public bool IsInputPermission;

        [PLCElement(ValueName = "OHxC_TO_MGV_INPUT_PERMISSION_FAIL")]
        public bool IsInputPermissionFailed;

        [PLCElement(ValueName = "OHxC_TO_MGV_INMODE")]
        public bool IsChangeToInMode;

        [PLCElement(ValueName = "OHxC_TO_MGV_OUTMODE")]
        public bool IsChangeToOutMode;

        [PLCElement(ValueName = "OHxC_TO_MGV_ERRORINDEX")]
        public UInt16 OhbcErrorIndex;

        [PLCElement(ValueName = "OHxC_TO_MGV_MOVEBACKREASON")]
        public UInt16 MoveBackReason;

        [PLCElement(ValueName = "OHxC_TO_MGV_READY_TO_WAITOUT_CARRIERID_1")]
        public string ReadyToWaitOutCarrierId1;

        [PLCElement(ValueName = "OHxC_TO_MGV_READY_TO_WAITOUT_CARRIERID_2")]
        public string ReadyToWaitOutCarrierId2;

        [PLCElement(ValueName = "OHxC_TO_MGV_COMING_OUT_CARRIERID")]
        public string ComingOutCarrierId;

        [PLCElement(ValueName = "TIME_CALIBRATION_BCD_YEAR_MONTH")]
        public UInt16 TimeCalibrationBcdYearMonth;

        [PLCElement(ValueName = "TIME_CALIBRATION_BCD_DAY_HOUR")]
        public UInt16 TimeCalibrationBcdDayHour;

        [PLCElement(ValueName = "TIME_CALIBRATION_BCD_MINUTE_SECOND")]
        public UInt16 TimeCalibrationBcdMinuteSecond;

        [PLCElement(ValueName = "TIME_CALIBRATION_INDEX")]
        public UInt16 TimeCalibrationIndex;

        public MoveBackReasons MoveBackReasons { get => GetMoveBackReason(); }

        private MoveBackReasons GetMoveBackReason()
        {
            if (MoveBackReason == 1)
                return MoveBackReasons.TypeMismatch;
            else
                return MoveBackReasons.Other;
        }
    }
}
