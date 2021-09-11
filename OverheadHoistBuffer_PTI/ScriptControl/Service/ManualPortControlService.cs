using com.mirle.ibg3k0.sc.BLL.Interface;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.PLC_Functions.MGV;
using com.mirle.ibg3k0.sc.Data.PLC_Functions.MGV.Enums;
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction.Interface;
using com.mirle.ibg3k0.sc.Service.Interface;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace com.mirle.ibg3k0.sc.Service
{
    public class ManualPortControlService : IManualPortControlService
    {
        private Logger logger = LogManager.GetLogger("ManualPortLogger");
        private string now { get => DateTime.Now.ToString("HH:mm:ss.fff"); }
        private ConcurrentDictionary<string, IManualPortValueDefMapAction> manualPorts { get; set; }
        private ConcurrentDictionary<string, string> comingOutCarrierOfManualPorts { get; set; }
        private ConcurrentDictionary<string, List<string>> readyToWaitOutCarrierOfManualPorts { get; set; }
        private ConcurrentDictionary<string, Stopwatch> stopWatchForCheckCommandingSignal { get; set; }

        private const string NO_CST = " - ";

        private const int timeoutElapsedMillisecondsForOffCommandingSignal = 12_000;

        private IManualPortCassetteDataBLL cassetteDataBLL;

        public int TimeOutForMoveBack { get; set; } = 30;

        public ManualPortControlService()
        {
            WriteLog($"ManualPortControlService Initial");
            //RegisterEvent(ports);
        }
        public void Start(IEnumerable<IManualPortValueDefMapAction> ports, IManualPortCassetteDataBLL cassetteDataBLL)
        {
            WriteLog($"ManualPortControlService Start");
            this.cassetteDataBLL = cassetteDataBLL;
            RegisterPort(ports);
        }

        private void RegisterPort(IEnumerable<IManualPortValueDefMapAction> ports)
        {
            manualPorts = new ConcurrentDictionary<string, IManualPortValueDefMapAction>();
            comingOutCarrierOfManualPorts = new ConcurrentDictionary<string, string>();
            readyToWaitOutCarrierOfManualPorts = new ConcurrentDictionary<string, List<string>>();
            stopWatchForCheckCommandingSignal = new ConcurrentDictionary<string, Stopwatch>();

            foreach (var port in ports)
            {
                manualPorts.TryAdd(port.PortName, port);
                WriteLog($"Add Manual Port Control Success ({port.PortName})");


                comingOutCarrierOfManualPorts.TryAdd(port.PortName, string.Empty);
                WriteLog($"Add Manual Port Control Coming Out Carrier Of Manual Ports Success ({port.PortName})");

                readyToWaitOutCarrierOfManualPorts.TryAdd(port.PortName, new List<string>());
                WriteLog($"Add Manual Port Control Ready To Wait Out Carrier Of Manual Ports Success ({port.PortName})");

                stopWatchForCheckCommandingSignal.TryAdd(port.PortName, new Stopwatch());
                WriteLog($"Add Manual Port Control Stopwatch Of Manual Ports For Check Commanding Signal Success ({port.PortName})");
            }
        }


        #region Log

        private void WriteLog(string message)
        {
            var logMessage = $"[{now}] {message}";
            logger.Info(logMessage);
        }

        #endregion Log

        public void RefreshState()
        {
            var allCommands = ACMD_MCS.MCS_CMD_InfoList;

            RefreshPlcMonitor(allCommands);
            RefreshReadyToWaitOutCarrier();
            RefreshComingOutCarrier();
            CheckCommandingSignal(allCommands);
            CheckMoveInCassetteTimedOut();
        }

        private void RefreshPlcMonitor(ConcurrentDictionary<string, ACMD_MCS> allCommands)
        {
            try
            {
                foreach (var portItem in manualPorts)
                {
                    var portName = portItem.Key;

                    readyToWaitOutCarrierOfManualPorts[portName].Clear();

                    var commandsOfManualPort = allCommands.Where(c => c.Value.HOSTDESTINATION == portName);

                    var manualPortcommands = new List<ACMD_MCS>();

                    foreach (var commandItem in commandsOfManualPort)
                        manualPortcommands.Add(commandItem.Value);

                    DescendingSortByPriority(manualPortcommands);

                    foreach (var command in manualPortcommands)
                    {
                        if (command.TRANSFERSTATE == E_TRAN_STATUS.Queue || string.IsNullOrEmpty(command.RelayStation) == false)
                        {
                            if (readyToWaitOutCarrierOfManualPorts[portName].Count < 2)
                                readyToWaitOutCarrierOfManualPorts[portName].Add(command.BOX_ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }

        private void RefreshReadyToWaitOutCarrier()
        {
            foreach (var item in readyToWaitOutCarrierOfManualPorts)
            {
                if (item.Value.Count == 0)
                {
                    manualPorts[item.Key].ShowReadyToWaitOutCarrierOnMonitorAsync(NO_CST, NO_CST);
                }
                else if (item.Value.Count == 1)
                {
                    manualPorts[item.Key].ShowReadyToWaitOutCarrierOnMonitorAsync(item.Value[0], NO_CST);
                    WriteLog($"{item.Key} Has one carrier that ready to WaitOut. Show PLC Monitor ({item.Value[0]})(NO_CST).");
                }
                else
                {
                    manualPorts[item.Key].ShowReadyToWaitOutCarrierOnMonitorAsync(item.Value[0], item.Value[1]);
                    WriteLog($"{item.Key} Has two carrier that ready to WaitOut. Show PLC Monitor ({item.Value[0]})({item.Value[1]}).");
                }
            }
        }

        private void RefreshComingOutCarrier()
        {
            var commandsOfOHT = ACMD_OHTC.CMD_OHTC_InfoList;

            foreach (var portItem in manualPorts)
            {
                var portName = portItem.Key;
                comingOutCarrierOfManualPorts[portName] = string.Empty;

                var cmds = commandsOfOHT.Where(c => SCUtility.isMatche(c.Value.DESTINATION, portName)).Select(c => c.Value).ToList();

                if (cmds == null)
                    continue;

                var cmd = cmds.FirstOrDefault();

                if (cmd == null)
                    continue;

                comingOutCarrierOfManualPorts[portName] = cmd.BOX_ID;
            }

            foreach (var item in comingOutCarrierOfManualPorts)
            {
                if (string.IsNullOrEmpty(item.Value))
                    manualPorts[item.Key].ShowComingOutCarrierOnMonitorAsync(NO_CST);
                else
                {
                    WriteLog($"{item.Key} Has carrier coming out. Show PLC Monitor ({item.Value}).");
                    manualPorts[item.Key].ShowComingOutCarrierOnMonitorAsync(item.Value);
                }
            }
        }

        protected void DescendingSortByPriority(List<ACMD_MCS> allCommands)
        {
            allCommands.Sort((x, y) =>
            {
                return -x.PRIORITY_SUM.CompareTo(y.PRIORITY_SUM);
            });
        }

        private void CheckCommandingSignal(ConcurrentDictionary<string, ACMD_MCS> allCommands)
        {
            try
            {
                foreach (var portItem in manualPorts)
                {
                    var portName = portItem.Key;

                    var commandsOfManualPort = allCommands.Where(c => c.Value.HOSTDESTINATION == portName || c.Value.HOSTSOURCE == portName);

                    if (commandsOfManualPort != null || commandsOfManualPort.Count() > 0)
                    {
                        stopWatchForCheckCommandingSignal[portName].Reset();
                        stopWatchForCheckCommandingSignal[portName].Stop();

                        continue;
                    }

                    if (stopWatchForCheckCommandingSignal[portName].IsRunning)
                    {
                        if (stopWatchForCheckCommandingSignal[portName].ElapsedMilliseconds > timeoutElapsedMillisecondsForOffCommandingSignal)
                        {
                            portItem.Value.SetCommandingAsync(setOn: false);
                            stopWatchForCheckCommandingSignal[portName].Reset();
                        }
                    }
                    else
                        stopWatchForCheckCommandingSignal[portName].Start();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }

        private void CheckMoveInCassetteTimedOut()
        {
            try
            {
                foreach (var portItem in manualPorts)
                {
                    //Move back cassette which is timed out
                    var portName = portItem.Key;
                    CassetteData cassetteOnPort;
                    bool hasCassette = cassetteDataBLL.GetCarrierByPortName(portName, stage: 1, out cassetteOnPort);

                    if (hasCassette && portItem.Value.PortDirection == DirectionType.InMode)
                    {
                        TimeSpan timeSpan = DateTime.Now - DateTime.Parse(cassetteOnPort.TrnDT);
                        if (timeSpan.TotalSeconds > TimeOutForMoveBack)
                        {
                            WriteLog($"{portName} has cassette {cassetteOnPort.BOXID} which is already timed out for move in. Move back.");
                            //SetMoveBackReason(portName, MoveBackReasons.MoveInTimedOut);
                            MoveBack(portName, MoveBackReasons.MoveInTimedOut);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }

        public bool GetPortPlcState(string portName, out ManualPortPLCInfo info)
        {
            info = null;

            if (manualPorts.ContainsKey(portName) == false)
            {
                WriteLog($"{MethodBase.GetCurrentMethod().Name}({portName}) => Cannot Find this port");
                return false;
            }

            info = manualPorts[portName].GetPortState() as ManualPortPLCInfo;
            WriteLog($"{MethodBase.GetCurrentMethod().Name}({portName})");
            return true;
        }

        public bool ChangeToInMode(string portName)
        {
            if (manualPorts.ContainsKey(portName) == false)
            {
                WriteLog($"{MethodBase.GetCurrentMethod().Name}({portName}) => Cannot Find this port");
                return false;
            }

            manualPorts[portName].ChangeToInModeAsync(true);
            WriteLog($"{MethodBase.GetCurrentMethod().Name}({portName})");
            return true;
        }

        public bool ChangeToOutMode(string portName)
        {
            if (manualPorts.ContainsKey(portName) == false)
            {
                WriteLog($"{MethodBase.GetCurrentMethod().Name}({portName}) => Cannot Find this port");
                return false;
            }

            manualPorts[portName].ChangeToOutModeAsync(true);
            WriteLog($"{MethodBase.GetCurrentMethod().Name}({portName})");
            return true;
        }

        public bool MoveBack(string portName)
        {
            if (manualPorts.ContainsKey(portName) == false)
            {
                WriteLog($"{MethodBase.GetCurrentMethod().Name}({portName}) => Cannot Find this port");
                return false;
            }

            manualPorts[portName].MoveBackAsync();
            WriteLog($"{MethodBase.GetCurrentMethod().Name}({portName})");
            return true;
        }

        public bool MoveBack(string portName, MoveBackReasons reason)
        {
            if (manualPorts.ContainsKey(portName) == false)
            {
                WriteLog($"{MethodBase.GetCurrentMethod().Name}({portName}) => Cannot Find this port");
                return false;
            }

            manualPorts[portName].MoveBackAsync(reason);
            WriteLog($"{MethodBase.GetCurrentMethod().Name}({portName})");
            return true;
        }

        public bool SetMoveBackReason(string portName, MoveBackReasons reason)
        {
            if (manualPorts.ContainsKey(portName) == false)
            {
                WriteLog($"{MethodBase.GetCurrentMethod().Name}({portName}) => Cannot Find this port");
                return false;
            }

            manualPorts[portName].SetMoveBackReasonAsync(reason);
            WriteLog($"{MethodBase.GetCurrentMethod().Name}({portName})");
            return true;
        }

        public bool ResetAlarm(string portName)
        {
            if (manualPorts.ContainsKey(portName) == false)
            {
                WriteLog($"{MethodBase.GetCurrentMethod().Name}({portName}) => Cannot Find this port");
                return false;
            }

            manualPorts[portName].ResetAlarmAsync();
            WriteLog($"{MethodBase.GetCurrentMethod().Name}({portName})");
            return true;
        }

        public bool StopBuzzer(string portName)
        {
            if (manualPorts.ContainsKey(portName) == false)
            {
                WriteLog($"{MethodBase.GetCurrentMethod().Name}({portName}) => Cannot Find this port");
                return false;
            }

            manualPorts[portName].StopBuzzerAsync();
            WriteLog($"{MethodBase.GetCurrentMethod().Name}({portName})");
            return true;
        }

        public bool SetRun(string portName)
        {
            if (manualPorts.ContainsKey(portName) == false)
            {
                WriteLog($"{MethodBase.GetCurrentMethod().Name}({portName}) => Cannot Find this port");
                return false;
            }

            manualPorts[portName].SetRunAsync();
            WriteLog($"{MethodBase.GetCurrentMethod().Name}({portName})");
            return true;
        }

        public bool SetStop(string portName)
        {
            if (manualPorts.ContainsKey(portName) == false)
            {
                WriteLog($"{MethodBase.GetCurrentMethod().Name}({portName}) => Cannot Find this port");
                return false;
            }

            manualPorts[portName].SetStopAsync();
            WriteLog($"{MethodBase.GetCurrentMethod().Name}({portName})");
            return true;
        }

        public bool SetCommanding(string portName, bool setOn)
        {
            if (manualPorts.ContainsKey(portName) == false)
            {
                WriteLog($"{MethodBase.GetCurrentMethod().Name}({portName}) => Cannot Find this port");
                return false;
            }

            manualPorts[portName].SetCommandingAsync(setOn);
            WriteLog($"{MethodBase.GetCurrentMethod().Name}({portName})");
            return true;
        }

        public bool SetControllerErrorIndex(string portName, int newIndex)
        {
            if (manualPorts.ContainsKey(portName) == false)
            {
                WriteLog($"{MethodBase.GetCurrentMethod().Name}({portName}) => Cannot Find this port");
                return false;
            }

            manualPorts[portName].SetControllerErrorIndexAsync(newIndex);
            WriteLog($"{MethodBase.GetCurrentMethod().Name}({portName})");
            return true;
        }
    }
}
