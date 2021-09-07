using com.mirle.ibg3k0.sc.BLL.Extensions;
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
                port.OnLoadPresenceChanged += Port_OnLoadPresenceChanged;
                port.OnWaitIn += Port_OnWaitIn;
                port.OnBcrReadDone += Port_OnBcrReadDone;
                port.OnWaitOut += Port_OnWaitOut;
                port.OnCstRemoved += Port_OnCstRemoved;
                port.OnDirectionChanged += Port_OnDirectionChanged;
                port.OnInServiceChanged += Port_OnInServiceChanged;
                port.OnAlarmHappen += Port_OnAlarmHappen;
                port.OnAlarmClear += Port_OnAlarmClear;
                port.OnDoorOpen += Port_OnDoorOpen;

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
                var stage1CarrierId = info.CarrierIdOfStage1.Trim();

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

            if (cassetteData == null)
                return;

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


            if (reportBll.ReportCarrierRemoveFromManualPort(cassetteData))
                WriteEventLog($"{logTitle} Report MCS carrier remove From manual port success.");
            else
                WriteEventLog($"{logTitle} Report MCS carrier remove From manual port Failed.");
            cassetteDataBLL.Delete(cassetteData.BOXID);
            WriteEventLog($"{logTitle} Delete carrier data [{cassetteData.BOXID}].");
        }
        #endregion LoadPresenceChanged

        #region Wait In
        private void Port_OnWaitIn(object sender, ManualPortEventArgs args)
        {
            var info = args.ManualPortPLCInfo;
            var readResult = info.CarrierIdReadResult;
            var stage1CarrierId = info.CarrierIdOfStage1.Trim();

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
                if (duplicateLocation.ToUnitType().IsShlef())
                    WaitInDuplicateAtShelfProcess(logTitle, portName, info, duplicateCarrierData);
                else
                    WaitInDuplicateAtPortProcess(logTitle, portName, info, duplicateCarrierData, duplicateLocation);
            }
            else
            {
                WaitInDuplicateAtOhtProcess(logTitle, portName, duplicateCarrierData);
            }
        }

        public string GetDuplicateUnknownId(string carrierId)
        {
            var year = DateTime.Now.Year % 100;
            var date = string.Format("{0}{1:00}{2:00}{3:00}{4:00}", year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute);
            var id = "UNKD" + carrierId + date + string.Format("{0:00}", DateTime.Now.Second);

            return id;
        }

        private void WaitInDuplicateAtShelfProcess(string logTitle, string portName, ManualPortPLCInfo info, CassetteData duplicateCarrierData)
        {
            WriteEventLog($"{logTitle} Duplicate at shelf ({duplicateCarrierData.Carrier_LOC}).");


            CheckDuplicateCarrier(logTitle, duplicateCarrierData, out var needRemoveDuplicateShelf, out var unknownId);

            var stage1CarrierId = info.CarrierIdOfStage1.Trim();

            if (needRemoveDuplicateShelf)
            {
                cassetteDataBLL.Install(portName, stage1CarrierId);
                WriteEventLog($"{logTitle} Install cassette data [{stage1CarrierId}] at this port.");
            }
            else
            {
                cassetteDataBLL.Install(portName, unknownId);
                WriteEventLog($"{logTitle} Install cassette data [{unknownId}] at this port.");
            }

            cassetteDataBLL.GetCarrierByPortName(portName, 1, out var cassetteData);
            ReportIDRead(logTitle, cassetteData, isDuplicate: true);

            //ReportIDRead(logTitle, cassetteData, isDuplicate: true);

            cassetteDataBLL.GetCarrierByPortName(duplicateCarrierData.Carrier_LOC, 1, out var duplicateData);

            if (needRemoveDuplicateShelf)
            {
                //ReportForcedCarrierRemove(logTitle, duplicateCarrierData);
                ReportCarrierRemovedCompletedForShelf(logTitle, duplicateCarrierData.BOXID, duplicateCarrierData.Carrier_LOC);
                //ReportInstallCarrier(logTitle, duplicateData);
                ReportInstallForShelf(logTitle, unknownId, duplicateCarrierData.Carrier_LOC);
            }

            ReportWaitIn(logTitle, cassetteData);
        }

        private void CheckDuplicateCarrier(string logTitle, CassetteData duplicateCarrierData, out bool needRemoveDuplicateShelf, out string unknownId)
        {
            needRemoveDuplicateShelf = true;

            unknownId = GetDuplicateUnknownId(duplicateCarrierData.BOXID);
            var duplicateCarrierhasNoCommand = commandBLL.GetCommandByBoxId(duplicateCarrierData.BOXID, out var command) == false;
            if (duplicateCarrierhasNoCommand)
            {
                ChageDuplicateLocationCarrierIdToUnknownId(logTitle, duplicateCarrierData, unknownId);
                return;
            }

            WriteEventLog($"{logTitle} Duplicate carrier has command [{command.CMD_ID}] now.");

            if (command.TRANSFERSTATE == E_TRAN_STATUS.Queue)
            {
                WriteEventLog($"{logTitle} Command state is queue.");

                commandBLL.Delete(duplicateCarrierData.BOXID);
                WriteEventLog($"{logTitle} Delete Command.");

                ChageDuplicateLocationCarrierIdToUnknownId(logTitle, duplicateCarrierData, unknownId);

                return;
            }

            needRemoveDuplicateShelf = false;

            WriteEventLog($"{logTitle} Command state is not queue.  Install UnknownID[{unknownId}] on this wait in port.");
        }

        private void ChageDuplicateLocationCarrierIdToUnknownId(string logTitle, CassetteData duplicateCarrierData, string unknownId)
        {
            //cassetteDataBLL.Delete(duplicateCarrierData.BOXID);
            DeleteForShelf(duplicateCarrierData.BOXID, duplicateCarrierData.Carrier_LOC);
            WriteEventLog($"{logTitle} Delete duplicate carrier.");

            //shelfDefBLL.SetStored(duplicateCarrierData.Carrier_LOC);
            //WriteEventLog($"{logTitle} Set shelf stage of duplicate shelf[{duplicateCarrierData.Carrier_LOC}] to stored.");

            //cassetteDataBLL.Install(duplicateCarrierData.Carrier_LOC, unknownId);
            InstallForShelf(unknownId, duplicateCarrierData.Carrier_LOC);
            WriteEventLog($"{logTitle} Install UnknownID[{unknownId}] on shelf[{duplicateCarrierData.Carrier_LOC}].");
        }
        private void DeleteForShelf(string carrierID, string carrierLoc)
        {
            cassetteDataBLL.Delete(carrierID);
            //reportBll.ReportCarrierRemovedCompletedForShelf(carrierID, carrierLoc);
        }
        private void InstallForShelf(string carrierID, string carrierLoc)
        {
            cassetteDataBLL.Install(carrierLoc, carrierID);
            //reportBll.ReportCarrierInstallCompletedForShelf(carrierID, carrierLoc);
        }


        private void WaitInDuplicateAtPortProcess(string logTitle, string portName, ManualPortPLCInfo info, CassetteData duplicateCarrierData, PortDef duplicatePort)
        {
            WriteEventLog($"{logTitle} Duplicate at Port ({duplicatePort.PLCPortID}).");

            if (commandBLL.GetCommandByBoxId(duplicateCarrierData.BOXID, out var command))
            {
                WriteEventLog($"{logTitle} Duplicate carrier has command [{command.CMD_ID}] now.");

                var unknownId = GetDuplicateUnknownId(duplicateCarrierData.BOXID);
                cassetteDataBLL.Install(portName, unknownId);
                WriteEventLog($"{logTitle} Install cassette data [{unknownId}] at this port.");

                cassetteDataBLL.GetCarrierByPortName(portName, 1, out var cassetteData);

                ReportIDRead(logTitle, cassetteData, isDuplicate: true);
                ReportWaitIn(logTitle, cassetteData);

                return;
            }

            cassetteDataBLL.Delete(duplicateCarrierData.BOXID);
            WriteEventLog($"{logTitle} Delete duplicate cassette data [{duplicateCarrierData.BOXID}].");

            var stage1CarrierId = info.CarrierIdOfStage1.Trim();

            cassetteDataBLL.Install(portName, stage1CarrierId);
            WriteEventLog($"{logTitle} Install cassette data [{stage1CarrierId}] Type[{info.CarrierType}] at this port.");

            cassetteDataBLL.GetCarrierByPortName(portName, 1, out var cassetteData2);

            ReportIDRead(logTitle, cassetteData2, isDuplicate: true);

            ReportForcedCarrierRemove(logTitle, duplicateCarrierData);

            ReportWaitIn(logTitle, cassetteData2);
        }

        private void WaitInDuplicateAtOhtProcess(string logTitle, string portName, CassetteData duplicateCarrierData)
        {
            WriteEventLog($"{logTitle} Duplicate at OHT ({duplicateCarrierData.Carrier_LOC}).");

            var unknownId = GetDuplicateUnknownId(duplicateCarrierData.BOXID);
            cassetteDataBLL.Install(portName, unknownId);
            WriteEventLog($"{logTitle} Install cassette data [{unknownId}] at this port.");

            cassetteDataBLL.GetCarrierByPortName(portName, 1, out var cassetteData);

            ReportIDRead(logTitle, cassetteData, isDuplicate: true);
            ReportWaitIn(logTitle, cassetteData);
        }

        private void WaitInNormalProcess(string logTitle, string portName, ManualPortPLCInfo info)
        {
            WriteEventLog($"{logTitle} Normal Process.");

            CheckResidualCassetteProcess(logTitle, portName);

            var stage1CarrierId = info.CarrierIdOfStage1.Trim();

            cassetteDataBLL.Install(portName, stage1CarrierId);
            WriteEventLog($"{logTitle} Install cassette data [{stage1CarrierId}] Type[{info.CarrierType}] at this port.");

            cassetteDataBLL.GetCarrierByPortName(portName, stage: 1, out var cassetteData);

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
            if (reportBll.ReportCarrierRemoveFromManualPort(cassetteData))
                WriteEventLog($"{logTitle} Report MCS CarrierRemoveComplete Success. CarrierId[{cassetteData.BOXID}]");
            else
                WriteEventLog($"{logTitle} Report MCS CarrierRemoveComplete Failed.  CarrierId[{cassetteData.BOXID}]");
        }
        private void ReportCarrierRemovedCompletedForShelf(string logTitle, string carrierID, string carrierLoc)
        {
            if (reportBll.ReportCarrierRemovedCompletedForShelf(carrierID, carrierLoc))
                WriteEventLog($"{logTitle} Report MCS CarrierRemoveComplete For Shelf Success. CarrierId[{carrierID}] ,loc:{carrierLoc}");
            else
                WriteEventLog($"{logTitle} Report MCS CarrierRemoveComplete For Shelf Failed.  CarrierId[{carrierID}] ,loc:{carrierLoc}");
        }

        private void ReportInstallCarrier(string logTitle, CassetteData cassetteData)
        {
            if (reportBll.ReportCarrierInstall(cassetteData))
                WriteEventLog($"{logTitle} Report MCS CarrierInstall Success. CarrierId[{cassetteData.BOXID}]");
            else
                WriteEventLog($"{logTitle} Report MCS CarrierInstall Failed.  CarrierId[{cassetteData.BOXID}]");
        }
        private void ReportInstallForShelf(string logTitle,string carrierID,string carrierLoc)
        {
            if (reportBll.ReportCarrierInstallCompletedForShelf(carrierID, carrierLoc))
                WriteEventLog($"{logTitle} Report MCS CarrierInstall for shelf Success. CarrierId[{carrierID}],loc[{carrierLoc}]");
            else
                WriteEventLog($"{logTitle} Report MCS CarrierInstall for shelf Failed.  CarrierId[{carrierID}],loc[{carrierLoc}]");
        }

        private void ReportIDRead(string logTitle, CassetteData cassetteData, bool isDuplicate)
        {
            if (reportBll.ReportCarrierIDRead(cassetteData, isDuplicate))
                WriteEventLog($"{logTitle} Report MCS CarrierIDRead Success. CarrierId[{cassetteData.BOXID}] isDuplicate[{isDuplicate}]");
            else
                WriteEventLog($"{logTitle} Report MCS CarrierIDRead Failed.  CarrierId[{cassetteData.BOXID}] isDuplicate[{isDuplicate}]");
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
                var readResult = info.CarrierIdReadResult.Trim();
                var stage1CarrierId = info.CarrierIdOfStage1.Trim();

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
                var stage1CarrierId = info.CarrierIdOfStage1.Trim();

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
                var stage1CarrierId = info.CarrierIdOfStage1.Trim();

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

                var logTitle = $"PortName[{args.PortName}] {newDirection} => ";

                var info = args.ManualPortPLCInfo;
                var stage1CarrierId = info.CarrierIdOfStage1.Trim();

                WriteEventLog($"{logTitle} CarrierIdOfStage1[{stage1CarrierId}]");

                CheckResidualCassetteProcess(logTitle, args.PortName);

                if (args.ManualPortPLCInfo.Direction == DirectionType.InMode)
                {
                    reportBll.ReportPortDirectionChanged(args.PortName, newDirectionIsInMode: true);
                    WriteEventLog($"{logTitle} Report MCS PortTypeChange InMode");

                    portDefBLL.ChangeDirectionToInMode(args.PortName);
                    WriteEventLog($"{logTitle} PortDef change direction to InMode");

                    if (manualPorts.TryGetValue(args.PortName, out var plcPort))
                    {
                        plcPort.ChangeToInModeAsync(isOn: false);
                        WriteEventLog($"{logTitle} OFF ChangeToInMode Signal");
                    }
                    else
                        WriteEventLog($"{logTitle} Cannot OFF ChangeToInMode Signal. Because cannot find IManualPortValueDefMapAction by portName[{args.PortName}]");
                }
                else
                {
                    reportBll.ReportPortDirectionChanged(args.PortName, newDirectionIsInMode: false);
                    WriteEventLog($"{logTitle} Report MCS PortTypeChange OutMode");

                    portDefBLL.ChangeDirectionToOutMode(args.PortName);
                    WriteEventLog($"{logTitle} PortDef change direction to OutMode");

                    if (manualPorts.TryGetValue(args.PortName, out var plcPort))
                    {
                        plcPort.ChangeToOutModeAsync(isOn: false);
                        WriteEventLog($"{logTitle} OFF ChangeToOutMode Signal");
                    }
                    else
                        WriteEventLog($"{logTitle} Cannot OFF ChangeToOutMode Signal. Because cannot find IManualPortValueDefMapAction by portName[{args.PortName}]");
                }
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
                var info = args.ManualPortPLCInfo;
                var logTitle = $"PortName[{args.PortName}] InServiceChanged => ";

                WriteEventLog($"{logTitle} IsRun[{info.IsRun}] IsDown[{info.IsDown}] IsAlarm[{info.IsAlarm}]");

                if (args.ManualPortPLCInfo.IsRun)
                {
                    reportBll.ReportPortInServiceChanged(args.PortName, newStateIsInService: true);
                    WriteEventLog($"{logTitle} Report MCS PortInService");
                }
                else
                {
                    reportBll.ReportPortInServiceChanged(args.PortName, newStateIsInService: false);
                    WriteEventLog($"{logTitle} Report MCS PortOutOfService");
                }
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
                var info = args.ManualPortPLCInfo;
                var portName = args.PortName;
                var logTitle = $"PortName[{args.PortName}] AlarmHappen => ";
                WriteEventLog($"{logTitle} AlarmIndex[{info.ErrorIndex}] AlarmCode[{info.AlarmCode}] IsRun[{info.IsRun}] IsDown[{info.IsDown}] IsAlarm[{info.IsAlarm}]");

                var alarmCode = info.AlarmCode.ToString().Trim();
                var commandOfPort = GetCommandOfPort(info);

                if (alarmBLL.SetAlarm(portName, alarmCode, commandOfPort, out var alarmReport, out var reasonOfAlarmSetFailed) == false)
                {
                    WriteEventLog($"{logTitle} AlarmCode[{info.AlarmCode}] Set Alarm failed. ({reasonOfAlarmSetFailed}) Cannot report MCS Alarm.");
                    return;
                }

                if (alarmReport.ALAM_LVL == E_ALARM_LVL.Error)
                {
                    reportBll.ReportAlarmSet(alarmReport);
                    WriteEventLog($"{logTitle} AlarmCode[{info.AlarmCode}] Alarm level is (Error). Report alarm set.");
                }
                else if (alarmReport.ALAM_LVL == E_ALARM_LVL.Warn)
                {
                    reportBll.ReportUnitAlarmSet(alarmReport);
                    WriteEventLog($"{logTitle} AlarmCode[{info.AlarmCode}] Alarm level is (Warn). Report unit alarm set.");
                }
                else
                    WriteEventLog($"{logTitle} AlarmCode[{info.AlarmCode}] Not reported because the alarm level is (None).  Should be (Error) or (Warn).");

                UpdateOHBCErrorIndex(logTitle, args);
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
                var info = args.ManualPortPLCInfo;
                var portName = args.PortName;
                var logTitle = $"PortName[{args.PortName}] AlarmClear => ";
                WriteEventLog($"{logTitle} AlarmIndex[{info.ErrorIndex}] AlarmCode[{info.AlarmCode}] IsRun[{info.IsRun}] IsDown[{info.IsDown}] IsAlarm[{info.IsAlarm}]");

                var alarmCode = info.AlarmCode.ToString().Trim();
                var commandOfPort = GetCommandOfPort(info);

                if (alarmBLL.ClearAllAlarm(portName, commandOfPort, out var alarmReports, out var reasonOfAlarmClearFailed) == false)
                {
                    WriteEventLog($"{logTitle} Clear all Alarm failed. ({reasonOfAlarmClearFailed}). Cannot report MCS Alarm.");
                    return;
                }

                foreach (var alarm in alarmReports)
                {
                    if (alarm.ALAM_LVL == E_ALARM_LVL.Error)
                    {
                        reportBll.ReportAlarmClear(alarm);
                        WriteEventLog($"{logTitle} AlarmCode[{info.AlarmCode}] Alarm level is (Error). Report alarm clear.");
                    }
                    else if (alarm.ALAM_LVL == E_ALARM_LVL.Warn)
                    {
                        reportBll.ReportUnitAlarmClear(alarm);
                        WriteEventLog($"{logTitle} AlarmCode[{info.AlarmCode}] Alarm level is (Warn). Report unit alarm clear.");
                    }
                    else
                        WriteEventLog($"{logTitle} AlarmCode[{info.AlarmCode}] Not reported because the alarm level is (None).  Should be (Error) or (Warn).");
                }

                UpdateOHBCErrorIndex(logTitle, args);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "");
            }
        }

        private void UpdateOHBCErrorIndex(string logTitle, ManualPortEventArgs args)
        {
            var info = args.ManualPortPLCInfo;

            if (manualPorts.TryGetValue(args.PortName, out var plcAction))
            {
                plcAction.SetControllerErrorIndexAsync(info.ErrorIndex);
                WriteEventLog($"{logTitle} Set OHBC AlarmIndex to [{info.ErrorIndex}]");
            }
            else
                WriteEventLog($"{logTitle} Set OHBC AlarmIndex to [{info.ErrorIndex}] Failed. Cannot find IManualPortValueDefMapAction by PortName[{args.PortName}]");
        }

        private ACMD_MCS GetCommandOfPort(ManualPortPLCInfo info)
        {
            var stage1CarrierId = info.CarrierIdOfStage1.Trim();
            var hasCommand = commandBLL.GetCommandByBoxId(stage1CarrierId, out var commandOfPort);
            if (hasCommand == false)
            {
                commandOfPort = new ACMD_MCS();
                commandOfPort.CMD_ID = "";
                commandOfPort.BOX_ID = "";
            }

            return commandOfPort;
        }
        #endregion Alarm

        private void Port_OnDoorOpen(object sender, ManualPortEventArgs args)
        {
            try
            {
                var info = args.ManualPortPLCInfo;
                var logTitle = $"PortName[{args.PortName}] DoorOpenChanged => ";

                if (info.IsDoorOpen)
                    WriteEventLog($"{logTitle} Door Open");
                else
                    WriteEventLog($"{logTitle} Door Close");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "");
            }
        }
    }
}
