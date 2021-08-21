using com.mirle.ibg3k0.sc.BLL.Interface;
using com.mirle.ibg3k0.sc.Data.PLC_Functions.MGV;
using com.mirle.ibg3k0.sc.Data.PLC_Functions.MGV.Enums;
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction.Events;
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction.Interface;
using com.mirle.ibg3k0.sc.Service.Interface;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace com.mirle.ibg3k0.sc.Service
{
    public class ManualPortEventService : IManualPortEventService
    {
        private Logger logger = LogManager.GetLogger("ManualPortLogger");

        private ConcurrentDictionary<string, IManualPortValueDefMapAction> manualPorts { get; set; }

        private IManualPortReportBLL reportBll;
        private IManualPortCMDBLL commandBLL;
        private IManualPortAlarmBLL alarmBLL;
        private IManualPortDefBLL portDefBLL;
        private IManualPortShelfDefBLL shelfDefBLL;
        private IManualPortCassetteDataBLL cassetteDataBLL;

        public ManualPortEventService()
        {
            WriteLog($"ManualPortEventService Initial");
            //RegisterEvent(ports);
        }
        public void Start(IEnumerable<IManualPortValueDefMapAction> ports,
                                      IManualPortReportBLL reportBll,
                                      IManualPortDefBLL portDefBLL,
                                      IManualPortShelfDefBLL shelfDefBLL,
                                      IManualPortCassetteDataBLL cassetteDataBLL,
                                      IManualPortCMDBLL commandBLL,
                                      IManualPortAlarmBLL alarmBLL)
        {
            this.reportBll = reportBll;
            this.portDefBLL = portDefBLL;
            this.shelfDefBLL = shelfDefBLL;
            this.cassetteDataBLL = cassetteDataBLL;
            this.commandBLL = commandBLL;
            this.alarmBLL = alarmBLL;
            WriteLog($"ManualPortEventService Start");

            RegisterEvent(ports);
        }

        private void RegisterEvent(IEnumerable<IManualPortValueDefMapAction> ports)
        {
            manualPorts = new ConcurrentDictionary<string, IManualPortValueDefMapAction>();

            foreach (var port in ports)
            {
                port.OnLoadPresenceChanged += Port_OnLoadPresenceChanged; ;
                port.OnWaitIn += Port_OnWaitIn;
                port.OnBcrReadDone += Port_OnBcrReadDone;
                port.OnWaitOut += Port_OnWaitOut;
                port.OnCstRemoved += Port_OnCstRemoved;
                port.OnDirectionChanged += Port_OnDirectionChanged;
                port.OnInServiceChanged += Port_OnInServiceChanged;
                port.OnAlarmHappen += Port_OnAlarmHappen;
                port.OnAlarmClear += Port_OnAlarmClear;

                manualPorts.TryAdd(port.PortName, port);
                WriteLog($"Add Manual Port Event Success ({port.PortName})");
            }
        }

        #region Log

        private void WriteLog(string message)
        {
            var logMessage = DateTime.Now.ToString("HH:mm:ss.fff ") + message;
            logger.Info(logMessage);
        }

        private void WriteEventLog(string message)
        {
            var logMessage = DateTime.Now.ToString("HH:mm:ss.fff ") + $"PLC >> OHBC | {message}";
            logger.Info(logMessage);
        }

        #endregion Log

        #region LoadPresenceChanged
        private void Port_OnLoadPresenceChanged(object sender, ManualPortEventArgs args)
        {
            try
            {
                var info = args.ManualPortPLCInfo;
                var stage1CarrierId = info.CarrierIdOfStage1;

                if (info.LoadPosition1)
                {
                    var logTitle = $"PortName[{args.PortName}] LoadPresenceChanged -> Stage1 ON => ";
                    WriteEventLog($"{logTitle} CarrierIdOfStage1[{stage1CarrierId}]");

                    StagePresenceOnProcess(logTitle, args.PortName, info);
                }
                else
                {
                    var logTitle = $"PortName[{args.PortName}] LoadPresenceChanged -> Stage1 OFF => ";
                    WriteEventLog($"{logTitle} CarrierIdOfStage1[{stage1CarrierId}]");

                    StagePresenceOffProcess(logTitle, args.PortName, info);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "");
            }
        }

        private void StagePresenceOnProcess(string logTitle, string portName, ManualPortPLCInfo info)
        {
            if (info.Direction == DirectionType.InMode)
                return;

            if (cassetteDataBLL.GetCarrierByPortName(portName, stage: 1, out var cassetteData) == false)
                WriteEventLog($"{logTitle} The port direction is InMode. Cannot find carrier data at this port.");

            //儲位到 port 的話 stage 1 ON 時通常已經有帳了嗎?  或是 需要將帳從車上移到 Port 上

            if (reportBll.ReportCarrierWaitOut(cassetteData))
                WriteEventLog($"{logTitle} Report MCS carrier wait out success.");
            else
                WriteEventLog($"{logTitle} Report MCS carrier wait out failed.");
        }

        private void StagePresenceOffProcess(string logTitle, string portName, ManualPortPLCInfo info)
        {
            if (info.Direction == DirectionType.InMode)
                return;

            if (cassetteDataBLL.GetCarrierByPortName(portName, stage: 1, out var cassetteData) == false)
            {
                WriteEventLog($"{logTitle} The port direction is OutMode but cannot find carrier data at this port. Normal should have data.");
                return;
            }

            WriteEventLog($"{logTitle} The port direction is OutMode. Find a carrier data [{cassetteData.BOXID}] at this port.");

            cassetteDataBLL.Delete(cassetteData.BOXID);
            WriteEventLog($"{logTitle} Delete carrier data [{cassetteData.BOXID}].");

            if (reportBll.ReportCarrierRemoveFromManualPort(cassetteData.BOXID))
                WriteEventLog($"{logTitle} Report MCS carrier remove From manual port success.");
            else
                WriteEventLog($"{logTitle} Report MCS carrier remove From manual port Failed.");
        }
        #endregion LoadPresenceChanged

        #region Wait In
        private void Port_OnWaitIn(object sender, ManualPortEventArgs args)
        {
            var info = args.ManualPortPLCInfo;
            var readResult = info.CarrierIdReadResult;
            var stage1CarrierId = info.CarrierIdOfStage1;

            var logTitle = $"PortName[{args.PortName}] WaitIn => ";

            WriteEventLog($"{logTitle} ReadResult[{readResult}] CarrierIdOfStage1[{stage1CarrierId}]");

            if (cassetteDataBLL.GetCarrierByBoxId(info.CarrierIdOfStage1, out var duplicateCarrierId))
            {
                if (duplicateCarrierId.Carrier_LOC != args.PortName)
                    WaitInDuplicateProcess(logTitle, args.PortName, info, duplicateCarrierId);
                else
                    WaitInNormalProcess(logTitle, args.PortName, info);
            }
            else
                WaitInNormalProcess(logTitle, args.PortName, info);
        }

        private void WaitInDuplicateProcess(string logTitle, string portName, ManualPortPLCInfo info, CassetteData duplicateCarrierData)
        {
            WriteEventLog($"{logTitle} Duplicate Happened. Duplication location is [{duplicateCarrierData.Carrier_LOC}]");

            if (portDefBLL.GetPortDef(duplicateCarrierData.Carrier_LOC, out var duplicateLocation))
            {
                WaitInDuplicateAtShelfProcess(logTitle, portName, info, duplicateCarrierData);

                WaitInDuplicateAtPortProcess(logTitle, portName, info, duplicateCarrierData, duplicateLocation);
            }
            else
            {
                WaitInDuplicateAtOhtProcess(logTitle, portName, info, duplicateCarrierData);
            }
        }

        public string GetDuplicateUnknownId(string carrierId)
        {
            var year = DateTime.Now.Year % 100;
            var date = string.Format("{0}{1:00}{2:00}{3:00}{4:00}", year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute);
            var id = "UNKD" + carrierId + date + string.Format("{0:00}", DateTime.Now.Second);

            return id;
        }

        private void WaitInDuplicateAtShelfProcess(string logTitle, string portName, ManualPortPLCInfo info, CassetteData duplicateCarrierId)
        {
            WriteEventLog($"{logTitle} Duplicate at shelf ({duplicateCarrierId.Carrier_LOC}).");
        }

        private void WaitInDuplicateAtPortProcess(string logTitle, string portName, ManualPortPLCInfo info, CassetteData duplicateCarrierId, PortDef duplicatePort)
        {
            WriteEventLog($"{logTitle} Duplicate at Port ({duplicatePort.PLCPortID}).");
        }

        private void WaitInDuplicateAtOhtProcess(string logTitle, string portName, ManualPortPLCInfo info, CassetteData duplicateCarrierId)
        {
            WriteEventLog($"{logTitle} Duplicate at OHT ({duplicateCarrierId.Carrier_LOC}).");
        }

        private void WaitInNormalProcess(string logTitle, string portName, ManualPortPLCInfo info)
        {
            WriteEventLog($"{logTitle} Normal Process.");

            CheckResidualCassetteProcess(logTitle, portName);

            cassetteDataBLL.Install(portName, info.CarrierIdOfStage1);
            WriteEventLog($"{logTitle} Install cassette data at this port.");

            cassetteDataBLL.GetCarrierByPortName(portName, 1, out var cassetteData);

            ReportIDRead(logTitle, cassetteData, isDuplicate: false);
            ReportWaitIn(logTitle, cassetteData);
        }

        private void CheckResidualCassetteProcess(string logTitle, string portName)
        {
            cassetteDataBLL.GetCarrierByPortName(portName, stage: 1, out var residueCassetteData);
            var hasNoResidueData = residueCassetteData == null;
            if (hasNoResidueData)
                return;

            var residueCarrierId = residueCassetteData.BOXID;
            WriteEventLog($"{logTitle} There is residual cassette data [{residueCarrierId}] on the port.");

            if (commandBLL.GetCommandByBoxId(residueCarrierId, out var residueCommand))
            {
                WriteEventLog($"{logTitle} There is residual command [{residueCommand.CMD_ID}] cassette data [{residueCarrierId}].");

                commandBLL.Delete(residueCarrierId);
                WriteEventLog($"{logTitle} Delete residual command [{residueCommand.CMD_ID}].");

                ReportForcedTransferComplete(logTitle, residueCommand, residueCassetteData);
            }

            cassetteDataBLL.Delete(residueCarrierId);
            WriteEventLog($"{logTitle} Delete residual cassette data.");

            ReportForcedCarrierRemove(logTitle, residueCassetteData);
        }

        private void ReportForcedTransferComplete(string logTitle, ACMD_MCS command, CassetteData cassetteData)
        {
            if (reportBll.ReportTransferCompleted(command, cassetteData, ACMD_MCS.ResultCode.OtherErrors))
                WriteEventLog($"{logTitle} Report MCS TransferComplete  ResultCod -> OtherErrors  Success.");
            else
                WriteEventLog($"{logTitle} Report MCS TransferComplete  ResultCode -> OtherErrors  Failed.");
        }

        private void ReportForcedCarrierRemove(string logTitle, CassetteData cassetteData)
        {
            if (reportBll.ReportForcedRemoveCarrier(cassetteData))
                WriteEventLog($"{logTitle} Report MCS CarrierRemoveComplete Success.");
            else
                WriteEventLog($"{logTitle} Report MCS CarrierRemoveComplete Failed.");
        }

        private void ReportIDRead(string logTitle, CassetteData cassetteData, bool isDuplicate)
        {
            if (reportBll.ReportCarrierIDRead(cassetteData, isDuplicate))
                WriteEventLog($"{logTitle} Report MCS CarrierIDRead Success. isDuplicate[{isDuplicate}]");
            else
                WriteEventLog($"{logTitle} Report MCS CarrierIDRead Failed.  isDuplicate[{isDuplicate}]");
        }

        private void ReportWaitIn(string logTitle, CassetteData cassetteData)
        {
            if (reportBll.ReportCarrierWaitIn(cassetteData))
                WriteEventLog($"{logTitle} Report MCS WaitIn Success.");
            else
                WriteEventLog($"{logTitle} Report MCS WaitIn Failed.");
        }
        #endregion Wait In

        private void Port_OnBcrReadDone(object sender, ManualPortEventArgs args)
        {
            try
            {
                var info = args.ManualPortPLCInfo;
                var readResult = info.CarrierIdReadResult;
                var stage1CarrierId = info.CarrierIdOfStage1;

                WriteEventLog($"PortName[{args.PortName}] BcrReadDone => ReadResult[{readResult}] CarrierIdOfStage1[{stage1CarrierId}]");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "");
            }
        }

        private void Port_OnWaitOut(object sender, ManualPortEventArgs args)
        {
            try
            {
                var info = args.ManualPortPLCInfo;
                var stage1CarrierId = info.CarrierIdOfStage1;

                WriteEventLog($"PortName[{args.PortName}] WaitOut => CarrierIdOfStage1[{stage1CarrierId}]");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "");
            }
        }

        private void Port_OnCstRemoved(object sender, ManualPortEventArgs args)
        {
            try
            {
                var info = args.ManualPortPLCInfo;
                var stage1CarrierId = info.CarrierIdOfStage1;

                WriteEventLog($"PortName[{args.PortName}] CstRemoved => CarrierIdOfStage1[{stage1CarrierId}]");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "");
            }
        }

        private void Port_OnDirectionChanged(object sender, ManualPortEventArgs args)
        {
            try
            {
                var newDirection = "";
                if (args.ManualPortPLCInfo.Direction == DirectionType.InMode)
                    newDirection += "DirectionChangeTo_InMode";
                else
                    newDirection += "DirectionChangeTo_OutMode";

                var info = args.ManualPortPLCInfo;
                var stage1CarrierId = info.CarrierIdOfStage1;

                WriteEventLog($"PortName[{args.PortName}] {newDirection} => CarrierIdOfStage1[{stage1CarrierId}]");

                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "");
            }
        }

        private void Port_OnInServiceChanged(object sender, ManualPortEventArgs args)
        {
            try
            {
                var newState = "";
                if (args.ManualPortPLCInfo.IsRun)
                    newState += "InService";
                else
                    newState += "OutOfService";

                var info = args.ManualPortPLCInfo;

                WriteEventLog($"PortName[{args.PortName}] InServiceChanged => Now state is {newState}. IsRun[{info.IsRun}] IsDown[{info.IsDown}] IsAlarm[{info.IsAlarm}]");

                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "");
            }
        }

        #region Alarm
        private void Port_OnAlarmHappen(object sender, ManualPortEventArgs args)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "");
            }
        }

        private void Port_OnAlarmClear(object sender, ManualPortEventArgs args)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "");
            }
        }
        #endregion Alarm
    }
}
