//*********************************************************************************
//      MTSValueDefMapAction.cs
//*********************************************************************************
// File Name: MTSValueDefMapAction.cs
// Description: 
//
//(c) Copyright 2018, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//**********************************************************************************
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Common.MPLC;
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
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

namespace com.mirle.ibg3k0.sc.Data.ValueDefMapAction
{
    public class MTSValueDefMapActionNewPH2 : MTxValueDefMapActionBase
    {
        Logger logger = NLog.LogManager.GetCurrentClassLogger();
        MaintainSpace MTS { get { return this.eqpt as MaintainSpace; } }
        public MTSValueDefMapActionNewPH2()
            : base()
        {
        }
        object dateTimeSyneObj = new object();
        uint dateTimeIndex = 0;
        public override void DateTimeSyncCommand(DateTime dateTime)
        {

            OHxCToMtl_DateTimeSyncPH2 send_function =
               scApp.getFunBaseObj<OHxCToMtl_DateTimeSyncPH2>(MTS.EQPT_ID) as OHxCToMtl_DateTimeSyncPH2;
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
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                             Data: send_function.ToString(),
                             XID: MTS.EQPT_ID);
                    //3.發送訊息
                    send_function.Write(bcfApp, MTS.EqptObjectCate, MTS.EQPT_ID);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<OHxCToMtl_DateTimeSyncPH2>(send_function);
            }
        }
        uint message_index = 0;
        public override void OHxCMessageDownload(string msg)
        {
            OHxCToMtl_MessageDownload_PH2 send_function =
                scApp.getFunBaseObj<OHxCToMtl_MessageDownload_PH2>(MTS.EQPT_ID) as OHxCToMtl_MessageDownload_PH2;
            try
            {
                //1.建立各個Function物件
                send_function.Message = msg;
                if (message_index > 9999)
                { message_index = 0; }
                send_function.Index = ++message_index;
                //2.紀錄發送資料的Log
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                         Data: send_function.ToString(),
                         XID: MTS.EQPT_ID);
                //3.發送訊息
                send_function.Write(bcfApp, MTS.EqptObjectCate, MTS.EQPT_ID);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<OHxCToMtl_MessageDownload_PH2>(send_function);
            }
        }
        UInt16 car_realtime_info = 0;
        public override void CarRealtimeInfo(UInt16 car_id, UInt16 action_mode, UInt16 cst_exist, UInt16 current_section_id, UInt32 current_address_id,
                                            UInt32 buffer_distance, UInt16 speed)
        {
            OHxCToMtl_CarRealtimeInfo_PH2 send_function =
                scApp.getFunBaseObj<OHxCToMtl_CarRealtimeInfo_PH2>(MTS.EQPT_ID) as OHxCToMtl_CarRealtimeInfo_PH2;
            try
            {
                //1.建立各個Function物件
                send_function.CarID = car_id;
                send_function.ActionMode = action_mode;
                send_function.CSTExist = cst_exist;
                send_function.CurrentSectionID = current_section_id;
                send_function.CurrentAddressID = current_address_id;
                send_function.BufferDistance = buffer_distance;
                send_function.Speed = speed;
                if (car_realtime_info > 9999)
                { car_realtime_info = 0; }
                send_function.Index = ++car_realtime_info;
                //2.紀錄發送資料的Log
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                         Data: send_function.ToString(),
                         XID: MTS.EQPT_ID);
                //3.發送訊息
                send_function.Write(bcfApp, MTS.EqptObjectCate, MTS.EQPT_ID);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<OHxCToMtl_CarRealtimeInfo_PH2>(send_function);
            }
        }

        public override (bool isSendSuccess, UInt16 returnCode) OHxC_CarOutNotify(UInt16 car_id, UInt16 action_type)
        {
            bool isSendSuccess = false;
            var send_function =
                scApp.getFunBaseObj<OHxCToMtl_CarOutNotify_PH2>(MTS.EQPT_ID) as OHxCToMtl_CarOutNotify_PH2;
            var receive_function =
                scApp.getFunBaseObj<MtlToOHxC_ReplyCarOutNotify_PH2>(MTS.EQPT_ID) as MtlToOHxC_ReplyCarOutNotify_PH2;
            try
            {
                //1.準備要發送的資料
                send_function.CarID = car_id;
                send_function.ActionType = action_type;
                ValueRead vr_reply = receive_function.getValueReadHandshake
                    (bcfApp, MTS.EqptObjectCate, MTS.EQPT_ID);
                //2.紀錄發送資料的Log
                send_function.Handshake = 1;
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                         Data: send_function.ToString(),
                         XID: MTS.EQPT_ID);
                //3.等待回復
                TrxMPLC.ReturnCode returnCode =
                    send_function.SendRecv(bcfApp, MTS.EqptObjectCate, MTS.EQPT_ID, vr_reply);
                //4.取得回復的結果
                if (returnCode == TrxMPLC.ReturnCode.Normal)
                {
                    receive_function.Read(bcfApp, MTS.EqptObjectCate, MTS.EQPT_ID);
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                             Data: receive_function.ToString(),
                             XID: MTS.EQPT_ID);
                    isSendSuccess = true;
                }
                send_function.Handshake = 0;
                send_function.resetHandshake(bcfApp, MTS.EqptObjectCate, MTS.EQPT_ID);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                         Data: send_function.ToString(),
                         XID: MTS.EQPT_ID);
                return (isSendSuccess, receive_function.ReturnCode);

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<OHxCToMtl_CarOutNotify_PH2>(send_function);
                scApp.putFunBaseObj<MtlToOHxC_ReplyCarOutNotify_PH2>(receive_function);
            }
            return (isSendSuccess, 0);
        }
        public override void OHxC2MTL_CarOutInterface(bool carOutInterlock, bool carOutReady, bool carMoving, bool carMoveComplete)
        {
            try
            {
                ValueWrite vm_carOutInterlock = bcfApp.getWriteValueEvent(MTS.EqptObjectCate, MTS.EQPT_ID, "OHXC_TO_MTL_U2D_CAR_OUT_INTERLOCK_PH2");
                //ValueWrite vm_carOutReady = bcfApp.getWriteValueEvent(MTS.EqptObjectCate, MTS.EQPT_ID, "OHXC_TO_MTL_U2D_CAR_OUT_READY");
                //ValueWrite vm_carMoving = bcfApp.getWriteValueEvent(MTS.EqptObjectCate, MTS.EQPT_ID, "OHXC_TO_MTL_U2D_CAR_MOVING");
                //ValueWrite vm_carMoveCmp = bcfApp.getWriteValueEvent(MTS.EqptObjectCate, MTS.EQPT_ID, "OHXC_TO_MTL_U2D_CAR_MOVE_COMPLETE");
                string setValue = carOutInterlock ? "1" : "0";
                vm_carOutInterlock.setWriteValue(carOutInterlock ? "1" : "0");
                //vm_carOutReady.setWriteValue(carOutReady ? "1" : "0");
                //vm_carMoving.setWriteValue(carMoving ? "1" : "0");
                //vm_carMoveCmp.setWriteValue(carMoveComplete ? "1" : "0");
                bool result = ISMControl.writeDeviceBlock(bcfApp, vm_carOutInterlock);
                //ISMControl.writeDeviceBlock(bcfApp, vm_carOutReady);
                //ISMControl.writeDeviceBlock(bcfApp, vm_carMoving);
                //ISMControl.writeDeviceBlock(bcfApp, vm_carMoveCmp);
                if (result) eqpt.Interlock = setValue == "1" ? true : false;
                MTS.CarOutInterlock = carOutInterlock;
                //MTS.CarInMoving = carMoving;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }
        public override bool setOHxC2MTL_CarOutInterlock(bool carOutInterlock)
        {
            try
            {
                ValueWrite vm_carOutInterlock = bcfApp.getWriteValueEvent(MTS.EqptObjectCate, MTS.EQPT_ID, "OHXC_TO_MTL_U2D_CAR_OUT_INTERLOCK_PH2");
                string setValue = carOutInterlock ? "1" : "0";
                vm_carOutInterlock.setWriteValue(setValue);
                bool result = ISMControl.writeDeviceBlock(bcfApp, vm_carOutInterlock);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                         Data: $"Set Car Out Intertlock:{carOutInterlock},result:{result}",
                         XID: MTS.EQPT_ID);
                if (result) eqpt.Interlock = setValue == "1" ? true : false;
                MTS.CarOutInterlock = carOutInterlock;

                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }
        }

        public override void OHxC2MTL_CarInInterface(bool carMoving, bool carMoveComplete)
        {
            try
            {
                ValueWrite vm_carMoving = bcfApp.getWriteValueEvent(MTS.EqptObjectCate, MTS.EQPT_ID, "OHXC_TO_MTL_D2U_CAR_MOVING_PH2");
                ValueWrite vm_carMoveCmp = bcfApp.getWriteValueEvent(MTS.EqptObjectCate, MTS.EQPT_ID, "OHXC_TO_MTL_D2U_CAR_MOVE_COMPLETE_PH2");
                vm_carMoving.setWriteValue(carMoving ? "1" : "0");
                vm_carMoveCmp.setWriteValue(carMoveComplete ? "1" : "0");
                ISMControl.writeDeviceBlock(bcfApp, vm_carMoving);
                ISMControl.writeDeviceBlock(bcfApp, vm_carMoveCmp);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }
        public override bool setOHxC2MTL_CarInMoving(bool carMoving)
        {
            try
            {
                ValueWrite vm_carMoving = bcfApp.getWriteValueEvent(MTS.EqptObjectCate, MTS.EQPT_ID, "OHXC_TO_MTL_D2U_CAR_MOVING_PH2");
                vm_carMoving.setWriteValue(carMoving ? "1" : "0");
                bool result = ISMControl.writeDeviceBlock(bcfApp, vm_carMoving);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                         Data: $"Set Car In Moving:{carMoving},result:{result}",
                         XID: MTS.EQPT_ID);
                MTS.CarInMoving = carMoving;
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }
            return true;
        }

        public override void GetMTL2OHxC_CarOutInterface(out bool carOutSafelyCheck, out bool carMoveComplete)
        {
            try
            {
                ValueRead vr_safety_check = bcfApp.getReadValueEvent(MTS.EqptObjectCate, MTS.EQPT_ID, "MTL_TO_OHXC_U2D_SAFETY_CHECK_PH2");
                // ValueRead vr_move_cmp = bcfApp.getReadValueEvent(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MTL_TO_OHXC_U2D_MOVE_COMPLETE");
                carOutSafelyCheck = (bool)vr_safety_check.getText();
                // carMoveComplete = (bool)vr_move_cmp.getText();
                carMoveComplete = false;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                carOutSafelyCheck = false;
                carMoveComplete = false;
            }
        }
        public override void GetMTL2OHxC_CarInInterface(out bool carOutSafelyCheck, out bool carInInterlock)
        {
            try
            {
                ValueRead vr_safety_check = bcfApp.getReadValueEvent(MTS.EqptObjectCate, MTS.EQPT_ID, "MTL_TO_OHXC_D2U_SAFETY_CHECK_PH2");
                ValueRead vr_car_in = bcfApp.getReadValueEvent(MTS.EqptObjectCate, MTS.EQPT_ID, "MTL_TO_OHXC_D2U_CAR_IN_INTERLOCK_PH2");
                carOutSafelyCheck = (bool)vr_safety_check.getText();
                carInInterlock = (bool)vr_car_in.getText();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                carOutSafelyCheck = false;
                carInInterlock = false;
            }
        }
        public override void OHxCResetAllhandshake()
        {
            try
            {
                ValueWrite vw_ReplyAlarmReport = bcfApp.getWriteValueEvent(MTS.EqptObjectCate, MTS.EQPT_ID, "OHXC_TO_MTL_REPLY_ALARM_REPORT_HS_PH2");
                ValueWrite vw_MTLAlarmResetRequest = bcfApp.getWriteValueEvent(MTS.EqptObjectCate, MTS.EQPT_ID, "OHXC_TO_MTL_ALARM_RESET_REQUEST_HS_PH2");
                ValueWrite vr_CarOutReply = bcfApp.getWriteValueEvent(MTS.EqptObjectCate, MTS.EQPT_ID, "OHXC_TO_MTL_CAR_OUT_REPLY_HS_PH2");
                ValueWrite vr_CarOutNotify = bcfApp.getWriteValueEvent(MTS.EqptObjectCate, MTS.EQPT_ID, "OHXC_TO_MTL_CAR_OUT_NOTIFY_HS_PH2");
                ValueWrite vr_ReplyCatInDataCheck = bcfApp.getWriteValueEvent(MTS.EqptObjectCate, MTS.EQPT_ID, "OHXC_TO_MTL_REPLY_CAR_IN_DATA_CHECK_HS_PH2");
                vw_ReplyAlarmReport.initWriteValue();
                vw_MTLAlarmResetRequest.initWriteValue();
                vr_CarOutReply.initWriteValue();
                vr_CarOutNotify.initWriteValue();
                vr_ReplyCatInDataCheck.initWriteValue();
                ISMControl.writeDeviceBlock(scApp.getBCFApplication(), vw_ReplyAlarmReport);
                ISMControl.writeDeviceBlock(scApp.getBCFApplication(), vw_MTLAlarmResetRequest);
                ISMControl.writeDeviceBlock(scApp.getBCFApplication(), vr_CarOutReply);
                ISMControl.writeDeviceBlock(scApp.getBCFApplication(), vr_CarOutNotify);
                ISMControl.writeDeviceBlock(scApp.getBCFApplication(), vr_ReplyCatInDataCheck);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
         Data: $"Reset All Handshake.",
         XID: MTS.EQPT_ID);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }

        public override void CarOutSafetyChcek(object sender, ValueChangedEventArgs args)
        {
            var recevie_function =
                scApp.getFunBaseObj<MtlToOHxC_CarOutSafetyCheck_PH2>(MTS.EQPT_ID) as MtlToOHxC_CarOutSafetyCheck_PH2;
            try
            {
                recevie_function.Read(bcfApp, MTS.EqptObjectCate, MTS.EQPT_ID);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                         Data: recevie_function.ToString(),
                         XID: MTS.EQPT_ID);
                MTS.CarOutSafetyCheck = recevie_function.SafetyCheck;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<MtlToOHxC_CarOutSafetyCheck_PH2>(recevie_function);

            }
        }
        //public override void MTL_Alarm_Report(object sender, ValueChangedEventArgs args)
        //{
        //    var recevie_function =
        //        scApp.getFunBaseObj<MtlToOHxC_AlarmReport>(MTS.EQPT_ID) as MtlToOHxC_AlarmReport;
        //    var send_function =
        //        scApp.getFunBaseObj<MtlToOHxC_ReplyAlarmReport>(MTS.EQPT_ID) as MtlToOHxC_ReplyAlarmReport;
        //    try
        //    {
        //        recevie_function.Read(bcfApp, MTS.EqptObjectCate, MTS.EQPT_ID);
        //        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
        //                 Data: recevie_function.ToString(),
        //                 XID: MTS.EQPT_ID);
        //        UInt16 error_code = recevie_function.ErrorCode;
        //        ProtocolFormat.OHTMessage.ErrorStatus status = (ProtocolFormat.OHTMessage.ErrorStatus)recevie_function.ErrorStatus;
        //        ushort hand_shake = recevie_function.Handshake;


        //        send_function.Handshake = hand_shake;
        //        send_function.Write(bcfApp, MTS.EqptObjectCate, MTS.EQPT_ID);
        //        MTS.SynchronizeTime = DateTime.Now;

        //        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
        //                 Data: send_function.ToString(),
        //                 XID: MTS.EQPT_ID);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex, "Exception");
        //    }
        //    finally
        //    {
        //        scApp.putFunBaseObj<MtlToOHxC_AlarmReport>(recevie_function);
        //        scApp.putFunBaseObj<MtlToOHxC_ReplyAlarmReport>(send_function);

        //    }
        //}
        public override void MTL_DATETIME(object sender, ValueChangedEventArgs args)
        {

            try
            {
                ValueRead vr = sender as ValueRead;
                UInt16[] dateTime = (UInt16[])vr.getText();
                MTS.SynchronizeTime = DateTime.Now;
                Console.WriteLine(String.Join(",", new List<UInt16>(dateTime).ConvertAll(i => i.ToString()).ToArray()));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }
        public override void MTL_Alive(object sender, ValueChangedEventArgs args)
        {
            var recevie_function =
                scApp.getFunBaseObj<MtlToOHxC_Alive_PH2>(MTS.EQPT_ID) as MtlToOHxC_Alive_PH2;
            try
            {
                recevie_function.Read(bcfApp, MTS.EqptObjectCate, MTS.EQPT_ID);
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTX,
                //         Data: recevie_function.ToString(),
                //         XID: eqpt.EQPT_ID);

                eqpt.Eq_Alive_Last_Change_time = DateTime.Now;
                eqpt.SynchronizeTime = DateTime.Now;

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<MtlToOHxC_Alive_PH2>(recevie_function);

            }
        }
        public override void MTL_Current_ID(object sender, ValueChangedEventArgs args)
        {
            var recevie_function =
                scApp.getFunBaseObj<MtlToOHxC_CurrentCarID_PH2>(MTS.EQPT_ID) as MtlToOHxC_CurrentCarID_PH2;
            try
            {
                recevie_function.Read(bcfApp, MTS.EqptObjectCate, MTS.EQPT_ID);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                         Data: recevie_function.ToString(),
                         XID: MTS.EQPT_ID);
                MTS.CurrentCarID = recevie_function.CarID.ToString();
                MTS.SynchronizeTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<MtlToOHxC_CurrentCarID_PH2>(recevie_function);

            }
        }
        public override void MTL_TO_OHXC_REPLY_OHXC_CAR_OUT_NOTIFY_HS(object sender, ValueChangedEventArgs args)
        {
            var recevie_function =
                scApp.getFunBaseObj<MtlToOHxC_ReplyCarOutNotify>(MTS.EQPT_ID) as MtlToOHxC_ReplyCarOutNotify;
            try
            {
                recevie_function.Read(bcfApp, MTS.EqptObjectCate, MTS.EQPT_ID);
                MTS.SynchronizeTime = DateTime.Now;
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                         Data: recevie_function.ToString(),
                         XID: MTS.EQPT_ID);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<MtlToOHxC_ReplyCarOutNotify>(recevie_function);

            }
        }


        public override void MTL_LFTStatus(object sender, ValueChangedEventArgs args)
        {
            var recevie_function =
                scApp.getFunBaseObj<MtlToOHxC_LFTStatus_PH2>(MTS.EQPT_ID) as MtlToOHxC_LFTStatus_PH2;
            try
            {
                recevie_function.Read(bcfApp, MTS.EqptObjectCate, MTS.EQPT_ID);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                         Data: recevie_function.ToString(),
                         XID: MTS.EQPT_ID);
                MTS.HasVehicle = recevie_function.HasVehicle;
                MTS.StopSingle = recevie_function.StopSingle;
                MTS.MTxMode = (ProtocolFormat.OHTMessage.MTxMode)recevie_function.Mode;
                MTS.VhInPosition = (ProtocolFormat.OHTMessage.VhInPosition)recevie_function.VhInPosition;
                MTS.MTSFrontDoorStatus = (ProtocolFormat.OHTMessage.MTSDoorStatus)recevie_function.FrontDoorStatus;
                MTS.MTSBackDoorStatus = (ProtocolFormat.OHTMessage.MTSDoorStatus)recevie_function.BackDoorStatus;
                MTS.SynchronizeTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<MtlToOHxC_LFTStatus_PH2>(recevie_function);

            }
        }
        public override void CarInSafetyChcek(object sender, ValueChangedEventArgs args)
        {
            var recevie_function =
                scApp.getFunBaseObj<MtlToOHxC_CarInSafetyCheck_PH2>(MTS.EQPT_ID) as MtlToOHxC_CarInSafetyCheck_PH2;
            try
            {
                recevie_function.Read(bcfApp, MTS.EqptObjectCate, MTS.EQPT_ID);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                         Data: recevie_function.ToString(),
                         XID: MTS.EQPT_ID);
                MTS.CarInSafetyCheck = recevie_function.SafetyCheck;
                MTS.SynchronizeTime = DateTime.Now;

                //if (eqpt.CarInSafetyCheck)
                //{
                //    scApp.MTLService.carInSafetyAndVehicleStatusCheck(eqpt);
                //}
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<MtlToOHxC_CarInSafetyCheck_PH2>(recevie_function);

            }
        }
        public override void MTL_CarOutRequest(object sender, ValueChangedEventArgs args)
        {

            var recevie_function =
                scApp.getFunBaseObj<MtlToOHxC_MtlCarOutRepuest_PH2>(MTS.EQPT_ID) as MtlToOHxC_MtlCarOutRepuest_PH2;
            var send_function =
                scApp.getFunBaseObj<OHxCToMtl_MtlCarOutReply_PH2>(MTS.EQPT_ID) as OHxCToMtl_MtlCarOutReply_PH2;
            try
            {
                recevie_function.Read(bcfApp, MTS.EqptObjectCate, MTS.EQPT_ID);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                         Data: recevie_function.ToString(),
                         XID: MTS.EQPT_ID);
                int pre_car_out_vh_num = recevie_function.CarID;
                ushort hand_shake = recevie_function.Handshake;
                if (hand_shake == 1)
                {
                    send_function.ReturnCode = 1;
                    if (recevie_function.Canacel == 1)
                    {
                        scApp.MTLService.carOutRequestCancle(MTS);
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                                 Data: $"Process MTS car out cancel",
                                 XID: MTS.EQPT_ID);
                    }
                    else if(recevie_function.MTLCarOutActionType !=1 && recevie_function.MTLCarOutActionType != 3)
                    {
                        send_function.ReturnCode = 2;
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                                 Data: $"Process MTS car out request, is success:{false},result:car out action type:{recevie_function.MTLCarOutActionType} is wrong",
                                 XID: MTS.EQPT_ID);
                    }
                    else
                    {
                        AVEHICLE pre_car_out_vh = scApp.VehicleBLL.cache.getVhByNum(pre_car_out_vh_num);

                        var car_out_check_result = scApp.MTLService.checkVhAndMTxCarOutStatus(this.MTS, null, pre_car_out_vh);
                        send_function.ReturnCode = car_out_check_result.isSuccess ? (ushort)1 : (ushort)2;
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                                 Data: $"Process MTS car out request, is success:{car_out_check_result.isSuccess},result:{car_out_check_result.result}",
                                 XID: MTS.EQPT_ID);

                    }
                }
                else
                {
                    send_function.ReturnCode = 0;
                }
                send_function.Handshake = hand_shake == 0 ? (ushort)0 : (ushort)1;
                send_function.Write(bcfApp, MTS.EqptObjectCate, MTS.EQPT_ID);

                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                         Data: send_function.ToString(),
                         XID: MTS.EQPT_ID);
                MTS.SynchronizeTime = DateTime.Now;
                //if (send_function.Handshake == 1 && send_function.ReturnCode == 1 )
                if (send_function.Handshake == 1 && send_function.ReturnCode == 1 && recevie_function.Canacel != 1)
                {
                    AVEHICLE pre_car_out_vh = scApp.VehicleBLL.cache.getVhByNum(pre_car_out_vh_num);
                    scApp.MTLService.processCarOutScenario(MTS, pre_car_out_vh);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<MtlToOHxC_MtlCarOutRepuest_PH2>(recevie_function);
                scApp.putFunBaseObj<OHxCToMtl_MtlCarOutReply_PH2>(send_function);
            }
        }

        public override void MTL_CarInRequest(object sender, ValueChangedEventArgs args)
        {
            var recevie_function =
                scApp.getFunBaseObj<MtlToOHxC_RequestCarInDataCheck_PH2>(MTS.EQPT_ID) as MtlToOHxC_RequestCarInDataCheck_PH2;
            var send_function =
                scApp.getFunBaseObj<OHxCToMtl_ReplyCarInDataCheck_PH2>(MTS.EQPT_ID) as OHxCToMtl_ReplyCarInDataCheck_PH2;
            try
            {
                recevie_function.Read(bcfApp, MTS.EqptObjectCate, MTS.EQPT_ID);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                         Data: recevie_function.ToString(),
                         XID: MTS.EQPT_ID);
                ushort vh_num = recevie_function.CarID;
                ushort hand_shake = recevie_function.Handshake;

                AVEHICLE pre_car_in_vh = scApp.VehicleBLL.cache.getVhByNum(vh_num);
                if (pre_car_in_vh != null)
                {
                    MaintainLift mtl = null;
                    //如果是MTS(即MTS1)的話，則需要去判斷MTL的狀態是否是可以通過的
                    if (SCUtility.isMatche(MTS.EQPT_ID, "MTS"))
                    {
                        mtl = scApp.EquipmentBLL.cache.GetMaintainLift();
                    }
                    var check_result = scApp.MTLService.checkVhAndMTxCarInStatus(MTS, mtl, pre_car_in_vh);
                    send_function.ReturnCode = check_result.isSuccess ? (UInt16)1 : (UInt16)3;
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                             Data: $"check mts car in result, is success:{check_result.isSuccess},result:{check_result.result}",
                             XID: MTS.EQPT_ID);
                }
                else
                {
                    send_function.ReturnCode = 2;
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                             Data: $"check mts car in result, vehicle num:{vh_num} not exist.",
                             XID: MTS.EQPT_ID);
                }
                send_function.Handshake = hand_shake;
                send_function.Write(bcfApp, MTS.EqptObjectCate, MTS.EQPT_ID);

                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                         Data: send_function.ToString(),
                         XID: MTS.EQPT_ID);

                MTS.SynchronizeTime = DateTime.Now;

                if (send_function.Handshake == 1 && send_function.ReturnCode == 1)
                {
                    scApp.MTLService.processCarInScenario(MTS);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<MtlToOHxC_RequestCarInDataCheck_PH2>(recevie_function);
                scApp.putFunBaseObj<OHxCToMtl_ReplyCarInDataCheck_PH2>(send_function);

            }
        }



        public override void MTL_Alarm_Report(object sender, ValueChangedEventArgs args)
        {
            var recevie_function =
                scApp.getFunBaseObj<MtlToOHxC_AlarmReport_PH2>(eqpt.EQPT_ID) as MtlToOHxC_AlarmReport_PH2;
            var send_function =
                scApp.getFunBaseObj<MtlToOHxC_ReplyAlarmReport_PH2>(eqpt.EQPT_ID) as MtlToOHxC_ReplyAlarmReport_PH2;
            try
            {
                recevie_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                         Data: recevie_function.ToString(),
                         XID: eqpt.EQPT_ID);
                UInt16 error_code = recevie_function.ErrorCode;
                ProtocolFormat.OHTMessage.ErrorStatus status = (ProtocolFormat.OHTMessage.ErrorStatus)recevie_function.ErrorStatus;
                ushort hand_shake = recevie_function.Handshake;

                send_function.Handshake = hand_shake;
                send_function.Write(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                eqpt.SynchronizeTime = DateTime.Now;
                if (hand_shake == 1)
                {
                    scApp.LineService.ProcessAlarmReport(eqpt.NODE_ID, eqpt.EQPT_ID, eqpt.Real_ID, "", error_code.ToString(), status);
                }

                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                         Data: send_function.ToString(),
                         XID: eqpt.EQPT_ID);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<MtlToOHxC_AlarmReport_PH2>(recevie_function);
                scApp.putFunBaseObj<MtlToOHxC_ReplyAlarmReport_PH2>(send_function);

            }
        }

        public override bool OHxC_AlarmResetRequest()
        {
            bool isSendSuccess = false;
            var send_function =
                scApp.getFunBaseObj<OHxCToMtl_AlarmResetRequest_PH2>(eqpt.EQPT_ID) as OHxCToMtl_AlarmResetRequest_PH2;
            var receive_function =
                scApp.getFunBaseObj<MtlToOHxC_AlarmResetReply_PH2>(eqpt.EQPT_ID) as MtlToOHxC_AlarmResetReply_PH2;
            try
            {
                //1.準備要發送的資料
                ValueRead vr_reply = receive_function.getValueReadHandshake
                    (bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                //2.紀錄發送資料的Log
                send_function.Handshake = 1;
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                         Data: send_function.ToString(),
                         XID: eqpt.EQPT_ID);
                //3.等待回復
                TrxMPLC.ReturnCode returnCode =
                    send_function.SendRecv(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID, vr_reply);
                //4.取得回復的結果
                if (returnCode == TrxMPLC.ReturnCode.Normal)
                {
                    receive_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                             Data: receive_function.ToString(),
                             XID: eqpt.EQPT_ID);
                    isSendSuccess = true;
                }
                send_function.Handshake = 0;
                send_function.resetHandshake(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNewPH2), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                         Data: send_function.ToString(),
                         XID: eqpt.EQPT_ID);

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<OHxCToMtl_AlarmResetRequest_PH2>(send_function);
                scApp.putFunBaseObj<MtlToOHxC_AlarmResetReply_PH2>(receive_function);
            }
            return (isSendSuccess);
        }

    }
}
