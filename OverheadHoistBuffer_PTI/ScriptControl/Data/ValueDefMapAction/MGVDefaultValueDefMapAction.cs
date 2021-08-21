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

        public string PortName { get => eqpt.EQPT_ID; }
        #endregion Implement

        protected AEQPT eqpt = null;
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
            this.eqpt = baseEQ as AEQPT;
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
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MGV_TO_OHxC_RUN", out ValueRead vr1))
                {
                    vr1.afterValueChange += (_sender, e) => MGV_Status_Change_RUN(_sender, e);
                }

                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MGV_TO_OHxC_DOWN", out ValueRead vr2))
                {
                    vr2.afterValueChange += (_sender, e) => MGV_Status_Change_DOWN(_sender, e);
                }

                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MGV_TO_OHxC_FAULT", out ValueRead vr3))
                {
                    vr3.afterValueChange += (_sender, e) => MGV_Status_Change_FAULT(_sender, e);
                }

                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MGV_TO_OHxC_OUTMODE", out ValueRead vr4))
                {
                    vr4.afterValueChange += (_sender, e) => MGV_Status_Change_to_OutMode(_sender, e);
                }

                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MGV_TO_OHxC_INMODE", out ValueRead vr5))
                {
                    vr5.afterValueChange += (_sender, e) => MGV_Status_Change_to_InMode(_sender, e);
                }

                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MGV_TO_OHxC_WAITIN", out ValueRead vr6))
                {
                    vr6.afterValueChange += (_sender, e) => MGV_Status_WaitIn(_sender, e);
                }

                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MGV_TO_OHxC_WAITOUT", out ValueRead vr7))
                {
                    vr7.afterValueChange += (_sender, e) => MGV_Status_WaitOut(_sender, e);
                }

                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MGV_TO_OHxC_LOADPRESENCE1", out ValueRead vr8))
                {
                    vr8.afterValueChange += (_sender, e) => MGV_Status_Stage1_PresenceChanged(_sender, e);
                }

                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MGV_TO_OHxC_BCRREADDONE", out ValueRead vr9))
                {
                    vr9.afterValueChange += (_sender, e) => MGV_Status_BcrReadDone(_sender, e);
                }

                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MGV_TO_OHxC_REMOVECHECK", out ValueRead vr10))
                {
                    vr10.afterValueChange += (_sender, e) => MGV_Status_RemoveCheck(_sender, e);
                }

                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MGV_TO_OHxC_ERRORINDEX", out ValueRead vr11))
                {
                    vr11.afterValueChange += (_sender, e) => MGV_Status_ErrorIndexChanged(_sender, e);
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
            var function = scApp.getFunBaseObj<ManualPortPLCInfo>(eqpt.EQPT_ID) as ManualPortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                //2.read log
                LogManager.GetCurrentClassLogger().Info(function.ToString());

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
            var function = scApp.getFunBaseObj<ManualPortPLCInfo>(eqpt.EQPT_ID) as ManualPortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                //2.read log
                LogManager.GetCurrentClassLogger().Info(function.ToString());

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
            var function = scApp.getFunBaseObj<ManualPortPLCInfo>(eqpt.EQPT_ID) as ManualPortPLCInfo;

            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);

                //2.read log
                LogManager.GetCurrentClassLogger().Info(function.ToString());

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
            var function = scApp.getFunBaseObj<ManualPortPLCInfo>(eqpt.EQPT_ID) as ManualPortPLCInfo;

            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);

                //2.read log
                LogManager.GetCurrentClassLogger().Info(function.ToString());

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
            var function = scApp.getFunBaseObj<ManualPortPLCInfo>(eqpt.EQPT_ID) as ManualPortPLCInfo;

            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);

                //2.read log
                LogManager.GetCurrentClassLogger().Info(function.ToString());

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
            var function = scApp.getFunBaseObj<ManualPortPLCInfo>(eqpt.EQPT_ID) as ManualPortPLCInfo;

            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);

                //2.read log
                LogManager.GetCurrentClassLogger().Info(function.ToString());

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
            var function = scApp.getFunBaseObj<ManualPortPLCInfo>(eqpt.EQPT_ID) as ManualPortPLCInfo;

            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);

                //2.read log
                LogManager.GetCurrentClassLogger().Info(function.ToString());

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
            var function = scApp.getFunBaseObj<ManualPortPLCInfo>(eqpt.EQPT_ID) as ManualPortPLCInfo;

            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);

                //2.read log
                LogManager.GetCurrentClassLogger().Info(function.ToString());

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
            var function = scApp.getFunBaseObj<ManualPortPLCInfo>(eqpt.EQPT_ID) as ManualPortPLCInfo;

            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);

                //2.read log
                LogManager.GetCurrentClassLogger().Info(function.ToString());

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
            var function = scApp.getFunBaseObj<ManualPortPLCInfo>(eqpt.EQPT_ID) as ManualPortPLCInfo;

            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);

                //2.read log
                LogManager.GetCurrentClassLogger().Info(function.ToString());

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
            var function = scApp.getFunBaseObj<ManualPortPLCInfo>(eqpt.EQPT_ID) as ManualPortPLCInfo;

            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);

                //2.read log
                LogManager.GetCurrentClassLogger().Info(function.ToString());

                if (function.IsRun && Int32.TryParse(function.AlarmCode, out var alarmCode))
                {
                    if (alarmCode == 0)
                        OnAlarmClear?.Invoke(this, new ManualPortEventArgs(function));
                    else
                        OnAlarmHappen?.Invoke(this, new ManualPortEventArgs(function));
                }
                else
                    OnAlarmHappen?.Invoke(this, new ManualPortEventArgs(function));
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
                var function = scApp.getFunBaseObj<ManualPortPLCControl>(eqpt.EQPT_ID) as ManualPortPLCControl;
                function.MoveBackReason = (ushort)reason;
                CommitChange(function);
            });
        }

        public Task ChangeToInModeAsync()
        {
            return Task.Run(() =>
            {
                var function = scApp.getFunBaseObj<ManualPortPLCControl>(eqpt.EQPT_ID) as ManualPortPLCControl;
                function.IsChangeToInMode = true;
                CommitChange(function);
            });
        }

        public Task ChangeToOutModeAsync()
        {
            return Task.Run(() =>
            {
                var function = scApp.getFunBaseObj<ManualPortPLCControl>(eqpt.EQPT_ID) as ManualPortPLCControl;
                function.IsChangeToOutMode = true;
                CommitChange(function);
            });
        }

        public Task MoveBackAsync()
        {
            return Task.Run(() =>
            {
                var function = scApp.getFunBaseObj<ManualPortPLCControl>(eqpt.EQPT_ID) as ManualPortPLCControl;
                function.IsMoveBack = true;
                CommitChange(function);
            });
        }

        public Task ResetAlarmAsync()
        {
            return Task.Run(() =>
            {
                var function = scApp.getFunBaseObj<ManualPortPLCControl>(eqpt.EQPT_ID) as ManualPortPLCControl;
                function.IsResetOn = true;
                CommitChange(function);
            });
        }

        public Task StopBuzzerAsync()
        {
            return Task.Run(() =>
            {
                var function = scApp.getFunBaseObj<ManualPortPLCControl>(eqpt.EQPT_ID) as ManualPortPLCControl;
                function.IsBuzzerStop = true;
                CommitChange(function);
            });
        }

        public Task SetRunAsync()
        {
            return Task.Run(() =>
            {
                var function = scApp.getFunBaseObj<ManualPortPLCControl>(eqpt.EQPT_ID) as ManualPortPLCControl;
                function.IsSetRun = true;
                CommitChange(function);
            });
        }

        public Task SetStopAsync()
        {
            return Task.Run(() =>
            {
                var function = scApp.getFunBaseObj<ManualPortPLCControl>(eqpt.EQPT_ID) as ManualPortPLCControl;
                function.IsSetStop = true;
                CommitChange(function);
            });
        }

        public Task SetCommandingAsync(bool setOn)
        {
            return Task.Run(() =>
            {
                var function = scApp.getFunBaseObj<ManualPortPLCControl>(eqpt.EQPT_ID) as ManualPortPLCControl;
                function.IsCommanding = setOn;
                CommitChange(function);
            });
        }

        public Task SetControllerErrorIndexAsync(int newIndex)
        {
            return Task.Run(() =>
            {
                throw new NotImplementedException();
            });
        }

        private void CommitChange(ManualPortPLCControl function)
        {
            try
            {
                function.Write(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);

                LogManager.GetCurrentClassLogger().Info(function.ToString());
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

        public ManualPortPLCInfo GetPortState()
        {
            return scApp.getFunBaseObj<ManualPortPLCInfo>(eqpt.EQPT_ID) as ManualPortPLCInfo;
        }
    }
}
