//*********************************************************************************
//      MTLValueDefMapAction.cs
//*********************************************************************************
// File Name: MTLValueDefMapAction.cs
// Description: 
//
//(c) Copyright 2018, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//**********************************************************************************
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.bcf.Data.ValueDefMapAction;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using com.mirle.ibg3k0.sc.Data.VO;
using KingAOP;
using NLog;
using System;
using System.Dynamic;
using System.Linq.Expressions;

namespace com.mirle.ibg3k0.sc.Data.ValueDefMapAction
{
    public class HIDValueDefMapAction : IValueDefMapAction
    {
        public const string DEVICE_NAME_HID = "HID";
        Logger logger = NLog.LogManager.GetCurrentClassLogger();
        //AEQPT eqpt = null;
        HID eqpt = null;
        protected SCApplication scApp = null;
        protected BCFApplication bcfApp = null;

        public HIDValueDefMapAction() : base()
        {
            scApp = SCApplication.getInstance();
            bcfApp = scApp.getBCFApplication();
        }
        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new AspectWeaver(parameter, this);
        }

        public virtual string getIdentityKey()
        {
            return this.GetType().Name;
        }
        public virtual void setContext(BaseEQObject baseEQ)
        {
            //this.eqpt = baseEQ as AEQPT;
            eqpt = baseEQ as HID;
        }
        public virtual void unRegisterEvent()
        {
            //not implement
        }
        public virtual void doShareMemoryInit(BCFAppConstants.RUN_LEVEL runLevel)
        {
            try
            {
                switch (runLevel)
                {
                    case BCFAppConstants.RUN_LEVEL.ZERO:
                        initHID_ChargeInfo();
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
                logger.Error(ex, "Exection:");
            }
        }

        public string EQID => eqpt.EQPT_ID;

        public virtual void initHID_ChargeInfo()
        {
            var function = scApp.getFunBaseObj<HIDToOHxC_ChargeInfo>(eqpt.EQPT_ID) as HIDToOHxC_ChargeInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                //2.read log
                function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)
                eqpt.Eq_Alive_Last_Change_time = DateTime.Now;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<HIDToOHxC_ChargeInfo>(function);
            }
        }

        private void HID_ChargeInfo(object sender, ValueChangedEventArgs args)
        {
            var function = scApp.getFunBaseObj<HIDToOHxC_ChargeInfo>(eqpt.EQPT_ID) as HIDToOHxC_ChargeInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                //2.read log
                function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                eqpt.HID_ID = function.HID_ID;
                eqpt.V_Unit = function.V_Unit;
                eqpt.V_Dot = function.V_Dot;
                eqpt.A_Unit = function.A_Unit;
                eqpt.A_Dot = function.A_Dot;
                eqpt.W_Unit = function.W_Unit;
                eqpt.W_Dot = function.W_Dot;
                eqpt.Hour_Unit = function.Hour_Unit;
                eqpt.Hour_Dot = function.Hour_Dot;
                eqpt.Hour_Sigma_High_Word = function.Hour_Sigma_High_Word;
                eqpt.Hour_Sigma_Low_Word = function.Hour_Sigma_Low_Word;
                eqpt.Hour_Positive_High_Word = function.Hour_Positive_High_Word;
                eqpt.Hour_Positive_Low_Word = function.Hour_Positive_Low_Word;
                eqpt.Hour_Negative_High_Word = function.Hour_Negative_High_Word;
                eqpt.Hour_Negative_Low_Word = function.Hour_Negative_Low_Word;
                eqpt.VR_Source = function.VR_Source;
                eqpt.VS_Source = function.VS_Source;
                eqpt.VT_Source = function.VT_Source;
                eqpt.Sigma_V_Source = function.Sigma_V_Source;
                eqpt.AR_Source = function.AR_Source;
                eqpt.AS_Source = function.AS_Source;
                eqpt.AT_Source = function.AT_Source;
                eqpt.Sigma_A_Source = function.Sigma_A_Source;
                eqpt.WR_Source = function.WR_Source;
                eqpt.WS_Source = function.WS_Source;
                eqpt.WT_Source = function.WT_Source;
                eqpt.Sigma_W_Source = function.Sigma_W_Source;

                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<HIDToOHxC_ChargeInfo>(function);
            }
        }
        //const string HID_IGBT_A_ALARM_CODE = "1";
        //const string HID_IGBT_B_ALARM_CODE = "2";
        //const string HID_TEMPERATURE_ALARM_CODE = "3";
        //const string HID_POWER_ALARM_CODE = "4";
        //const string HID_EMI_ALARM_CODE = "5";
        //const string HID_SMOKE_ALARM_CODE = "6";
        //const string HID_SAFE_CIRCUIT_ALARM_CODE = "7";
        //const string HID_FAN_1_ALARM_CODE = "8";
        //const string HID_FAN_2_ALARM_CODE = "9";
        //const string HID_FAN_3_ALARM_CODE = "10";
        //const string HID_TIMEOUT_ALARM_CODE = "11";
        const string HID_TEMPERATURE_ALARM_CODE = "11";
        const string HID_POWER_ALARM_CODE = "12";

        const string HID_ALARM_1_CODE = "1";
        const string HID_ALARM_2_CODE = "2";
        const string HID_ALARM_3_CODE = "3";
        const string HID_ALARM_4_CODE = "4";
        const string HID_ALARM_5_CODE = "5";
        const string HID_ALARM_6_CODE = "6";
        const string HID_ALARM_7_CODE = "7";
        const string HID_ALARM_8_CODE = "8";
        const string HID_ALARM_9_CODE = "9";
        const string HID_ALARM_10_CODE = "10";

        private void HID_Heartbeat(object sender, ValueChangedEventArgs args)
        {
            var function = scApp.getFunBaseObj<HIDToOHxC_Alive>(eqpt.EQPT_ID) as HIDToOHxC_Alive;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                //2.read log
                function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                eqpt.Alive = function.Alive;
                eqpt.Eq_Alive_Last_Change_time = DateTime.Now;

                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<HIDToOHxC_Alive>(function);
            }
        }

        private void Alarm_1(object sender, ValueChangedEventArgs args)
        {
            var recevie_function = scApp.getFunBaseObj<HIDToOHxC_Alarm_1>(eqpt.EQPT_ID) as HIDToOHxC_Alarm_1;
            try
            {
                //1.建立各個Function物件
                recevie_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                recevie_function.EQ_ID = eqpt.EQPT_ID;
                //2.read log
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(HIDValueDefMapAction), Device: DEVICE_NAME_HID,
                         Data: recevie_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);
                NLog.LogManager.GetLogger("HIDAlarm").Info(recevie_function.ToString());
                ProtocolFormat.OHTMessage.ErrorStatus status = recevie_function.Alarm_1_Happend ?
                                                                    ProtocolFormat.OHTMessage.ErrorStatus.ErrSet :
                                                                    ProtocolFormat.OHTMessage.ErrorStatus.ErrReset;
                //scApp.LineService.ProcessAlarmReport(eqpt.NODE_ID, eqpt.EQPT_ID, HID_ALARM_1_CODE, status, "");
                eqpt.ReportHIDAlarm(HID_ALARM_1_CODE, status);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<HIDToOHxC_Alarm_1>(recevie_function);
            }
        }


        private void Alarm_2(object sender, ValueChangedEventArgs args)
        {
            var recevie_function = scApp.getFunBaseObj<HIDToOHxC_Alarm_2>(eqpt.EQPT_ID) as HIDToOHxC_Alarm_2;
            try
            {
                //1.建立各個Function物件
                recevie_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                recevie_function.EQ_ID = eqpt.EQPT_ID;
                //2.read log
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(HIDValueDefMapAction), Device: DEVICE_NAME_HID,
                         Data: recevie_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);
                NLog.LogManager.GetLogger("HIDAlarm").Info(recevie_function.ToString());
                ProtocolFormat.OHTMessage.ErrorStatus status = recevie_function.Alarm_2_Happend ?
                                                                    ProtocolFormat.OHTMessage.ErrorStatus.ErrSet :
                                                                    ProtocolFormat.OHTMessage.ErrorStatus.ErrReset;
                //scApp.LineService.ProcessAlarmReport(eqpt.NODE_ID, eqpt.EQPT_ID, HID_ALARM_2_CODE, status, "");
                eqpt.ReportHIDAlarm(HID_ALARM_2_CODE, status);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<HIDToOHxC_Alarm_2>(recevie_function);
            }
        }


        private void Alarm_3(object sender, ValueChangedEventArgs args)
        {
            var recevie_function = scApp.getFunBaseObj<HIDToOHxC_Alarm_3>(eqpt.EQPT_ID) as HIDToOHxC_Alarm_3;
            try
            {
                //1.建立各個Function物件
                recevie_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                recevie_function.EQ_ID = eqpt.EQPT_ID;
                //2.read log
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(HIDValueDefMapAction), Device: DEVICE_NAME_HID,
                         Data: recevie_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);
                NLog.LogManager.GetLogger("HIDAlarm").Info(recevie_function.ToString());
                ProtocolFormat.OHTMessage.ErrorStatus status = recevie_function.Alarm_3_Happend ?
                                                                    ProtocolFormat.OHTMessage.ErrorStatus.ErrSet :
                                                                    ProtocolFormat.OHTMessage.ErrorStatus.ErrReset;
                //scApp.LineService.ProcessAlarmReport(eqpt.NODE_ID, eqpt.EQPT_ID, HID_ALARM_3_CODE, status, "");
                eqpt.ReportHIDAlarm(HID_ALARM_3_CODE, status);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<HIDToOHxC_Alarm_3>(recevie_function);
            }
        }

        private void Alarm_4(object sender, ValueChangedEventArgs args)
        {
            var recevie_function = scApp.getFunBaseObj<HIDToOHxC_Alarm_4>(eqpt.EQPT_ID) as HIDToOHxC_Alarm_4;
            try
            {
                //1.建立各個Function物件
                recevie_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                recevie_function.EQ_ID = eqpt.EQPT_ID;
                //2.read log
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(HIDValueDefMapAction), Device: DEVICE_NAME_HID,
                         Data: recevie_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);
                NLog.LogManager.GetLogger("HIDAlarm").Info(recevie_function.ToString());
                ProtocolFormat.OHTMessage.ErrorStatus status = recevie_function.Alarm_4_Happend ?
                                                                    ProtocolFormat.OHTMessage.ErrorStatus.ErrSet :
                                                                    ProtocolFormat.OHTMessage.ErrorStatus.ErrReset;
                //scApp.LineService.ProcessAlarmReport(eqpt.NODE_ID, eqpt.EQPT_ID, HID_ALARM_4_CODE, status, "");
                eqpt.ReportHIDAlarm(HID_ALARM_4_CODE, status);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<HIDToOHxC_Alarm_4>(recevie_function);
            }
        }

        private void Alarm_5(object sender, ValueChangedEventArgs args)
        {
            var recevie_function = scApp.getFunBaseObj<HIDToOHxC_Alarm_5>(eqpt.EQPT_ID) as HIDToOHxC_Alarm_5;
            try
            {
                //1.建立各個Function物件
                recevie_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                recevie_function.EQ_ID = eqpt.EQPT_ID;
                //2.read log
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(HIDValueDefMapAction), Device: DEVICE_NAME_HID,
                         Data: recevie_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);
                NLog.LogManager.GetLogger("HIDAlarm").Info(recevie_function.ToString());
                ProtocolFormat.OHTMessage.ErrorStatus status = recevie_function.Alarm_5_Happend ?
                                                                    ProtocolFormat.OHTMessage.ErrorStatus.ErrSet :
                                                                    ProtocolFormat.OHTMessage.ErrorStatus.ErrReset;
                //scApp.LineService.ProcessAlarmReport(eqpt.NODE_ID, eqpt.EQPT_ID, HID_ALARM_5_CODE, status, "");
                eqpt.ReportHIDAlarm(HID_ALARM_5_CODE, status);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<HIDToOHxC_Alarm_5>(recevie_function);
            }
        }
        private void Alarm_6(object sender, ValueChangedEventArgs args)
        {
            var recevie_function = scApp.getFunBaseObj<HIDToOHxC_Alarm_6>(eqpt.EQPT_ID) as HIDToOHxC_Alarm_6;
            try
            {
                //1.建立各個Function物件
                recevie_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                recevie_function.EQ_ID = eqpt.EQPT_ID;
                //2.read log
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(HIDValueDefMapAction), Device: DEVICE_NAME_HID,
                         Data: recevie_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);
                NLog.LogManager.GetLogger("HIDAlarm").Info(recevie_function.ToString());
                ProtocolFormat.OHTMessage.ErrorStatus status = recevie_function.Alarm_6_Happend ?
                                                                    ProtocolFormat.OHTMessage.ErrorStatus.ErrSet :
                                                                    ProtocolFormat.OHTMessage.ErrorStatus.ErrReset;
                //scApp.LineService.ProcessAlarmReport(eqpt.NODE_ID, eqpt.EQPT_ID, HID_ALARM_6_CODE, status, "");
                eqpt.ReportHIDAlarm(HID_ALARM_6_CODE, status);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<HIDToOHxC_Alarm_6>(recevie_function);
            }
        }
        private void Alarm_7(object sender, ValueChangedEventArgs args)
        {
            var recevie_function = scApp.getFunBaseObj<HIDToOHxC_Alarm_7>(eqpt.EQPT_ID) as HIDToOHxC_Alarm_7;
            try
            {
                //1.建立各個Function物件
                recevie_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                recevie_function.EQ_ID = eqpt.EQPT_ID;
                //2.read log
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(HIDValueDefMapAction), Device: DEVICE_NAME_HID,
                         Data: recevie_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);
                NLog.LogManager.GetLogger("HIDAlarm").Info(recevie_function.ToString());
                ProtocolFormat.OHTMessage.ErrorStatus status = recevie_function.Alarm_7_Happend ?
                                                                    ProtocolFormat.OHTMessage.ErrorStatus.ErrSet :
                                                                    ProtocolFormat.OHTMessage.ErrorStatus.ErrReset;
                //scApp.LineService.ProcessAlarmReport(eqpt.NODE_ID, eqpt.EQPT_ID, HID_ALARM_7_CODE, status, "");
                eqpt.ReportHIDAlarm(HID_ALARM_7_CODE, status);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<HIDToOHxC_Alarm_7>(recevie_function);
            }
        }

        private void Alarm_8(object sender, ValueChangedEventArgs args)
        {
            var recevie_function = scApp.getFunBaseObj<HIDToOHxC_Alarm_8>(eqpt.EQPT_ID) as HIDToOHxC_Alarm_8;
            try
            {
                //1.建立各個Function物件
                recevie_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                recevie_function.EQ_ID = eqpt.EQPT_ID;
                //2.read log
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(HIDValueDefMapAction), Device: DEVICE_NAME_HID,
                         Data: recevie_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);
                NLog.LogManager.GetLogger("HIDAlarm").Info(recevie_function.ToString());
                ProtocolFormat.OHTMessage.ErrorStatus status = recevie_function.Alarm_8_Happend ?
                                                                    ProtocolFormat.OHTMessage.ErrorStatus.ErrSet :
                                                                    ProtocolFormat.OHTMessage.ErrorStatus.ErrReset;
                //scApp.LineService.ProcessAlarmReport(eqpt.NODE_ID, eqpt.EQPT_ID, HID_ALARM_8_CODE, status, "");
                eqpt.ReportHIDAlarm(HID_ALARM_8_CODE, status);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<HIDToOHxC_Alarm_8>(recevie_function);
            }
        }
        private void Alarm_9(object sender, ValueChangedEventArgs args)
        {
            var recevie_function = scApp.getFunBaseObj<HIDToOHxC_Alarm_9>(eqpt.EQPT_ID) as HIDToOHxC_Alarm_9;
            try
            {
                //1.建立各個Function物件
                recevie_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                recevie_function.EQ_ID = eqpt.EQPT_ID;
                //2.read log
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(HIDValueDefMapAction), Device: DEVICE_NAME_HID,
                         Data: recevie_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);
                NLog.LogManager.GetLogger("HIDAlarm").Info(recevie_function.ToString());
                ProtocolFormat.OHTMessage.ErrorStatus status = recevie_function.Alarm_9_Happend ?
                                                                    ProtocolFormat.OHTMessage.ErrorStatus.ErrSet :
                                                                    ProtocolFormat.OHTMessage.ErrorStatus.ErrReset;
                //scApp.LineService.ProcessAlarmReport(eqpt.NODE_ID, eqpt.EQPT_ID, HID_ALARM_9_CODE, status, "");
                eqpt.ReportHIDAlarm(HID_ALARM_9_CODE, status);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<HIDToOHxC_Alarm_9>(recevie_function);
            }
        }

        private void Alarm_10(object sender, ValueChangedEventArgs args)
        {
            var recevie_function = scApp.getFunBaseObj<HIDToOHxC_Alarm_10>(eqpt.EQPT_ID) as HIDToOHxC_Alarm_10;
            try
            {
                //1.建立各個Function物件
                recevie_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                recevie_function.EQ_ID = eqpt.EQPT_ID;
                //2.read log
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(HIDValueDefMapAction), Device: DEVICE_NAME_HID,
                         Data: recevie_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);
                NLog.LogManager.GetLogger("HIDAlarm").Info(recevie_function.ToString());
                ProtocolFormat.OHTMessage.ErrorStatus status = recevie_function.Alarm_10_Happend ?
                                                                    ProtocolFormat.OHTMessage.ErrorStatus.ErrSet :
                                                                    ProtocolFormat.OHTMessage.ErrorStatus.ErrReset;
                //scApp.LineService.ProcessAlarmReport(eqpt.NODE_ID, eqpt.EQPT_ID, HID_ALARM_10_CODE, status, "");
                eqpt.ReportHIDAlarm(HID_ALARM_10_CODE, status);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<HIDToOHxC_Alarm_10>(recevie_function);
            }
        }

        public virtual void HID_Control(bool control)
        {
            var function = scApp.getFunBaseObj<OHxCToHID_Control>(eqpt.EQPT_ID) as OHxCToHID_Control;
            try
            {
                //1.建立各個Function物件

                function.Control = control;
                function.Write(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                function.Timestamp = DateTime.Now;

                //2.write log
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());

                //3.logical (include db save)
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<OHxCToHID_Control>(function);
            }
        }


        object dateTimeSyneObj = new object();
        uint dateTimeIndex = 0;
        public virtual void DateTimeSyncCommand(DateTime dateTime)
        {

            OHxCToHID_DateTimeSync send_function =
               scApp.getFunBaseObj<OHxCToHID_DateTimeSync>(eqpt.EQPT_ID) as OHxCToHID_DateTimeSync;
            try
            {
                lock (dateTimeSyneObj)
                {
                    //1.準備要發送的資料
                    send_function.Year = Convert.ToUInt16(dateTime.Year.ToString(), 10);
                    send_function.Month = Convert.ToUInt16(dateTime.Month.ToString(), 10);
                    send_function.Day = Convert.ToUInt16(dateTime.Day.ToString(), 10);
                    send_function.Hour = Convert.ToUInt16(dateTime.Hour.ToString(), 10);
                    send_function.Min = Convert.ToUInt16(dateTime.Minute.ToString(), 10);
                    send_function.Sec = Convert.ToUInt16(dateTime.Second.ToString(), 10);
                    if (dateTimeIndex >= 9999)
                        dateTimeIndex = 0;
                    send_function.Index = ++dateTimeIndex;
                    //2.紀錄發送資料的Log
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(HIDValueDefMapAction), Device: SCAppConstants.DeviceName.DEVICE_NAME_HID,
                             Data: send_function.ToString(),
                             XID: eqpt.EQPT_ID);
                    //3.發送訊息
                    send_function.Write(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<OHxCToHID_DateTimeSync>(send_function);
            }
        }

        object SilentSyneObj = new object();
        uint silentIndex = 0;
        public virtual void SilentCommand()
        {
            OHxCToHID_SilentIndex send_function =
               scApp.getFunBaseObj<OHxCToHID_SilentIndex>(eqpt.EQPT_ID) as OHxCToHID_SilentIndex;
            try
            {
                lock (SilentSyneObj)
                {
                    if (silentIndex >= 9999)
                        silentIndex = 0;
                    send_function.Index = ++silentIndex;
                    //2.紀錄發送資料的Log
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(HIDValueDefMapAction), Device: SCAppConstants.DeviceName.DEVICE_NAME_HID,
                             Data: send_function.ToString(),
                             XID: eqpt.EQPT_ID);
                    //3.發送訊息
                    send_function.Write(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<OHxCToHID_SilentIndex>(send_function);
            }
        }

        object HeartbeatSyneObj = new object();
        uint heartbeatIndex = 0;
        public virtual void SendHeartbeat()
        {
            OHxCToHID_AliveIndex send_function =
               scApp.getFunBaseObj<OHxCToHID_AliveIndex>(eqpt.EQPT_ID) as OHxCToHID_AliveIndex;
            try
            {
                lock (HeartbeatSyneObj)
                {
                    if (heartbeatIndex >= 9999)
                        heartbeatIndex = 0;
                    send_function.Index = ++heartbeatIndex;
                    //2.紀錄發送資料的Log
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(HIDValueDefMapAction), Device: SCAppConstants.DeviceName.DEVICE_NAME_HID,
                             Data: send_function.ToString(),
                             XID: eqpt.EQPT_ID);
                    //3.發送訊息
                    send_function.Write(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<OHxCToHID_AliveIndex>(send_function);
            }
        }

        private void TempAlarm(object sender, ValueChangedEventArgs args)
        {
            var recevie_function = scApp.getFunBaseObj<HIDToOHxC_TempAlarm>(eqpt.EQPT_ID) as HIDToOHxC_TempAlarm;
            try
            {
                //1.建立各個Function物件
                recevie_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                recevie_function.EQ_ID = eqpt.EQPT_ID;
                //2.read log
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(HIDValueDefMapAction), Device: DEVICE_NAME_HID,
                         Data: recevie_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);
                NLog.LogManager.GetLogger("HIDAlarm").Info(recevie_function.ToString());
                ProtocolFormat.OHTMessage.ErrorStatus status = recevie_function.TempAlarmHappend ?
                                                                    ProtocolFormat.OHTMessage.ErrorStatus.ErrSet :
                                                                    ProtocolFormat.OHTMessage.ErrorStatus.ErrReset;
                //scApp.LineService.ProcessAlarmReport(eqpt.NODE_ID, eqpt.EQPT_ID, HID_TEMPERATURE_ALARM_CODE, status, "");
                eqpt.ReportHIDTemperatureAlarm(HID_TEMPERATURE_ALARM_CODE, status);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<HIDToOHxC_TempAlarm>(recevie_function);
            }
        }

        private void PowerAlarm(object sender, ValueChangedEventArgs args)
        {
            var recevie_function = scApp.getFunBaseObj<HIDToOHxC_PowerAlarm>(eqpt.EQPT_ID) as HIDToOHxC_PowerAlarm;
            try
            {
                //1.建立各個Function物件
                recevie_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                recevie_function.EQ_ID = eqpt.EQPT_ID;
                //2.read log
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(HIDValueDefMapAction), Device: DEVICE_NAME_HID,
                         Data: recevie_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);
                NLog.LogManager.GetLogger("HIDAlarm").Info(recevie_function.ToString());
                ProtocolFormat.OHTMessage.ErrorStatus status = recevie_function.PowerAlarmHappend ?
                                                                    ProtocolFormat.OHTMessage.ErrorStatus.ErrSet :
                                                                    ProtocolFormat.OHTMessage.ErrorStatus.ErrReset;
                //scApp.LineService.ProcessAlarmReport(eqpt.NODE_ID, eqpt.EQPT_ID, HID_POWER_ALARM_CODE, status, "");
                eqpt.ReportHIDPowerAlarm(HID_POWER_ALARM_CODE, status);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<HIDToOHxC_PowerAlarm>(recevie_function);
            }
        }

        string event_id = string.Empty;
        /// <summary>
        /// Does the initialize.
        /// </summary>
        public virtual void doInit()
        {
            try
            {
                ValueRead vr = null;
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "HID_TO_OHXC_ALIVE", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => HID_Heartbeat(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "HID_TO_OHXC_TRIGGER", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => HID_ChargeInfo(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "HID_TO_OHXC_ALARM_1", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Alarm_1(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "HID_TO_OHXC_ALARM_2", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Alarm_2(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "HID_TO_OHXC_ALARM_3", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Alarm_3(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "HID_TO_OHXC_ALARM_4", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Alarm_4(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "HID_TO_OHXC_ALARM_5", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Alarm_5(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "HID_TO_OHXC_ALARM_6", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Alarm_6(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "HID_TO_OHXC_ALARM_7", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Alarm_7(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "HID_TO_OHXC_ALARM_8", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Alarm_8(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "HID_TO_OHXC_ALARM_9", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Alarm_9(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "HID_TO_OHXC_ALARM_10", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Alarm_10(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "HID_TO_OHXC_TEMP_ALARM", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => TempAlarm(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "HID_TO_OHXC_POWER_ALARM", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => PowerAlarm(_sender, _e);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
            }
        }
    }
}
