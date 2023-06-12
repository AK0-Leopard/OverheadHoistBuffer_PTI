using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.BLL;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Service
{
    public class ZoneService
    {
        VehicleBLL vehicleBLL = null;
        ParkingZoneBLL ParkingZoneBLL = null;
        CMDBLL cmdBLL = null;
        GuideBLL guideBLL = null;
        SCApplication scApp = null;
        VehicleService VehicleService = null;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public void Start(SCApplication app)
        {
            scApp = app;
            vehicleBLL = scApp.VehicleBLL;
            cmdBLL = scApp.CMDBLL;
            guideBLL = scApp.GuideBLL;
            ParkingZoneBLL = scApp.ParkingZoneBLL;
            VehicleService = scApp.VehicleService;
        }



        private ParkingZone GetParkingZonebyAdr(string address)
        {
            List<ParkingZone> parkingZones = scApp.ParkingZoneBLL.cache.LoadAllParkingZoneInfo();
            ParkingZone rtn_parkingzone = null;

            foreach (ParkingZone pz in parkingZones)
            {
                if (pz.ParkAddressIDs.Contains(address))
                {
                    rtn_parkingzone = pz;
                    return rtn_parkingzone;
                }
            }
            return rtn_parkingzone;
        }


        public bool TryChooseVehicleToDriveIn(ParkingZone underWaterLevelParkingZone, out AVEHICLE bestVH)
        {
            var pkvhs = GetParkedVH();
            bestVH = null;
            //AVEHICLE bestVH = null;
            //HashSet<ParkingZone> candidateParkingZones = parkingZones.Where(p => p.GetUsage(pkvhs, ufcmds) - p.LowWaterlevel > 0).ToHashSet();

            if (pkvhs.Count == 0)
                return false;
            else
            {
                pkvhs = pkvhs.Where(vh => underWaterLevelParkingZone.AllowedVehicleTypes.Contains(vh.VEHICLE_TYPE)).ToList();
            }
            string correspond_vh_ids = string.Join(",", pkvhs.Select(vh => vh.VEHICLE_ID));
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ZoneService), Device: string.Empty,
                Data: $"找尋低水位停車區:{underWaterLevelParkingZone.ParkingZoneID} 可支援車輛，進行目前閒置車輛:{correspond_vh_ids}。");

            int minDistance = int.MaxValue;
            int distToPark;
            bool is_walkable;
            // 找到最近的車
            foreach (var pkvh in pkvhs)
            {
                //is_walkable = guideBLL.IsRoadWalkable(pkvh.CUR_ADR_ID, underWaterLevelParkingZone.EntryAddress, out distToPark);
                var check_result = guideBLL.IsRoadWalkable(pkvh.CUR_ADR_ID, underWaterLevelParkingZone.EntryAddress);
                //if (is_walkable && distToPark != 0 && distToPark < minDistance)
                if (check_result.isSuccess && check_result.distance > 0 && check_result.distance < minDistance)
                {
                    minDistance = check_result.distance;
                    bestVH = pkvh;
                }
            }
            // 若找到的車在停車區內且該停車區的水位低於其最低水位 則不拉車
            if ((minDistance < int.MaxValue) && (bestVH != null))
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ZoneService), Device: string.Empty,
                    Data: $"低水位停車區:{underWaterLevelParkingZone.ParkingZoneID}，找到最近的閒置車輛:{bestVH.VEHICLE_ID} 開始嘗試拉車...");
                ParkingZone bestvhPZ = GetParkingZonebyAdr(bestVH.CUR_ADR_ID);
                //if ((bestvhPZ != null) && (bestvhPZ.GetUsage(pkvhs, ufcmds) < bestvhPZ.LowWaterlevel))
                if ((bestvhPZ != null) && (bestvhPZ.UsedCount < bestvhPZ.LowWaterlevel))
                {
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ZoneService), Device: string.Empty,
                        Data: $"最近的閒置車輛:{bestVH.VEHICLE_ID} 位於低水位停車區:{bestvhPZ.ParkingZoneID}，取消拉車功能。");
                    bestVH = null;
                    return false;
                }
                else
                {
                    //if (canCreatPZCommand(bestVH))
                    var check_can_creat_parking_command = bestVH.CanCreatParkingCommand(scApp.CMDBLL);
                    //if (bestVH.CanCreatParkingCommand(scApp.CMDBLL).is_can)
                    if (check_can_creat_parking_command.is_can)
                    {
                        //bool is_success = VehicleService.Command.Move(bestVH.VEHICLE_ID, underWaterLevelParkingZone.EntryAddress).isSuccess;

                        bool is_success = scApp.CMDBLL.doCreatTransferCommand(bestVH.VEHICLE_ID,
                                                                              cmd_type: E_CMD_TYPE.Move,
                                                                              destination_address: underWaterLevelParkingZone.EntryAddress);
                    }
                    else
                    {
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ZoneService), Device: string.Empty,
                                      Data: $"最近的閒置車輛:{bestVH.VEHICLE_ID} 不能創建停車命令 reason:{check_can_creat_parking_command.result}，取消拉車功能。");
                        bestVH = null;
                        return false;
                    }
                }
                return true;
            }
            else
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ZoneService), Device: string.Empty,
                              Data: $"並無找到適合的Idle車輛");
                return false;
            }
        }
        public void pushAllParkingZone()
        {
            List<ParkingZone> parkingZones = scApp.ParkingZoneBLL.cache.LoadAllParkingZoneInfo();
            foreach (ParkingZone pz in parkingZones)
            {
                if (pz.order == E_PARK_TYPE.OrderByDes)
                    pushVhInParkingZone(pz);
            }
        }
        public void pushVhInParkingZone(ParkingZone parkingzone)
        {
            List<string> RV_ParkAddressIDs = new List<string>(parkingzone.ParkAddressIDs.Count());
            for (int i = parkingzone.ParkAddressIDs.Count() - 1; i >= 0; i--)
            {
                RV_ParkAddressIDs.Add(parkingzone.ParkAddressIDs[i]);
            }
            int movedestcount = 0;
            foreach (var pz_addr in RV_ParkAddressIDs)
            {
                //AVEHICLE pre_moveVH = vehicleBLL.cache.getVhOnAddress(pz_addr);
                AVEHICLE pre_moveVH = vehicleBLL.cache.getVhByAddressID(pz_addr);

                if (pre_moveVH != null)
                {
                    //move vh
                    if (RV_ParkAddressIDs[movedestcount] != pz_addr)
                    {
                        //bool is_success = VehicleService.Command.Move(pre_moveVH.VEHICLE_ID, RV_ParkAddressIDs[movedestcount]).isSuccess;
                        bool is_success = scApp.CMDBLL.doCreatTransferCommand(pre_moveVH.VEHICLE_ID,
                                                                              cmd_type: E_CMD_TYPE.Move,
                                                                              destination_address: RV_ParkAddressIDs[movedestcount]);
                    }
                    movedestcount++;
                }
            }
        }

        private List<AVEHICLE> GetParkedVH()
        {
            List<AVEHICLE> pkvhs = vehicleBLL.cache.getIdleVhs(scApp.CMDBLL);
            return pkvhs;
        }
        public (List<ParkingZone> Hightpzs, List<ParkingZone> Lowpzs) GetHightAndLowWaterLevelPZ()
        {
            try
            {
                List<ParkingZone> rtn_hightpzs = new List<ParkingZone>();
                List<ParkingZone> rtn_Lowpzs = new List<ParkingZone>();
                List<ParkingZone> parkingZones = scApp.ParkingZoneBLL.cache.LoadAllParkingZoneInfo();
                List<AVEHICLE> pkvhs = GetParkedVH();
                List<ACMD_OHTC> ufcmds = cmdBLL.cache.loadUnfinishMoveCmd();
                int parkedvhcount;
                StringBuilder logstring = new StringBuilder();

                foreach (ParkingZone pz in parkingZones)
                {
                    parkedvhcount = pz.GetUsage(pkvhs, ufcmds);
                    if (parkedvhcount > pz.LowWaterlevel)
                    {
                        rtn_hightpzs.Add(pz);
                    }
                    else if (parkedvhcount < pz.LowWaterlevel)
                    {
                        rtn_Lowpzs.Add(pz);
                    }
                    logstring.Append($"parkzone ID: {pz.ParkingZoneID}, LowWaterlevel: {pz.LowWaterlevel}, waterLevel: {parkedvhcount}; ");
                }

                logstring.Append("hightlevel:");
                foreach (ParkingZone pz in rtn_hightpzs)
                    logstring.Append(" " + pz.ParkingZoneID);
                logstring.Append("; lowlevel:");
                foreach (ParkingZone pz in rtn_Lowpzs)
                    logstring.Append(" " + pz.ParkingZoneID);

                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ZoneService), Device: string.Empty,
                    Data: logstring.ToString());

                return (rtn_hightpzs, rtn_Lowpzs);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
                return (null, null);
            }
        }
        private bool canCreatPZCommand(AVEHICLE bestvh)
        {
            if (bestvh.IsError)
            {
                return false;
            }
            else
            {
                bool is_can = bestvh.isTcpIpConnect &&
                       (bestvh.MODE_STATUS == VHModeStatus.AutoRemote || bestvh.MODE_STATUS == VHModeStatus.AutoLocal) &&
                       bestvh.ACT_STATUS == VHActionStatus.NoCommand &&
                       !bestvh.IsProcessingCommandFinish &&                           //如果是在結束命令中的話，就也先不要進行趕車因為此時可能有機會把準備在該port的貨給他載走 By Kevin
                       !scApp.CMDBLL.isCMD_OHTCQueueByVh(bestvh.VEHICLE_ID);
                //!scApp.CMDBLL.HasCMD_MCSInQueue();

                //若被趕車車輛存在unload Transfer命令，則不趕車
                bool has_unload_command = cmdBLL.cache.hasCMD_MCSByHostSourcePort(bestvh.VEHICLE_ID);

                if (has_unload_command)
                {
                    return false;
                }

                return is_can;
            }
        }
    }
}
