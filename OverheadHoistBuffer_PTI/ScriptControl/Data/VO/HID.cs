using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace com.mirle.ibg3k0.sc.Data.VO
{
    public class HID : AEQPT
    {
        const string HID_HEARTBEAT_LOST_CODE = "21";

        #region HID status
        public UInt16 Alive { get; internal set; }
        public UInt16 HID_ID { get; internal set; }
        public UInt16 V_Unit { get; internal set; }
        public UInt16 V_Dot { get; internal set; }
        public UInt16 A_Unit { get; internal set; }
        public UInt16 A_Dot { get; internal set; }
        public UInt16 W_Unit { get; internal set; }
        public UInt16 W_Dot { get; internal set; }
        public UInt16 Hour_Unit { get; internal set; }
        public UInt16 Hour_Dot { get; internal set; }
        public UInt16 Hour_Sigma_High_Word { get; internal set; }
        public UInt16 Hour_Sigma_Low_Word { get; internal set; }
        public UInt16 Hour_Positive_High_Word { get; internal set; }
        public UInt16 Hour_Positive_Low_Word { get; internal set; }
        public UInt16 Hour_Negative_High_Word { get; internal set; }
        public UInt16 Hour_Negative_Low_Word { get; internal set; }
        public UInt16 VR_Source { get; internal set; }
        public UInt16 VS_Source { get; internal set; }
        public UInt16 VT_Source { get; internal set; }
        public UInt16 Sigma_V_Source { get; internal set; }
        public UInt16 AR_Source { get; internal set; }
        public UInt16 AS_Source { get; internal set; }
        public UInt16 AT_Source { get; internal set; }
        public UInt16 Sigma_A_Source { get; internal set; }
        public UInt16 WR_Source { get; internal set; }
        public UInt16 WS_Source { get; internal set; }
        public UInt16 WT_Source { get; internal set; }
        public UInt16 Sigma_W_Source { get; internal set; }
        public double Hour_Sigma_Converted => convertValueTwoWord(Hour_Unit, Hour_Dot, Hour_Sigma_High_Word, Hour_Sigma_Low_Word);
        public double Hour_Positive_Converted => convertValueTwoWord(Hour_Unit, Hour_Dot, Hour_Positive_High_Word, Hour_Positive_Low_Word);
        public double Hour_Negative_Converted => convertValueTwoWord(Hour_Unit, Hour_Dot, Hour_Negative_High_Word, Hour_Negative_Low_Word);
        public double VR_Converted => convertValueOneWord(V_Unit, V_Dot, VR_Source);
        public double VS_Converted => convertValueOneWord(V_Unit, V_Dot, VS_Source);
        public double VT_Converted => convertValueOneWord(V_Unit, V_Dot, VT_Source);
        public double Sigma_V_Converted => convertValueOneWord(V_Unit, V_Dot, Sigma_V_Source);
        public double AR_Converted => convertValueOneWord(A_Unit, A_Dot, AR_Source);
        public double AS_Converted => convertValueOneWord(A_Unit, A_Dot, AS_Source);
        public double AT_Converted => convertValueOneWord(A_Unit, A_Dot, AT_Source);
        public double Sigma_A_Converted => convertValueOneWord(A_Unit, A_Dot, Sigma_A_Source);
        public double WR_Converted => convertValueOneWord(W_Unit, W_Dot, WR_Source);
        public double WS_Converted => convertValueOneWord(W_Unit, W_Dot, WS_Source);
        public double WT_Converted => convertValueOneWord(W_Unit, W_Dot, WT_Source);
        public double Sigma_W_Converted => convertValueOneWord(W_Unit, W_Dot, Sigma_W_Source);
        #endregion
        public bool IsPowerAlarm { get; private set; }
        public bool IsTemperatureAlarm { get; private set; }

        private bool isHeartbeatLoss;
        public bool IsHeartbeatLoss
        {
            get => isHeartbeatLoss;
            private set
            {
                if (value != isHeartbeatLoss)
                {
                    isHeartbeatLoss = value;
                    ReportHIDAlarm(HID_HEARTBEAT_LOST_CODE, isHeartbeatLoss ? ErrorStatus.ErrSet : ErrorStatus.ErrReset);
                    if (isHeartbeatLoss)
                        OnPowerOrTempAlarm?.Invoke(this, null);
                }
            }
        }
        public EventHandler OnPowerOrTempAlarm;

        List<string> Segments { get; set; } = new List<string>();
        public void setSegments(List<string> segIDs)
        {
            Segments = segIDs;
        }
        public List<string> getSegments()
        {
            return Segments.ToList();
        }

        private HIDValueDefMapAction getExcuteMapAction()
        {
            HIDValueDefMapAction mapAction;
            mapAction = this.getMapActionByIdentityKey(typeof(HIDValueDefMapAction).Name) as HIDValueDefMapAction;

            return mapAction;
        }

        public void HIDControl(bool control)
        {
            var mapAction = getExcuteMapAction();
            if (mapAction != null)
                mapAction.HID_Control(control);
        }

        public void SilentCommand()
        {
            var mapAction = getExcuteMapAction();
            if (mapAction != null)
                mapAction.SilentCommand();
        }

        public void DateTimeSyncCommand(DateTime dateTime)
        {
            var mapAction = getExcuteMapAction();
            if (mapAction != null)
                mapAction.DateTimeSyncCommand(dateTime);
        }

        public void SendHeartbeatCommand()
        {
            var mapAction = getExcuteMapAction();
            if (mapAction != null)
                mapAction.SendHeartbeat();
        }

        public void CheckHeartbeatTimedOut(int threshold)
        {
            if ((threshold > 0) 
                && (Eq_Alive_Last_Change_time > DateTime.MinValue) //2023.04.17: 已經至少被更新過一次，才檢查是否dead
                && (Eq_Alive_Last_Change_time.AddSeconds(threshold) < DateTime.Now))
                IsHeartbeatLoss = true;
            else
                IsHeartbeatLoss = false;
        }

        internal void ReportHIDAlarm(string AlarmCode, ErrorStatus status)
        {
            if (status is ErrorStatus.ErrSet)
                SCApplication.getInstance().AlarmBLL.setAlarmReport(NODE_ID, EQPT_ID, AlarmCode, null);
            else
                SCApplication.getInstance().AlarmBLL.resetAlarmReport(EQPT_ID, AlarmCode);
        }

        internal void ReportHIDPowerAlarm(string AlarmCode, ErrorStatus status)
        {
            IsPowerAlarm = (status == ErrorStatus.ErrSet);
            if (IsPowerAlarm)
            {
                SCApplication.getInstance().AlarmBLL.setAlarmReport(NODE_ID, EQPT_ID, AlarmCode, null);
                OnPowerOrTempAlarm?.Invoke(this, null);
            }
            else
                SCApplication.getInstance().AlarmBLL.resetAlarmReport(EQPT_ID, AlarmCode);
        }

        internal void ReportHIDTemperatureAlarm(string AlarmCode, ErrorStatus status)
        {
            IsTemperatureAlarm = (status == ErrorStatus.ErrSet);
            if (IsTemperatureAlarm)
            {
                SCApplication.getInstance().AlarmBLL.setAlarmReport(NODE_ID, EQPT_ID, AlarmCode, null);
                OnPowerOrTempAlarm?.Invoke(this, null);
            }
            else
                SCApplication.getInstance().AlarmBLL.resetAlarmReport(EQPT_ID, AlarmCode);
        }

        private double convertValueOneWord(UInt64 unit, UInt64 dot, UInt64 source_value)
        {
            double convertValue;
            double temp;
            double unit_d = Convert.ToDouble(unit);
            double dot_d = Convert.ToDouble(dot);
            double multiplier = Math.Pow(10, (unit_d - dot_d));
            temp = source_value * multiplier;
            convertValue = temp;
            return convertValue;
        }
        private double convertValueTwoWord(UInt64 unit, UInt64 dot, UInt64 source_value_high_word, UInt64 source_value_low_word)
        {
            double convertValue;
            double temp;
            double source_value = (source_value_high_word * 65536) + source_value_low_word;
            double unit_d = Convert.ToDouble(unit);
            double dot_d = Convert.ToDouble(dot);
            double source_value_d = Convert.ToDouble(source_value);
            double multiplier = Math.Pow(10, (unit_d - dot_d));
            //UInt64 multiplier = Convert.ToUInt64(Math.Pow(10, (unit_d - dot_d)));
            temp = source_value_d * multiplier;
            convertValue = temp;
            return convertValue;
        }
    }
}
