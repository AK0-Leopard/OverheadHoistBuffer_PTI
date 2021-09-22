using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.BLL;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.Data.VO.Interface;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Service
{
    public class MTLService
    {
        //public readonly string MTL_CAR_OUT_BUFFER_ADDRESS = "20292";
        //public readonly string MTL_CAR_IN_BUFFER_ADDRESS = "24294";
        //public readonly string MTL_ADDRESS = "20293";
        //public readonly string MTL_SYSTEM_IN_ADDRESS = "20198";
        const ushort CAR_ACTION_MODE_NO_ACTION = 0;
        const ushort CAR_ACTION_MODE_ACTION = 1;
        const ushort CAR_ACTION_MODE_ACTION_FOR_MCS_COMMAND = 2;
        VehicleService VehicleService = null;
        VehicleBLL vehicleBLL = null;
        ReportBLL reportBLL = null;
        private SCApplication scApp = null;
        //MaintainLift mtx = null;
        //string carOutVhID = "";
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public MTLService()
        {
        }
        public void start(SCApplication app)
        {
            scApp = app;
            List<AEQPT> eqpts = app.getEQObjCacheManager().getAllEquipment();
           
            //由於7/1更新完OHTC版本後，現場有發生一直跳Nats的例外導致最後程序也無法進行位置更新的問題
            //因此先取消該部分的Nats事件通知 //20210802 Kevin Wei

            //registerMTxStatusChengeEvent(eqpts);

            VehicleService = app.VehicleService;
            vehicleBLL = app.VehicleBLL;
            reportBLL = app.ReportBLL;
            //  mtl = app.getEQObjCacheManager().getEquipmentByEQPTID("MTL") as MaintainLift;
        }

        private void registerMTxStatusChengeEvent(List<AEQPT> eqpts)
        {
            foreach (var eqpt in eqpts)
            {
                if (eqpt is IMaintainDevice)
                {
                    IMaintainDevice maintainDevice = eqpt as IMaintainDevice;
                    if (maintainDevice is MaintainSpace)
                    {
                        MaintainSpace maintainSpace = eqpt as MaintainSpace;
                        maintainSpace.addEventHandler(nameof(MTLService), nameof(maintainSpace.Plc_Link_Stat), PublishMTSInfo);
                        maintainSpace.addEventHandler(nameof(MTLService), nameof(maintainSpace.Is_Eq_Alive), PublishMTSInfo);
                        maintainSpace.addEventHandler(nameof(MTLService), nameof(maintainSpace.MTxMode), PublishMTSInfo);
                        maintainSpace.addEventHandler(nameof(MTLService), nameof(maintainSpace.Interlock), PublishMTSInfo);
                        maintainSpace.addEventHandler(nameof(MTLService), nameof(maintainSpace.CurrentCarID), PublishMTSInfo);
                        maintainSpace.addEventHandler(nameof(MTLService), nameof(maintainSpace.CurrentPreCarOurDistance), PublishMTSInfo);
                        maintainSpace.addEventHandler(nameof(MTLService), nameof(maintainSpace.SynchronizeTime), PublishMTSInfo);
                        maintainSpace.addEventHandler(nameof(MTLService), nameof(maintainSpace.CarOutInterlock), PublishMTSInfo);
                        maintainSpace.addEventHandler(nameof(MTLService), nameof(maintainSpace.CarInMoving), PublishMTSInfo);
                    }
                    else if (maintainDevice is MaintainLift)
                    {
                        MaintainLift maintainLift = eqpt as MaintainLift;
                        maintainLift.addEventHandler(nameof(MTLService), nameof(maintainLift.Plc_Link_Stat), PublishMTLInfo);
                        maintainLift.addEventHandler(nameof(MTLService), nameof(maintainLift.Is_Eq_Alive), PublishMTLInfo);
                        maintainLift.addEventHandler(nameof(MTLService), nameof(maintainLift.MTxMode), PublishMTLInfo);
                        maintainLift.addEventHandler(nameof(MTLService), nameof(maintainLift.Interlock), PublishMTLInfo);
                        maintainLift.addEventHandler(nameof(MTLService), nameof(maintainLift.CurrentCarID), PublishMTLInfo);
                        maintainLift.addEventHandler(nameof(MTLService), nameof(maintainLift.MTLLocation), PublishMTLInfo);
                        maintainLift.addEventHandler(nameof(MTLService), nameof(maintainLift.CurrentPreCarOurDistance), PublishMTLInfo);
                        maintainLift.addEventHandler(nameof(MTLService), nameof(maintainLift.SynchronizeTime), PublishMTLInfo);
                        maintainLift.addEventHandler(nameof(MTLService), nameof(maintainLift.CarOutInterlock), PublishMTLInfo);
                        maintainLift.addEventHandler(nameof(MTLService), nameof(maintainLift.CarInMoving), PublishMTLInfo);
                    }
                }
            }

            VehicleService = scApp.VehicleService;
            vehicleBLL = scApp.VehicleBLL;
            reportBLL = scApp.ReportBLL;
            //  mtl = scApp.getEQObjCacheManager().getEquipmentByEQPTID("MTL") as MaintainLift;
        }


        //bool cancelCarOutRequest = false;
        //bool carOurSuccess = false;

        /// <summary>
        /// 處理人員由MTL執行CAR OUT時的流程
        /// </summary>
        /// <param name="vhNum"></param>
        /// <returns></returns>
        //public (bool isSuccess, string result) carOutRequset(IMaintainDevice mtx, int vhNum)
        //{
        //    AVEHICLE pre_car_out_vh = vehicleBLL.cache.getVhByNum(vhNum);
        //    if (pre_car_out_vh == null)
        //        return (false, $"vh num:{vhNum}, not exist.");
        //    else
        //    {
        //        bool isSuccess = true;
        //        string result = "";
        //        var check_result = checkVhAndMTxCarOutStatus(mtx, null, pre_car_out_vh);
        //        isSuccess = check_result.isSuccess;
        //        result = check_result.result;
        //        if (isSuccess)
        //        {
        //            (bool isSuccess, string result) process_result = default((bool isSuccess, string result));
        //            if (mtx is MaintainLift)
        //            {
        //                process_result = processCarOutScenario(mtx as MaintainLift, pre_car_out_vh);
        //            }
        //            else if (mtx is MaintainSpace)
        //            {
        //                process_result = processCarOutScenario(mtx as MaintainSpace, pre_car_out_vh);
        //            }
        //            else
        //            {
        //                return process_result;
        //            }
        //            isSuccess = process_result.isSuccess;
        //            result = process_result.result;
        //        }
        //        return (isSuccess, result);
        //    }
        //}
        /// <summary>
        /// 處理人員由OHTC執行CAR OUT時的流程
        /// </summary>
        /// <param name="vhID"></param>
        /// <returns></returns>
        //public (bool isSuccess, string result) carOutRequset(IMaintainDevice mtx, string vhID)
        //{
        //    AVEHICLE pre_car_out_vh = vehicleBLL.cache.getVhByID(vhID);
        //    bool isSuccess = true;
        //    string result = "";
        //    var check_result = checkVhAndMTxCarOutStatus(mtx, null, pre_car_out_vh);
        //    isSuccess = check_result.isSuccess;
        //    result = check_result.result;
        //    //2.向MTL發出Car out request
        //    //成功後開始向MTL發送該台Vh的當前狀態，並在裡面判斷是否有收到Cancel的命令，有的話要將資料清空
        //    //Trun on 給MTL的Interlock flag
        //    //將該台Vh變更為AutoToMtl
        //    if (isSuccess)
        //    {
        //        var send_result = mtx.carOutRequest((UInt16)pre_car_out_vh.Num);
        //        if (send_result.isSendSuccess && send_result.returnCode == 1)
        //        {
        //            (bool isSuccess, string result) process_result = default((bool isSuccess, string result));
        //            if (mtx is MaintainLift)
        //            {
        //                process_result = processCarOutScenario(mtx as MaintainLift, pre_car_out_vh);
        //            }
        //            else if (mtx is MaintainSpace)
        //            {
        //                process_result = processCarOutScenario(mtx as MaintainSpace, pre_car_out_vh);
        //            }
        //            else
        //            {
        //                return process_result;
        //            }
        //            isSuccess = process_result.isSuccess;
        //            result = process_result.result;
        //        }
        //        else
        //        {
        //            isSuccess = false;
        //            result = $"Request car fail,Send result:{send_result.isSendSuccess}, return code:{send_result.returnCode}";
        //        }
        //    }
        //    return (isSuccess, result);
        //}

        public (bool isSuccess, string result) checkVhAndMTxCarOutStatus(IMaintainDevice mtx, IMaintainDevice dockingMtx, AVEHICLE car_out_vh)
        {
            bool isSuccess = true;
            string result = "";

            //1.要判斷目前車子的狀態
            if (isSuccess && mtx.CarOutInterlock)
            {
                isSuccess = false;
                result = $"MTx:{mtx.DeviceID} Current CarOutInterlock:{mtx.CarOutInterlock}, can't excute cat out requset.";
            }

            if (isSuccess && car_out_vh == null)
            {
                isSuccess = false;
                result = $"vh not exist.";
                //如果car_out_vh是Null就直接return回去
                return (isSuccess, result);
            }
            string vh_id = car_out_vh.VEHICLE_ID;
            if (isSuccess && !car_out_vh.isTcpIpConnect)
            {
                isSuccess = false;
                result = $"vh id:{vh_id}, not connection.";
            }
            if (isSuccess && car_out_vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.Manual)
            {
                isSuccess = false;
                result = $"Vehicle:{vh_id}, current mode is:{car_out_vh.MODE_STATUS}, can't excute auto car out";
            }
            if (isSuccess && SCUtility.isEmpty(car_out_vh.CUR_SEC_ID) && SCUtility.isEmpty(car_out_vh.CUR_ADR_ID))
            {
                isSuccess = false;
                result = $"vh id:{vh_id}, both current section and current address are empty.";
            }
            if (isSuccess && SCUtility.isEmpty(car_out_vh.CUR_ADR_ID))
            {
                isSuccess = false;
                result = $"vh id:{vh_id}, current address is empty.";
            }

            //要判斷目前到車子所在位置到目的地(MTL/MTS)路徑是不是通的
            //KeyValuePair<string[], double> route_distance;
            double route_distance;
            if (isSuccess && !scApp.RouteGuide.checkRoadIsWalkable(car_out_vh.CUR_ADR_ID, mtx.DeviceAddress, true, out route_distance))
            {
                isSuccess = false;
                result = $"vh id:{vh_id}, current address:{car_out_vh.CUR_ADR_ID} to device:{mtx.DeviceID} of address id:{mtx.DeviceAddress} not walkable.";
            }

            //2.要判斷MTL的 Safety check是否有On且是否為Auto Mode
            if (isSuccess && !SCUtility.isEmpty(mtx.PreCarOutVhID))
            {
                isSuccess = false;
                result = $"MTL:{mtx.DeviceID} Current process car our vh:{mtx.PreCarOutVhID}, can't excute cat out again.";
            }

            if (isSuccess && !mtx.IsAlive)
            {
                isSuccess = false;
                result = $"MTL:{mtx.DeviceID} Current Alive:{mtx.IsAlive}, can't excute cat out requset.";
            }
            if (isSuccess && mtx.MTxMode == ProtocolFormat.OHTMessage.MTxMode.Manual)
            {
                isSuccess = false;
                result = $"MTL:{mtx.DeviceID} Current Mode:{mtx.MTxMode}, can't excute cat out requset.";
            }
            //if (isSuccess && !mtx.CarOutSafetyCheck)
            //{
            //    isSuccess = false;
            //    result = $"MTx:{mtx.DeviceID} CarOutSafetyCheck:{mtx.CarOutSafetyCheck}, can't excute cat out requset.";
            //}

            if (dockingMtx != null)
            {
                if (isSuccess && dockingMtx.MTxMode == ProtocolFormat.OHTMessage.MTxMode.Manual)
                {
                    isSuccess = false;
                    result = $"Docking MTx:{dockingMtx.DeviceID} Current Mode:{dockingMtx.MTxMode}, can't excute cat out requset.";
                }
                //if (!SCUtility.isMatche(car_out_vh.CUR_ADR_ID, dockingMtx.DeviceAddress))
                //{
                //    if (isSuccess && !dockingMtx.CarOutSafetyCheck)
                //    {
                //        isSuccess = false;
                //        result = $"Docking MTx:{dockingMtx.DeviceID} CarOutSafetyCheck:{dockingMtx.CarOutSafetyCheck}, can't excute cat out requset.";
                //    }
                //}
                if (isSuccess && !SCUtility.isEmpty(dockingMtx.PreCarOutVhID))
                {
                    isSuccess = false;
                    result = $"Docking MTx:{dockingMtx.DeviceID} has prepare car out vh:{dockingMtx.PreCarOutVhID}, can't excute cat out requset.";
                }
                //如果是MTL來確認是否可以 CAR OUT，則要判斷她前方的MTS 是否有車子在上面，
                //但如果是在該VH是在MTS上面則可以讓他下來
                if (mtx is MaintainLift &&
                    dockingMtx is MaintainSpace)
                {
                    if (!SCUtility.isMatche(car_out_vh.CUR_ADR_ID, dockingMtx.DeviceAddress))
                    {
                        if (isSuccess && dockingMtx.HasVehicle)
                        {
                            isSuccess = false;
                            result = $"Docking MTx:{dockingMtx.DeviceID} {nameof(dockingMtx.HasVehicle)}:{dockingMtx.HasVehicle}, can't excute cat out requset.";
                        }
                    }
                }
            }

            return (isSuccess, result);
        }

        public (bool isSuccess, string result) CarOurRequest(IMaintainDevice mtx, AVEHICLE car_out_vh)
        {
            bool is_success = false;
            string result = "";
            var send_result = mtx.carOutRequest((UInt16)car_out_vh.Num);
            is_success = send_result.isSendSuccess && send_result.returnCode == 1;
            if (!is_success)
            {
                result = $"MTL:{mtx.DeviceID} reject car our request. return code:{send_result.returnCode}";
            }
            else
            {
                result = "OK";
            }
            return (is_success, result);
        }

        public (bool isSuccess, string result) processCarOutScenario(MaintainLift mtx, AVEHICLE preCarOutVh)
        {
            string pre_car_out_vh_id = preCarOutVh.VEHICLE_ID;
            string pre_car_out_vh_ohtc_cmd_id = preCarOutVh.OHTC_CMD;
            string pre_car_out_vh_cur_adr_id = preCarOutVh.CUR_ADR_ID;
            bool isSuccess;
            string result = "OK";
            mtx.CancelCarOutRequest = false;
            mtx.CarOurSuccess = false;

            if (!SpinWait.SpinUntil(() => mtx.CarOutSafetyCheck == true &&
            //maintainDevice.CarOutActionTypeSystemOutToMTL == true && 
            (mtx.DokingMaintainDevice == null || mtx.DokingMaintainDevice.CarOutSafetyCheck == true), 60000))
            {
                isSuccess = false;
                result = $"Process car out scenario,but mtl:{mtx.DeviceID} status not ready " +
                $"{nameof(mtx.CarOutSafetyCheck)}:{mtx.CarOutSafetyCheck}";
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                         Data: result,
                         XID: mtx.DeviceID);
                return (isSuccess, result);
            }

            CarOutStart(mtx);
            //接著要開始等待MTS的兩個門都放下來之後，才可以將OHT開過來
            if (mtx.DokingMaintainDevice != null && mtx.DokingMaintainDevice is MaintainSpace)
            {
                MaintainSpace dockingMaintainSpace = mtx.DokingMaintainDevice as MaintainSpace;
                if (!SpinWait.SpinUntil(() => dockingMaintainSpace.MTSBackDoorStatus == MTSDoorStatus.Open &&
                                             dockingMaintainSpace.MTSFrontDoorStatus == MTSDoorStatus.Open,
                                             MTS_DOOR_OPEN_TIME_OUT_ms))
                {
                    isSuccess = false;
                    result = $"Process car out scenario,but mts:{dockingMaintainSpace.DeviceID} status not ready " +
                        $"{nameof(dockingMaintainSpace.MTSBackDoorStatus)}:{dockingMaintainSpace.MTSBackDoorStatus}," +
                        $"{nameof(dockingMaintainSpace.MTSFrontDoorStatus)}:{dockingMaintainSpace.MTSFrontDoorStatus}";
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                             Data: result,
                             XID: dockingMaintainSpace.DeviceID);
                    CarOutFinish(mtx);
                    return (isSuccess, result);
                }
            }

            //isSuccess = VehicleService.doReservationVhToMaintainsBufferAddress(pre_car_out_vh_id, MTL_CAR_OUT_BUFFER_ADDRESS);
            isSuccess = VehicleService.doReservationVhToMaintainsBufferAddress(pre_car_out_vh_id, mtx.MTL_SYSTEM_OUT_ADDRESS);
            if (isSuccess && SCUtility.isEmpty(pre_car_out_vh_ohtc_cmd_id))
            {
                //在收到OHT的ID:132-命令結束後或者在變為AutoLocal後此時OHT沒有命令的話則會呼叫此Function來創建一個Transfer command，讓Vh移至移動至System out上
                if (SCUtility.isMatche(pre_car_out_vh_cur_adr_id, mtx.MTL_SYSTEM_OUT_ADDRESS))
                {
                    VehicleService.doAskVhToMaintainsAddress(pre_car_out_vh_id, mtx.MTL_ADDRESS);
                }
                else
                {
                    VehicleService.doAskVhToSystemOutAddress(pre_car_out_vh_id, mtx.MTL_SYSTEM_OUT_ADDRESS);
                }
            }
            if (isSuccess)
            {
                //carOutVhID = pre_car_out_vh_id;
                mtx.PreCarOutVhID = pre_car_out_vh_id;
                Task.Run(() => RegularUpdateRealTimeCarInfo(mtx, preCarOutVh));
            }
            else
            {
                //mtx.SetCarOutInterlock(false);
                CarOutFinish(mtx);
                isSuccess = false;
                result = $"Reservation vh to mtl fail.";
            }
            return (isSuccess, result);
        }

        private void CarOutStart(MaintainLift mtx)
        {
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                     Data: $"Process CarOutStart!",
                     XID: mtx.DeviceID);

            mtx.SetCarOutInterlock(true);
            if (mtx.DokingMaintainDevice != null &&
                mtx.DokingMaintainDevice is MaintainSpace)
            {
                //因為MTL的前方有DockingMTS，因此要觸發CAR IN INTERLOCK，通知將門放下
                mtx.DokingMaintainDevice.SetCarInMoving(true);
            }
        }
        private void CarOutFinish(MaintainLift mtx)
        {
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                     Data: $"Process CarOutFinish!",
                     XID: mtx.DeviceID);
            mtx.SetCarOutInterlock(false);
            if (mtx.DokingMaintainDevice != null &&
                mtx.DokingMaintainDevice is MaintainSpace)
            {
                mtx.DokingMaintainDevice.SetCarInMoving(false);
            }
        }
        public (bool isSuccess, string result) processCarOutScenario(MaintainSpace mtx, AVEHICLE preCarOutVh)
        {
            string pre_car_out_vh_id = preCarOutVh.VEHICLE_ID;
            string pre_car_out_vh_ohtc_cmd_id = preCarOutVh.OHTC_CMD;
            string pre_car_out_vh_cur_adr_id = preCarOutVh.CUR_ADR_ID;
            bool isSuccess;
            string result = "";
            mtx.CancelCarOutRequest = false;
            mtx.CarOurSuccess = false;

            if (!SpinWait.SpinUntil(() => mtx.CarOutSafetyCheck == true, 30000))
            {
                isSuccess = false;
                result = $"Process car out scenario,but mts:{mtx.DeviceID} status not ready " +
                $"{nameof(mtx.CarOutSafetyCheck)}:{mtx.CarOutSafetyCheck}";
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                 Data: result,
                 XID: mtx.DeviceID);
                return (false, result);
            }

            CarOutStart(mtx);
            //if (!SpinWait.SpinUntil(() => mtx.MTSBackDoorStatus == MTSDoorStatus.Open, 20000))
            //需要判斷MTS的門是否已經開啟
            if (!SpinWait.SpinUntil(() => mtx.MTSBackDoorStatus == MTSDoorStatus.Open, MTS_DOOR_OPEN_TIME_OUT_ms))
            {
                result = $"mts:{mtx.DeviceID}, status not ready  {nameof(mtx.MTSBackDoorStatus)}:{mtx.MTSBackDoorStatus} after interlock on 20 sec, can't excute car out";
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                         Data: result,
                         XID: mtx.DeviceID);
                CarOutFinish(mtx);
                return (false, result);
            }
            isSuccess = VehicleService.doReservationVhToMaintainsSpace(pre_car_out_vh_id);
            if (isSuccess && SCUtility.isEmpty(pre_car_out_vh_ohtc_cmd_id))
            {
                //在收到OHT的ID:132-命令結束後或者在變為AutoLocal後此時OHT沒有命令的話則會呼叫此Function來創建一個Transfer command，讓Vh移至移動至System out上
                isSuccess = VehicleService.doAskVhToSystemOutAddress(pre_car_out_vh_id, mtx.MTS_ADDRESS);
            }
            if (isSuccess)
            {
                //carOutVhID = pre_car_out_vh_id;
                mtx.PreCarOutVhID = pre_car_out_vh_id;
                Task.Run(() => RegularUpdateRealTimeCarInfo(mtx, preCarOutVh));
            }
            else
            {
                //mtx.SetCarOutInterlock(false);
                CarOutFinish(mtx);
                isSuccess = false;
                result = $"Reservation vh to mtl fail.";
            }
            return (isSuccess, result);
        }

        private void CarOutStart(MaintainSpace mtx)
        {
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                     Data: $"Process CarOutStart!",
                     XID: mtx.DeviceID);
            mtx.SetCarOutInterlock(true);
        }

        private void CarOutFinish(MaintainSpace mtx)
        {
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                     Data: $"Process CarOutFinish!",
                     XID: mtx.DeviceID);
            mtx.SetCarOutInterlock(false);
        }

        private void RegularUpdateRealTimeCarInfo(IMaintainDevice mtx, AVEHICLE carOurVh)
        {
            do
            {
                UInt16 car_id = (ushort)carOurVh.Num;
                UInt16 action_mode = 0;
                if (carOurVh.ACT_STATUS == ProtocolFormat.OHTMessage.VHActionStatus.Commanding)
                {
                    if (!SCUtility.isEmpty(carOurVh.MCS_CMD))
                    {
                        action_mode = CAR_ACTION_MODE_ACTION_FOR_MCS_COMMAND;
                    }
                    else
                    {
                        action_mode = CAR_ACTION_MODE_ACTION;
                    }
                }
                else
                {
                    action_mode = CAR_ACTION_MODE_NO_ACTION;
                }
                UInt16 cst_exist = (ushort)carOurVh.HAS_CST;
                UInt16 current_section_id = 0;
                UInt16.TryParse(carOurVh.CUR_SEC_ID, out current_section_id);
                UInt16 current_address_id = 0;
                UInt16.TryParse(carOurVh.CUR_ADR_ID, out current_address_id);
                UInt32 buffer_distance = 0;
                UInt16 speed = (ushort)carOurVh.Speed;

                mtx.CurrentPreCarOurID = car_id;
                mtx.CurrentPreCarOurActionMode = action_mode;
                mtx.CurrentPreCarOurCSTExist = cst_exist;
                mtx.CurrentPreCarOurSectionID = current_section_id;
                mtx.CurrentPreCarOurAddressID = current_address_id;
                mtx.CurrentPreCarOurDistance = buffer_distance;
                mtx.CurrentPreCarOurSpeed = speed;

                mtx.setCarRealTimeInfo(car_id, action_mode, cst_exist, current_section_id, current_address_id, buffer_distance, speed);

                //如果在移動過程中，MTx突然變成手動模式的話，則要將原本在移動的車子取消命令
                if (mtx.MTxMode == MTxMode.Manual || !mtx.CarOutSafetyCheck)
                {
                    carOutRequestCancle(mtx, true);
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                             Data: $"Device:{mtx.DeviceID} mtx mode suddenly turned mode:{mtx.MTxMode} or car out safety check change:{mtx.CarOutSafetyCheck}, " +
                             $"so urgent cancel vh:{mtx.PreCarOutVhID} of command.",
                             XID: mtx.DeviceID);
                    break;
                }
                //如果是MTL的話，要去檢查他的Doking device(MTS)是否狀態有變化，有的話也要將命令給取消
                if (mtx is MaintainLift)
                {
                    IMaintainDevice dokingMaintainDevice = (mtx as MaintainLift).DokingMaintainDevice;
                    if (dokingMaintainDevice != null)
                    {
                        if (!SCUtility.isMatche(dokingMaintainDevice.DeviceSegment, carOurVh.CUR_SEG_ID) &&
                            (dokingMaintainDevice.MTxMode == MTxMode.Manual || !dokingMaintainDevice.CarOutSafetyCheck))
                        {
                            carOutRequestCancle(mtx, true);
                            LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                                     Data: $"Device:{dokingMaintainDevice.DeviceID} of mtx mode suddenly turned mode:{dokingMaintainDevice.MTxMode} or car out safety check change:{dokingMaintainDevice.CarOutSafetyCheck}, " +
                                     $"so urgent cancel vh:{mtx.PreCarOutVhID} of command.",
                                     XID: dokingMaintainDevice.DeviceID);
                            break;
                        }
                    }
                }

                SpinWait.SpinUntil(() => false, 200);
            } while (!mtx.CancelCarOutRequest && !mtx.CarOurSuccess);

            //mtx.setCarRealTimeInfo(0, 0, 0, 0, 0, 0, 0);
        }

        public void carOutComplete(IMaintainDevice mtx)
        {
            mtx.CarOurSuccess = true;
            //carOutVhID = "";
            mtx.PreCarOutVhID = "";
            //mtx.SetCarOutInterlock(false);
            if (mtx is MaintainLift)
            {
                CarOutFinish(mtx as MaintainLift);
            }
            else if (mtx is MaintainSpace)
            {
                CarOutFinish(mtx as MaintainSpace);
            }
        }

        public void carOutRequestCancle(IMaintainDevice mtx)
        {
            carOutRequestCancle(mtx, false);
        }
        public void carOutRequestCancle(IMaintainDevice mtx, bool isForceFinish)
        {
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                     Data: $"Process car out cancel request. mtx:{mtx.DeviceID}, pre car out vh:{mtx.PreCarOutVhID}, is force finish:{isForceFinish}",
                     XID: mtx.DeviceID);
            //將原本的在等待Carout的Vh改回AutoRemote
            mtx.CancelCarOutRequest = true;
            //if (!SCUtility.isEmpty(carOutVhID))
            //{
            //    VehicleService.doRecoverModeStatusToAutoRemote(carOutVhID);
            //}
            //carOutVhID = "";
            if (!SCUtility.isEmpty(mtx.PreCarOutVhID))
            {
                VehicleService.doRecoverModeStatusToAutoRemote(mtx.PreCarOutVhID);
                AVEHICLE pre_car_out_vh = vehicleBLL.cache.getVhByID(mtx.PreCarOutVhID);
                if (!SCUtility.isEmpty(pre_car_out_vh?.OHTC_CMD))
                {
                    ACMD_OHTC cmd = scApp.CMDBLL.getCMD_OHTCByID(pre_car_out_vh.OHTC_CMD);
                    if (cmd != null)
                    {
                        if (cmd.CMD_TPYE == E_CMD_TYPE.MoveToMTL || cmd.CMD_TPYE == E_CMD_TYPE.SystemOut ||
                            cmd.CMD_TPYE == E_CMD_TYPE.SystemIn || cmd.CMD_TPYE == E_CMD_TYPE.MTLHome)
                        {
                            //如果是強制被取消(Safety check突然關閉)的時候，要先下一次暫停給車子
                            if (isForceFinish)
                            {
                                VehicleService.PauseRequest
                                    (pre_car_out_vh.VEHICLE_ID, PauseEvent.Pause, SCAppConstants.OHxCPauseType.Normal);
                            }
                            VehicleService.doAbortCommand
                                (pre_car_out_vh, pre_car_out_vh.OHTC_CMD, ProtocolFormat.OHTMessage.CMDCancelType.CmdCancel);
                        }
                    }
                    else
                    {
                        if (cmd.CMD_TPYE == E_CMD_TYPE.MoveToMTL || cmd.CMD_TPYE == E_CMD_TYPE.SystemOut ||
                            cmd.CMD_TPYE == E_CMD_TYPE.SystemIn || cmd.CMD_TPYE == E_CMD_TYPE.MTLHome)
                        {
                            //如果是強制被取消(Safety check突然關閉)的時候，要先下一次暫停給車子
                            if (isForceFinish)
                            {
                                VehicleService.PauseRequest
                                    (pre_car_out_vh.VEHICLE_ID, PauseEvent.Pause, SCAppConstants.OHxCPauseType.Normal);
                            }
                            VehicleService.doAbortCommand
                                (pre_car_out_vh, pre_car_out_vh.OHTC_CMD, ProtocolFormat.OHTMessage.CMDCancelType.CmdCancel);
                        }
                    }

                }
                //如果OHT已經在MTS/MTL的Segment上時，
                //就不能將他的對應訊號關閉
                if (SCUtility.isMatche(mtx.DeviceSegment, pre_car_out_vh.CUR_SEG_ID))
                {
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                             Data: $"Process car out cancel request. mtx:{mtx.DeviceID}, pre car out vh:{mtx.PreCarOutVhID}, is force finish:{isForceFinish}," +
                                   $"But vh current section is in MTL segment:{mtx.DeviceSegment} .can't trun off car out single ",
                             XID: mtx.DeviceID);
                    return;
                }
            }

            //mtx.SetCarOutInterlock(false);
            //mtx.PreCarOutVhID = "";

            mtx.PreCarOutVhID = "";
            if (mtx is MaintainLift)
            {
                CarOutFinish(mtx as MaintainLift);
            }
            else if (mtx is MaintainSpace)
            {
                CarOutFinish(mtx as MaintainSpace);
            }
        }


        public void carInSafetyAndVehicleStatusCheck(MaintainLift mtl)
        {
            if (!mtl.CarInSafetyCheck || mtl.MTxMode != ProtocolFormat.OHTMessage.MTxMode.Auto || mtl.MTLLocation != MTLLocation.Upper)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                         Data: $"Device:{mtl.DeviceID} car in safety check in on, but mts mode:{mtl.MTxMode} or Location:{mtl.MTLLocation}, can't excute car in.",
                         XID: mtl.DeviceID);
                return;
            }


            CarInStart(mtl);
            //在收到MTL的 Car in safety check後，就可以叫Vh移動至Car in 的buffer區(MTL Home)
            //不過要先判斷vh是否已經在Auto模式下如果是則先將它變成AutoLocal的模式

            AVEHICLE car_in_vh = vehicleBLL.cache.getVhByAddressID(mtl.MTL_ADDRESS);
            if (car_in_vh != null && car_in_vh.isTcpIpConnect)
            {
                if (car_in_vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.Manual)
                {
                    VehicleService.ModeChangeRequest(car_in_vh.VEHICLE_ID, OperatingVHMode.OperatingAuto);
                    //if (SpinWait.SpinUntil(() => car_in_vh.MODE_STATUS == VHModeStatus.AutoMtl, 10000))
                    if (SpinWait.SpinUntil(() => IsAutoMTLReady(car_in_vh), 10000))
                    {
                        //mtl.SetCarInMoving(true);
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                            Data: $"vh:{car_in_vh.VEHICLE_ID} request car in, OHTC Ready to create system in command.",
                             XID: mtl.DeviceID,
                            VehicleID: car_in_vh.VEHICLE_ID);
                        bool create_result = VehicleService.doAskVhToCarInBufferAddress(car_in_vh.VEHICLE_ID, mtl.MTL_CAR_IN_BUFFER_ADDRESS);
                        if (create_result)
                        {
                            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                            Data: $"vh:{car_in_vh.VEHICLE_ID} request car in, OHTC create system in command  successful.",
                             XID: mtl.DeviceID,
                            VehicleID: car_in_vh.VEHICLE_ID);
                        }
                        else
                        {
                            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                                Data: $"vh:{car_in_vh.VEHICLE_ID} request car in, OHTC create system in command failes.",
                                 XID: mtl.DeviceID,
                                VehicleID: car_in_vh.VEHICLE_ID);
                            CarInFinish(mtl);
                        }
                    }
                    else
                    {
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                                 Data: $"vh:{car_in_vh.VEHICLE_ID} request car in, but can't change to auto mode.",
                                 XID: mtl.DeviceID,
                                 VehicleID: car_in_vh.VEHICLE_ID);
                        CarInFinish(mtl);
                    }
                }
                else if (car_in_vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.AutoMtl)
                {
                    //mtl.SetCarInMoving(true);
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                        Data: $"vh:{car_in_vh.VEHICLE_ID} request car in, OHTC Ready to create system in command.",
                         XID: mtl.DeviceID,
                        VehicleID: car_in_vh.VEHICLE_ID);
                    bool create_result = VehicleService.doAskVhToCarInBufferAddress(car_in_vh.VEHICLE_ID, mtl.MTL_CAR_IN_BUFFER_ADDRESS);
                    if (create_result)
                    {
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                        Data: $"vh:{car_in_vh.VEHICLE_ID} request car in, OHTC create system in command  successful.",
                         XID: mtl.DeviceID,
                        VehicleID: car_in_vh.VEHICLE_ID);
                    }
                    else
                    {
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                            Data: $"vh:{car_in_vh.VEHICLE_ID} request car in, OHTC create system in command failes.",
                             XID: mtl.DeviceID,
                            VehicleID: car_in_vh.VEHICLE_ID);
                        CarInFinish(mtl);
                    }
                }
                else
                {
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                             Data: $"vh:{car_in_vh.VEHICLE_ID} request car in, but status is incorrect current status:{car_in_vh.MODE_STATUS}.",
                             XID: mtl.DeviceID,
                             VehicleID: car_in_vh.VEHICLE_ID);
                }
            }
            else
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                     Data: $"Request car in, but no vehicle at MTL or vehicle is not connected.",
                    XID: mtl.DeviceID);
            }
        }

        private bool IsAutoMTLReady(AVEHICLE carInVh)
        {
            string vh_id = carInVh.VEHICLE_ID;
            if (carInVh.MODE_STATUS == VHModeStatus.AutoMtl)
            {
                return true;
            }
            else
            {
                Task.Run(() => scApp.VehicleService.VehicleStatusRequest(vh_id, true));
                SpinWait.SpinUntil(() => false, 1000);
                return false;
            }
        }

        private void CarInStart(MaintainLift mtl)
        {
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                     Data: $"Process CarInStart!",
                     XID: mtl.DeviceID);
            mtl.SetCarInMoving(true);
        }
        private void CarInFinish(MaintainLift mtl)
        {
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                     Data: $"Process CarInFinish!",
                     XID: mtl.DeviceID);
            mtl.SetCarInMoving(false);
        }

        public (bool isSuccess, string result, UInt16 resultCode) checkVhAndMTxCarInStatus(IMaintainDevice mtx, IMaintainDevice dockingMtx, AVEHICLE car_in_vh)
        {
            bool isSuccess = true;
            string result = "";
            UInt16 resultCode = 1;

            string vh_id = car_in_vh.VEHICLE_ID;
            //1.要判斷目前車子的狀態
            //要判斷目前MTS是否有在處理CAR IN流程
            if (isSuccess && mtx.CarInMoving)
            {
                isSuccess = false;
                result = $"MTx:{mtx.DeviceID} Current CarInMoving:{mtx.CarInMoving}, can't excute cat in requset.";
                resultCode = 3;
            }
            if (isSuccess && !car_in_vh.isTcpIpConnect)
            {
                isSuccess = false;
                result = $"vh id:{vh_id}, not connection.";
                resultCode = 4;
            }
            //if (isSuccess && car_in_vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.Manual)
            //{
            //    isSuccess = false;
            //    result = $"Vehicle:{vh_id}, current mode is:{car_in_vh.MODE_STATUS}, can't excute auto car out";
            //}
            if (isSuccess && SCUtility.isEmpty(car_in_vh.CUR_SEC_ID) && SCUtility.isEmpty(car_in_vh.CUR_ADR_ID))
            {
                isSuccess = false;
                result = $"vh id:{vh_id}, both current section and current address are empty.";
                resultCode = 5;
            }
            if (isSuccess && !SCUtility.isMatche(car_in_vh.CUR_ADR_ID, mtx.DeviceAddress))
            {
                isSuccess = false;
                result = $"vh id:{vh_id}, current address:{car_in_vh.CUR_ADR_ID} not match mtx device address:{mtx.DeviceAddress}.";
                resultCode = 6;
            }


            //2.要判斷MTL的 Safety check是否有On且是否為Auto Mode
            //if (isSuccess && mtx.MTxMode == ProtocolFormat.OHTMessage.MTxMode.Manual)
            if (isSuccess && mtx.MTxMode != ProtocolFormat.OHTMessage.MTxMode.Auto)
            {
                isSuccess = false;
                result = $"MTx:{mtx.DeviceID} Current Mode:{mtx.MTxMode}, can't excute cat in requset.";
                resultCode = 7;
            }
            //3.要判斷MTS的 Car In Safety Check是否是準備好的
            //if (mtx is MaintainSpace)
            //{
            //    if (isSuccess && !mtx.CarInSafetyCheck)
            //    {
            //        isSuccess = false;
            //        result = $"MTx:{mtx.DeviceID} {nameof(mtx.CarInSafetyCheck)}:{mtx.CarInSafetyCheck}, can't excute cat in requset.";
            //    }
            //}
            //4.若有Docking的Device，則需要再判斷一次他的狀態
            if (dockingMtx != null)
            {
                if (isSuccess && dockingMtx.MTxMode == ProtocolFormat.OHTMessage.MTxMode.Manual)
                {
                    isSuccess = false;
                    result = $"Docking MTx:{dockingMtx.DeviceID} Current Mode:{dockingMtx.MTxMode}, can't excute cat in requset.";
                    resultCode = 8;
                }
                //if (isSuccess && !dockingMtx.CarOutSafetyCheck)
                //{
                //    isSuccess = false;
                //    result = $"Docking MTx:{dockingMtx.DeviceID} CarInSafetyCheck:{dockingMtx.CarOutSafetyCheck}, can't excute cat in requset.";
                //}
                //要執行Car in的時候，如果是MTS而且有Docking MTL，則要確認MTL上面是否有車子
                if (mtx is MaintainSpace &&
                    dockingMtx is MaintainLift)
                {
                    if (dockingMtx.HasVehicle)
                    {
                        isSuccess = false;
                        result = $"Docking MTx:{dockingMtx.DeviceID} {nameof(dockingMtx.HasVehicle)}:{dockingMtx.HasVehicle}, can't excute cat in requset.";
                        resultCode = 9;
                    }
                }
            }
            return (isSuccess, result, resultCode);
        }

        public (bool isSuccess, string result) CarInFromMTSRequest(MaintainSpace mtx, AVEHICLE car_out_vh)
        {
            bool is_success = false;
            string result = "";
            var send_result = mtx.carInRequest((UInt16)car_out_vh.Num);
            is_success = send_result.isSendSuccess && send_result.returnCode == 1;
            if (!is_success)
            {
                result = $"MTL:{mtx.DeviceID} reject car in request. return code:{send_result.returnCode}";
            }
            else
            {
                result = "OK";
            }
            return (is_success, result);
        }

        const int MTS_DOOR_OPEN_TIME_OUT_ms = 20000;
        public (bool isSuccess, string result) processCarInScenario(MaintainSpace mts)
        {
            bool isSuccess = true;
            string result = "OK";

            CarInStart(mts);
            //在收到MTL的 Car in safety check後，就可以叫Vh移動至Car in 的buffer區(MTL Home)
            //不過要先判斷vh是否已經在Auto模式下如果是則先將它變成AutoLocal的模式
            if (!SpinWait.SpinUntil(() => /*mts.CarInSafetyCheck &&*/ mts.MTxMode == MTxMode.Auto, 10000))
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                         Data: $"mts:{mts.DeviceID}, status not ready CarInSafetyCheck:{mts.CarInSafetyCheck},Mode:{mts.MTxMode} ,can't excute car in",
                         XID: mts.DeviceID);
                result = $"mts:{mts.DeviceID}, status not ready CarInSafetyCheck:{mts.CarInSafetyCheck},Mode:{mts.MTxMode} ,can't excute car in";
                isSuccess = false;
                CarInFinish(mts);
                return (isSuccess, result);
            }

            //在車子要Car In的時候，要判斷MTS的前門是否已經開啟
            if (!SpinWait.SpinUntil(() => mts.MTSFrontDoorStatus == MTSDoorStatus.Open, MTS_DOOR_OPEN_TIME_OUT_ms))
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                         Data: $"mts:{mts.DeviceID}, status not ready {nameof(mts.MTSFrontDoorStatus)}:{ mts.MTSFrontDoorStatus} ,can't excute car in",
                         XID: mts.DeviceID);
                CarInFinish(mts);
                result = $"mts:{mts.DeviceID}, status not ready {nameof(mts.MTSFrontDoorStatus)}:{ mts.MTSFrontDoorStatus} ,can't excute car in";
                isSuccess = false;
                return (isSuccess, result);
            }

            AVEHICLE car_in_vh = vehicleBLL.cache.getVhByAddressID(mts.MTS_ADDRESS);
            //if (car_in_vh != null && car_in_vh.isTcpIpConnect)
            if (car_in_vh != null)
            {
                if (car_in_vh.isTcpIpConnect)
                {
                    if (car_in_vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.Manual)
                    {
                        VehicleService.ModeChangeRequest(car_in_vh.VEHICLE_ID, ProtocolFormat.OHTMessage.OperatingVHMode.OperatingAuto);

                        if (SpinWait.SpinUntil(() => car_in_vh.MODE_STATUS == VHModeStatus.AutoMts, 10000))
                        {
                            //mts.SetCarInMoving(true);
                            VehicleService.doAskVhToSystemInAddress(car_in_vh.VEHICLE_ID, mts.MTS_SYSTEM_IN_ADDRESS);
                        }
                        else
                        {
                            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                                     Data: $"Process car in scenario:{mts.DeviceID} fail. ask vh change to auto mode time out",
                                     XID: mts.DeviceID,
                                     VehicleID: car_in_vh.VEHICLE_ID);
                            isSuccess = false;
                            result = $"Process car in scenario:{mts.DeviceID} fail. ask vh change to auto mode time out";
                            CarInFinish(mts);
                        }
                    }
                    else if (car_in_vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.AutoMts)
                    {
                        //mts.SetCarInMoving(true);
                        VehicleService.doAskVhToSystemInAddress(car_in_vh.VEHICLE_ID, mts.MTS_SYSTEM_IN_ADDRESS);
                    }
                }
                else
                {
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                             Data: $"Process car in scenario fail, mts:{mts.DeviceID}. because on mts of vh:{car_in_vh.VEHICLE_ID} is disconnect ",
                             XID: mts.DeviceID,
                             VehicleID: car_in_vh.VEHICLE_ID);
                    //mts.SetCarInMoving(false);
                    isSuccess = false;
                    result = $"Process car in scenario fail, mts:{mts.DeviceID}. because on mts of vh:{car_in_vh.VEHICLE_ID} is disconnect";
                    CarInFinish(mts);
                }
            }
            else
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                         Data: $"Process car in scenario fail, mts:{mts.DeviceID}. because no vh in mts",
                         XID: mts.DeviceID);
                //mts.SetCarInMoving(false);
                isSuccess = false;
                result = $"Process car in scenario fail, mts:{mts.DeviceID}. because no vh in mts";
                CarInFinish(mts);
            }
            return (isSuccess, result);
        }

        private void CarInStart(MaintainSpace mts)
        {
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                     Data: $"Process CarInStart!",
                     XID: mts.DeviceID);
            mts.SetCarInMoving(true);
            if (mts.DokingMaintainDevice != null)
            {
                if (mts.DokingMaintainDevice is MaintainLift)
                {
                    mts.DokingMaintainDevice.SetCarOutInterlock(true);
                }
            }
        }
        private void CarInFinish(MaintainSpace mts)
        {
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                     Data: $"Process CarInFinish!",
                     XID: mts.DeviceID);
            mts.SetCarInMoving(false);
            if (mts.DokingMaintainDevice != null)
            {
                if (mts.DokingMaintainDevice is MaintainLift)
                {
                    mts.DokingMaintainDevice.SetCarOutInterlock(false);
                }
            }
        }

        public void carInComplete(IMaintainDevice mtx, string vhID)
        {
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLService), Device: SCAppConstants.DeviceName.DEVICE_NAME_MTx,
                     Data: $"Process carInComplete!",
                     XID: mtx.DeviceID,
                     VehicleID: vhID);
            VehicleService.doRecoverModeStatusToAutoRemote(vhID);
            //mtx.SetCarInMoving(false);
            if (mtx is MaintainLift)
            {
                CarInFinish(mtx as MaintainLift);
                VehicleService.doAskVhToSystemInAddress(vhID, (mtx as MaintainLift).MTL_SYSTEM_IN_ADDRESS);
            }
            else if (mtx is MaintainSpace)
            {
                CarInFinish(mtx as MaintainSpace);
            }
            //如果是MTS機台，因為在準備通過MTL時，會將CAR OUT訊號 TURN ON，因此在結束的時候要將它復歸

            //List<AMCSREPORTQUEUE> reportqueues = new List<AMCSREPORTQUEUE>();
            //reportBLL.newReportVehicleInstalled(vhID, reportqueues);
        }


        public void PublishMTSInfo(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                MaintainSpace eqpt = sender as MaintainSpace;
                if (sender == null) return;
                byte[] line_serialize = BLL.LineBLL.Convert2GPB_MTLMTSInfo(eqpt);
                scApp.getNatsManager().PublishAsync
                    (SCAppConstants.NATS_SUBJECT_MTLMTS, line_serialize);
                //TODO 要改用GPP傳送
                //var line_Serialize = ZeroFormatter.ZeroFormatterSerializer.Serialize(line);
                //scApp.getNatsManager().PublishAsync
                //    (string.Format(SCAppConstants.NATS_SUBJECT_LINE_INFO), line_Serialize);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }


        public void PublishMTLInfo(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                MaintainLift eqpt = sender as MaintainLift;
                if (sender == null) return;
                byte[] line_serialize = BLL.LineBLL.Convert2GPB_MTLMTSInfo(eqpt);
                scApp.getNatsManager().PublishAsync
                    (SCAppConstants.NATS_SUBJECT_MTLMTS, line_serialize);
                //TODO 要改用GPP傳送
                //var line_Serialize = ZeroFormatter.ZeroFormatterSerializer.Serialize(line);
                //scApp.getNatsManager().PublishAsync
                //    (string.Format(SCAppConstants.NATS_SUBJECT_LINE_INFO), line_Serialize);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }

    }
}
