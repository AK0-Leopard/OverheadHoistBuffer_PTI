//*********************************************************************************
//      DefaultValueDefMapAction.cs
//*********************************************************************************
// File Name: MGVDefaultValueDefMapAction.cs
// Description: Port Scenario 
//
//(c) Copyright 2013, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//**********************************************************************************
using System;
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction.Events;
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction.Interface;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Data.PLC_Functions.MGV;
using com.mirle.ibg3k0.sc.Data.VO;
using NLog;
using System.Threading.Tasks;
using com.mirle.ibg3k0.sc.Data.PLC_Functions.MGV.Enums;
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction.Extension;

namespace com.mirle.ibg3k0.sc.Data.ValueDefMapAction
{
    public class MGVDefaultValueDefMapAction : IManualPortValueDefMapAction
    {
        #region Implement
        public event ManualPortEvents.ManualPortEventHandler OnWaitIn;
        public event ManualPortEvents.ManualPortEventHandler OnWaitOut;
        public event ManualPortEvents.ManualPortEventHandler OnDirectionChanged;
        public event ManualPortEvents.ManualPortEventHandler OnInServiceChanged;
        public event ManualPortEvents.ManualPortEventHandler OnBcrReadDone;
        public event ManualPortEvents.ManualPortEventHandler OnCstRemoved;
        public event ManualPortEvents.ManualPortEventHandler OnLoadPresenceChanged;
        public event ManualPortEvents.ManualPortEventHandler OnAlarmHappen;
        public event ManualPortEvents.ManualPortEventHandler OnAlarmClear;
        public event ManualPortEvents.ManualPortEventHandler OnDoorOpen;

        public string PortName { get => port.PORT_ID; }
        public DirectionType PortDirection { get; private set; }
        #endregion Implement

        protected MANUAL_PORTSTATION port = null;
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private SCApplication scApp = null;
        private BCFApplication bcfApp = null;

        protected String[] recipeIDNodes = null;

        public MGVDefaultValueDefMapAction()
            : base()
        {
            scApp = SCApplication.getInstance();
            bcfApp = scApp.getBCFApplication();
        }

        public string getIdentityKey()
        {
            return this.GetType().Name;
        }

        public void setContext(BaseEQObject baseEQ)
        {
            this.port = baseEQ as MANUAL_PORTSTATION;
        }

        public void unRegisterEvent()
        {
        }

        public void doShareMemoryInit(BCFAppConstants.RUN_LEVEL runLevel)
        {
            try
            {
                switch (runLevel)
                {
                    case BCFAppConstants.RUN_LEVEL.ZERO:

                        break;
                    case BCFAppConstants.RUN_LEVEL.ONE:
                        break;
                    case BCFAppConstants.RUN_LEVEL.TWO:
                        break;
                    case BCFAppConstants.RUN_LEVEL.NINE:
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }

        /// <summary>
        /// Does the initialize.
        /// </summary>
        public void doInit()
        {
            try
            {
                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "MGV_TO_OHxC_RUN", out ValueRead vr1))
                {
                    vr1.afterValueChange += (_sender, e) => MGV_Status_Change_RUN(_sender, e);
                }

                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "MGV_TO_OHxC_DOWN", out ValueRead vr2))
                {
                    vr2.afterValueChange += (_sender, e) => MGV_Status_Change_DOWN(_sender, e);
                }

                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "MGV_TO_OHxC_FAULT", out ValueRead vr3))
                {
                    vr3.afterValueChange += (_sender, e) => MGV_Status_Change_FAULT(_sender, e);
                }

                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "MGV_TO_OHxC_OUTMODE", out ValueRead vr4))
                {
                    vr4.afterValueChange += (_sender, e) => MGV_Status_Change_to_OutMode(_sender, e);
                }

                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "MGV_TO_OHxC_INMODE", out ValueRead vr5))
                {
                    vr5.afterValueChange += (_sender, e) => MGV_Status_Change_to_InMode(_sender, e);
                }

                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "MGV_TO_OHxC_WAITIN", out ValueRead vr6))
                {
                    vr6.afterValueChange += (_sender, e) => MGV_Status_WaitIn(_sender, e);
                }

                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "MGV_TO_OHxC_WAITOUT", out ValueRead vr7))
                {
                    vr7.afterValueChange += (_sender, e) => MGV_Status_WaitOut(_sender, e);
                }

                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "MGV_TO_OHxC_LOADPRESENCE1", out ValueRead vr8))
                {
                    vr8.afterValueChange += (_sender, e) => MGV_Status_Stage1_PresenceChanged(_sender, e);
                }

                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "MGV_TO_OHxC_BCRREADDONE", out ValueRead vr9))
                {
                    vr9.afterValueChange += (_sender, e) => MGV_Status_BcrReadDone(_sender, e);
                }

                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "MGV_TO_OHxC_REMOVECHECK", out ValueRead vr10))
                {
                    vr10.afterValueChange += (_sender, e) => MGV_Status_RemoveCheck(_sender, e);
                }

                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "MGV_TO_OHxC_ERRORINDEX", out ValueRead vr11))
                {
                    vr11.afterValueChange += (_sender, e) => MGV_Status_ErrorIndexChanged(_sender, e);
                }
                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "MGV_TO_OHxC_DOOROPEN", out ValueRead vr12))
                {
                    vr12.afterValueChange += (_sender, e) => MGV_Status_DoorOpenChanged(_sender, e);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }

        #region State
        private void MGV_Status_Change_RUN(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<ManualPortPLCInfo>(port.PORT_ID) as ManualPortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
                //2.read log
                logger.Info(function.ToString());

                if (function.IsRun)
                    OnInServiceChanged?.Invoke(this, new ManualPortEventArgs(function));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<ManualPortPLCInfo>(function);
            }
        }

        private void MGV_Status_Change_DOWN(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<ManualPortPLCInfo>(port.PORT_ID) as ManualPortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
                //2.read log
                logger.Info(function.ToString());

                if (function.IsDown)
                    OnInServiceChanged?.Invoke(this, new ManualPortEventArgs(function));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<ManualPortPLCInfo>(function);
            }
        }

        private void MGV_Status_Change_FAULT(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<ManualPortPLCInfo>(port.PORT_ID) as ManualPortPLCInfo;

            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);

                //2.read log
                logger.Info(function.ToString());

                if (function.IsAlarm)
                    OnInServiceChanged?.Invoke(this, new ManualPortEventArgs(function));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<ManualPortPLCInfo>(function);
            }
        }
        #endregion State

        #region Direction

        private void MGV_Status_Change_to_OutMode(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<ManualPortPLCInfo>(port.PORT_ID) as ManualPortPLCInfo;

            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);

                //2.read log
                logger.Info(function.ToString());

                PortDirection = DirectionType.OutMode;
                OnDirectionChanged?.Invoke(this, new ManualPortEventArgs(function));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<ManualPortPLCInfo>(function);
            }
        }

        private void MGV_Status_Change_to_InMode(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<ManualPortPLCInfo>(port.PORT_ID) as ManualPortPLCInfo;

            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);

                //2.read log
                logger.Info(function.ToString());
                PortDirection = DirectionType.InMode;
                OnDirectionChanged?.Invoke(this, new ManualPortEventArgs(function));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<ManualPortPLCInfo>(function);
            }
        }

        #endregion Direction

        #region In

        private void MGV_Status_BcrReadDone(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<ManualPortPLCInfo>(port.PORT_ID) as ManualPortPLCInfo;

            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);

                //2.read log
                logger.Info(function.ToString());

                if (function.IsBcrReadDone)
                    OnBcrReadDone?.Invoke(this, new ManualPortEventArgs(function));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<ManualPortPLCInfo>(function);
            }
        }

        private void MGV_Status_WaitIn(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<ManualPortPLCInfo>(port.PORT_ID) as ManualPortPLCInfo;

            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);

                //2.read log
                logger.Info(function.ToString());

                if (function.IsWaitIn)
                    OnWaitIn?.Invoke(this, new ManualPortEventArgs(function));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<ManualPortPLCInfo>(function);
            }
        }

        #endregion In

        #region Out

        private void MGV_Status_WaitOut(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<ManualPortPLCInfo>(port.PORT_ID) as ManualPortPLCInfo;

            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);

                //2.read log
                logger.Info(function.ToString());

                if (function.IsWaitOut)
                    OnWaitOut?.Invoke(this, new ManualPortEventArgs(function));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<ManualPortPLCInfo>(function);
            }
        }

        private void MGV_Status_RemoveCheck(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<ManualPortPLCInfo>(port.PORT_ID) as ManualPortPLCInfo;

            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);

                //2.read log
                logger.Info(function.ToString());

                if (function.IsRemoveCheck)
                    OnCstRemoved?.Invoke(this, new ManualPortEventArgs(function));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<ManualPortPLCInfo>(function);
            }
        }

        #endregion Out

        private void MGV_Status_Stage1_PresenceChanged(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<ManualPortPLCInfo>(port.PORT_ID) as ManualPortPLCInfo;

            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);

                //2.read log
                logger.Info(function.ToString());

                OnLoadPresenceChanged?.Invoke(this, new ManualPortEventArgs(function));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<ManualPortPLCInfo>(function);
            }
        }

        private void MGV_Status_ErrorIndexChanged(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<ManualPortPLCInfo>(port.PORT_ID) as ManualPortPLCInfo;

            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);

                //2.read log
                logger.Info(function.ToString());

                var alarmCode = function.AlarmCode;
                if (alarmCode == 0)
                {
                    OnAlarmClear?.Invoke(this, new ManualPortEventArgs(function));
                }
                else if (function.IsRun)
                {
                    WarningHappen(function);
                }
                else
                {
                    AlarmHappen(function);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<ManualPortPLCInfo>(function);
            }
        }

        private void WarningHappen(ManualPortPLCInfo function)
        {
            OnAlarmHappen?.Invoke(this, new ManualPortEventArgs(function));
        }

        private void AlarmHappen(ManualPortPLCInfo function)
        {
            OnAlarmHappen?.Invoke(this, new ManualPortEventArgs(function));
        }

        private void MGV_Status_DoorOpenChanged(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<ManualPortPLCInfo>(port.PORT_ID) as ManualPortPLCInfo;

            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);

                //2.read log
                logger.Info(function.ToString());

                OnDoorOpen?.Invoke(this, new ManualPortEventArgs(function));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<ManualPortPLCInfo>(function);
            }
        }
        #region Control

        public Task SetMoveBackReasonAsync(MoveBackReasons reason)
        {
            return Task.Run(() =>
            {
                var function = scApp.getFunBaseObj<ManualPortPLCControl>(port.PORT_ID) as ManualPortPLCControl;
                function.MoveBackReason = (ushort)reason;
                CommitChange(function);
            });
        }

        public Task ChangeToInModeAsync(bool isOn)
        {
            return Task.Run(() =>
            {
                var function = scApp.getFunBaseObj<ManualPortPLCControl>(port.PORT_ID) as ManualPortPLCControl;
                function.IsChangeToInMode = isOn;
                CommitChange(function);
            });
        }

        public Task ChangeToOutModeAsync(bool isOn)
        {
            return Task.Run(() =>
            {
                var function = scApp.getFunBaseObj<ManualPortPLCControl>(port.PORT_ID) as ManualPortPLCControl;
                function.IsChangeToOutMode = isOn;
                CommitChange(function);
            });
        }

        public Task MoveBackAsync()
        {
            return Task.Run(() =>
            {
                var function = scApp.getFunBaseObj<ManualPortPLCControl>(port.PORT_ID) as ManualPortPLCControl;
                function.IsMoveBack = true;
                CommitChange(function);

                //此訊號不會由PLC off，需由OHBC切換
                Task.Delay(3_000).Wait();

                function = scApp.getFunBaseObj<ManualPortPLCControl>(port.PORT_ID) as ManualPortPLCControl;
                function.IsMoveBack = false;
                CommitChange(function);
            });
        }

        public Task ResetAlarmAsync()
        {
            return Task.Run(() =>
            {
                var function = scApp.getFunBaseObj<ManualPortPLCControl>(port.PORT_ID) as ManualPortPLCControl;
                function.IsResetOn = true;
                CommitChange(function);

                //此訊號不會由PLC off，需由OHBC切換
                Task.Delay(3_000).Wait();

                function = scApp.getFunBaseObj<ManualPortPLCControl>(port.PORT_ID) as ManualPortPLCControl;
                function.IsResetOn = false;
                CommitChange(function);
            });
        }

        public Task StopBuzzerAsync()
        {
            return Task.Run(() =>
            {
                var function = scApp.getFunBaseObj<ManualPortPLCControl>(port.PORT_ID) as ManualPortPLCControl;
                function.IsBuzzerStop = true;
                CommitChange(function);

                //此訊號不會由PLC off，需由OHBC切換
                Task.Delay(3_000).Wait();

                function = scApp.getFunBaseObj<ManualPortPLCControl>(port.PORT_ID) as ManualPortPLCControl;
                function.IsBuzzerStop = false;
                CommitChange(function);
            });
        }

        public Task SetRunAsync()
        {
            return Task.Run(() =>
            {
                var function = scApp.getFunBaseObj<ManualPortPLCControl>(port.PORT_ID) as ManualPortPLCControl;
                function.IsSetRun = true;
                CommitChange(function);

                //此訊號不會由PLC off，需由OHBC切換
                Task.Delay(3_000).Wait();

                function = scApp.getFunBaseObj<ManualPortPLCControl>(port.PORT_ID) as ManualPortPLCControl;
                function.IsSetRun = false;
                CommitChange(function);
            });
        }

        public Task SetStopAsync()
        {
            return Task.Run(() =>
            {
                var function = scApp.getFunBaseObj<ManualPortPLCControl>(port.PORT_ID) as ManualPortPLCControl;
                function.IsSetStop = true;
                CommitChange(function);

                //此訊號不會由PLC off，需由OHBC切換
                Task.Delay(3_000).Wait();

                function = scApp.getFunBaseObj<ManualPortPLCControl>(port.PORT_ID) as ManualPortPLCControl;
                function.IsSetStop = false;
                CommitChange(function);
            });
        }

        public Task SetCommandingAsync(bool setOn)
        {
            return Task.Run(() =>
            {
                var function = scApp.getFunBaseObj<ManualPortPLCControl>(port.PORT_ID) as ManualPortPLCControl;
                function.IsCommanding = setOn;
                CommitChange(function);
            });
        }

        public Task SetControllerErrorIndexAsync(int newIndex)
        {
            {
                return Task.Run(() =>
                {
                    var function = scApp.getFunBaseObj<ManualPortPLCControl>(port.PORT_ID) as ManualPortPLCControl;

                    if (newIndex <= 65535)
                        function.OhbcErrorIndex = (UInt16)(newIndex);
                    else
                        function.OhbcErrorIndex = 1;

                    CommitChange(function);
                });
            }
        }

        public Task ShowReadyToWaitOutCarrierOnMonitorAsync(string carrierId_1, string carrierId_2)
        {
            return Task.Run(() =>
            {
                var function = scApp.getFunBaseObj<ManualPortPLCControl>(port.PORT_ID) as ManualPortPLCControl;

                carrierId_1 = carrierId_1.Trim();
                carrierId_2 = carrierId_2.Trim();

                if (carrierId_1.Length > 14)
                    carrierId_1 = carrierId_1.Substring(0, 14);

                if (carrierId_2.Length > 14)
                    carrierId_2 = carrierId_2.Substring(0, 14);

                function.ReadyToWaitOutCarrierId1 = carrierId_1;
                function.ReadyToWaitOutCarrierId2 = carrierId_2;

                CommitChange(function);
            });
        }

        public Task ShowComingOutCarrierOnMonitorAsync(string carrierId)
        {
            return Task.Run(() =>
            {
                var function = scApp.getFunBaseObj<ManualPortPLCControl>(port.PORT_ID) as ManualPortPLCControl;

                carrierId = carrierId.Trim();

                if (carrierId.Length > 14)
                    carrierId = carrierId.Substring(0, 14);

                function.ComingOutCarrierId = carrierId;

                CommitChange(function);
            });
        }

        public Task TimeCalibrationAsync()
        {
            return Task.Run(() =>
            {
                var function = scApp.getFunBaseObj<ManualPortPLCControl>(port.PORT_ID) as ManualPortPLCControl;

                function.TimeCalibrationBcdYearMonth = (UInt16)((DateTime.Now.Year.ToBCD() - 2000) * 256 + DateTime.Now.Month.ToBCD());
                function.TimeCalibrationBcdDayHour = (UInt16)(DateTime.Now.Day.ToBCD() * 256 + DateTime.Now.Hour.ToBCD());
                function.TimeCalibrationBcdMinuteSecond = (UInt16)(DateTime.Now.Minute.ToBCD() * 256 + DateTime.Now.Second.ToBCD());

                if (function.TimeCalibrationIndex < 65535)
                    function.TimeCalibrationIndex = (UInt16)(function.OhbcErrorIndex + 1);
                else
                    function.TimeCalibrationIndex = 1;

                CommitChange(function);
            });
        }

        private void CommitChange(ManualPortPLCControl function)
        {
            try
            {
                function.Write(bcfApp, port.EqptObjectCate, port.PORT_ID);

                logger.Info(function.ToString());
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<ManualPortPLCControl>(function);
            }
        }

        #endregion Control

        public object GetPortState()
        {
            var port_data = scApp.getFunBaseObj<ManualPortPLCInfo>(port.PORT_ID) as ManualPortPLCInfo;
            port_data.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
            return port_data;
        }
    }
}
