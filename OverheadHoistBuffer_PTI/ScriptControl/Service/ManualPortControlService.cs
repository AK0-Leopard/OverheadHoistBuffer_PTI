using com.mirle.ibg3k0.sc.Data.PLC_Functions.MGV;
using com.mirle.ibg3k0.sc.Data.PLC_Functions.MGV.Enums;
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction.Interface;
using com.mirle.ibg3k0.sc.Service.Interface;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace com.mirle.ibg3k0.sc.Service
{
    public class ManualPortControlService : IManualPortControlService
    {
        private Logger logger = LogManager.GetLogger("ManualPortLogger");

        private ConcurrentDictionary<string, IManualPortValueDefMapAction> manualPorts { get; set; }

        public ManualPortControlService()
        {
            WriteLog($"ManualPortControlService Initial");
            //RegisterEvent(ports);
        }
        public void Start(IEnumerable<IManualPortValueDefMapAction> ports)
        {
            WriteLog($"ManualPortControlService Start");
            RegisterEvent(ports);
        }

        private void RegisterEvent(IEnumerable<IManualPortValueDefMapAction> ports)
        {
            manualPorts = new ConcurrentDictionary<string, IManualPortValueDefMapAction>();

            foreach (var port in ports)
            {
                manualPorts.TryAdd(port.PortName, port);
                WriteLog($"Add Manual Port Control Success ({port.PortName})");
            }
        }

        #region Log

        private void WriteLog(string message)
        {
            var logMessage = DateTime.Now.ToString("HH:mm:ss.fff ") + message;
            logger.Info(logMessage);
        }

        #endregion Log

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

            manualPorts[portName].ChangeToInModeAsync();
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

            manualPorts[portName].ChangeToOutModeAsync();
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
