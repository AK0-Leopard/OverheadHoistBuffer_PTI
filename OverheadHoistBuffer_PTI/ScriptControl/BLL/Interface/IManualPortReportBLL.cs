namespace com.mirle.ibg3k0.sc.BLL.Interface
{
    public interface IManualPortReportBLL
    {
        /// <summary>
        /// 實作時需上報 S6F11 Alarm Set 和 S5F1 Alarm Report
        /// </summary>
        /// <param name="alarm"></param>
        /// <returns></returns>
        bool ReportAlarmSet(ALARM alarm);

        /// <summary>
        /// 實作時需上報 S6F11 Alarm Clear 和 S5F1 Alarm Report
        /// </summary>
        /// <param name="alarm"></param>
        /// <returns></returns>
        bool ReportAlarmClear(ALARM alarm);

        /// <summary>
        /// 實作時只上報 S6F11 Unit Alarm Set 不可多上報 S5F1 Alarm Report
        /// </summary>
        /// <param name="alarm"></param>
        /// <returns></returns>
        bool ReportUnitAlarmSet(ALARM alarm);

        /// <summary>
        /// 實作時只上報 S6F11 Unit Alarm Clear 不可多上報 S5F1 Alarm Report
        /// </summary>
        /// <param name="alarm"></param>
        /// <returns></returns>
        bool ReportUnitAlarmClear(ALARM alarm);

        /// <summary>
        /// 實作時需先上報 S6F11 CarrierIDRead 再上報 S6F11 WaitIn
        /// </summary>
        /// <param name="cassetteData"></param>
        /// <returns></returns>
        bool ReportCarrierWaitIn(CassetteData cassetteData, bool isDuplicate);
        bool ReportForcedRemoveCarrier(CassetteData cassetteData);
        bool ReportCarrierRemoveFromManualPort(string carrierId);
        bool ReportPortDirectionChanged(string portName, bool newDirectionIsInMode);
        bool ReportPortInServiceChanged(string portName, bool newStateIsInService);
        bool ReportCarrierIDRead(CassetteData cassetteData, bool isDuplicate);
        bool ReportCarrierWaitIn(CassetteData cassetteData);
        bool ReportCarrierWaitOut(CassetteData cassetteData);
        bool ReportTransferCompleted(ACMD_MCS cmd, CassetteData cassette, string resultCode);
    }
}
