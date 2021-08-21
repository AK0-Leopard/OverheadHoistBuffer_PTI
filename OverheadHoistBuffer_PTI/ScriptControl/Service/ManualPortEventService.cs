using com.mirle.ibg3k0.sc.BLL.Interface;
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

        private readonly IManualPortReportBLL reportBll;
        private readonly IManualPortCMDBLL commandBLL;
        private readonly IManualPortAlarmBLL alarmBLL;
        private readonly IManualPortDefBLL portDefBLL;
        private readonly IManualPortShelfDefBLL shelfDefBLL;
        private readonly IManualPortCassetteDataBLL cassetteDataBLL;

        public ManualPortEventService(IEnumerable<IManualPortValueDefMapAction> ports,
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

            WriteLog($"ManualPortEventService Initial");

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

        private void Port_OnLoadPresenceChanged(object sender, ManualPortEventArgs args)
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

        private void Port_OnWaitIn(object sender, ManualPortEventArgs args)
        {
            var info = args.ManualPortPLCInfo;

            if (cassetteDataBLL.GetCarrierByBoxId(info.CarrierIdOfStage1, out var duplicateCarrierId))
            {
                return;
            }

            var cassetteData = new CassetteData();
            cassetteData.BOXID = info.CarrierIdOfStage1;
            reportBll.ReportCarrierWaitIn(cassetteData, isDuplicate: false);
        }

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
                throw new NotImplementedException();
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
    }
}
