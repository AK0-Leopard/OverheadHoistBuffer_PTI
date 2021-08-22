using System.Collections.Generic;

namespace com.mirle.ibg3k0.sc.BLL.Interface
{
    public interface IManualPortAlarmBLL
    {
        bool SetAlarm(string portName, string alarmCode, ACMD_MCS commandOfPort, out ALARM alarmReport, out string reasonOfAlarmSetFalied);
        bool ClearAllAlarm(string portName, ACMD_MCS commandOfPort, out List<ALARM> alarmReports, out string reasonOfAlarmClearFalied);
    }
}
