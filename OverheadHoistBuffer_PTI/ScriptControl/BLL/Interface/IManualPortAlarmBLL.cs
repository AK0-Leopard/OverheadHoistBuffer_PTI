namespace com.mirle.ibg3k0.sc.BLL.Interface
{
    public interface IManualPortAlarmBLL
    {
        bool GetAlarmReport(string eqId, string alarmCode, out ALARM alarmReport);

        bool GetAlarmReport(string eqId, string alarmCode, string commandId, out ALARM alarmReport);
    }
}
