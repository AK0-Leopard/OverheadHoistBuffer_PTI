using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data;
using com.mirle.ibg3k0.sc.Data.DAO;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.BLL
{
    public class ParkingZoneBLL
    {
        public SCApplication scApp;
        public Database dataBase { get; private set; }
        public Cache cache { get; private set; }
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ParkingZoneBLL()
        {
        }

        public void start(SCApplication _app)
        {
            scApp = _app;
            dataBase = new Database(scApp.ParkZoneMasterDao, scApp.ParkZoneDetailDao);
            cache = new Cache(scApp.getCommObjCacheManager());
        }

        public List<ParkingZone> getAllParkingZoneData()
        {
            try
            {
                List<ParkingZone> rtnlist = new List<ParkingZone>();
                //List<E_VH_TYPE> dedicated_type = new List<E_VH_TYPE>();
                //foreach(E_VH_TYPE vhtype in Enum.GetValues(typeof(E_VH_TYPE)))
                //{
                //    dedicated_type.Add(E_VH_TYPE);
                //}
                List<APARKZONEMASTER> pzmasters = dataBase.loadAllParkingZoneMaster();
                Dictionary<string, List<string>> PZaddress = dataBase.loadAllParkingZoneAddress();
                List<string> pzadrs;
                foreach (var pzmaster in pzmasters)
                {
                    if (pzmaster.IS_ACTIVE)
                    {
                        bool getaddress = PZaddress.TryGetValue(pzmaster.PARK_ZONE_ID.Trim(), out pzadrs);
                        if (getaddress)
                        {
                            ParkingZone pztmp = new ParkingZone()
                            {
                                ParkingZoneID = pzmaster.PARK_ZONE_ID.Trim(),
                                ParkAddressIDs = pzadrs,
                                AllowedVehicleTypes = new List<E_VH_TYPE>(),
                                //EntryAddress = pzmaster.ENTRY_ADR_ID.Trim(),
                                Capacity = pzmaster.TOTAL_BORDER,
                                LowWaterlevel = pzmaster.LOWER_BORDER,
                                order = pzmaster.PARK_TYPE
                            };
                            if (pzmaster.VEHICLE_TYPE == E_VH_TYPE.None)
                            {
                                foreach (E_VH_TYPE vhtype in Enum.GetValues(typeof(E_VH_TYPE)))
                                {
                                    pztmp.AllowedVehicleTypes.Add(vhtype);
                                }
                            }
                            else
                                pztmp.AllowedVehicleTypes.Add(pzmaster.VEHICLE_TYPE);
                            rtnlist.Add(pztmp);
                        }
                    }
                }
                return rtnlist;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
                throw;
            }
        }

        public List<string> LoadAvoidParkingzoneAddresses(AVEHICLE avoidVH)
        {
            //return cache.LoadAllParkingZoneInfo().Where(PZ => PZ.AllowedVehicleTypes.Contains(avoidVH.VEHICLE_TYPE) && (PZ.UsedCount < (PZ.Capacity + 1)))
            var enough_position_parking_zone = cache.LoadAllParkingZoneInfo().Where(PZ => PZ.AllowedVehicleTypes.Contains(avoidVH.VEHICLE_TYPE) && (PZ.UsedCount < (PZ.Capacity))).ToList();
            var check_vh_is_in_parking_zone = cache.tryGetParkingZonebyAdr(avoidVH.CUR_ADR_ID);

            if (check_vh_is_in_parking_zone.isFind)
            {
                if (!enough_position_parking_zone.Contains(check_vh_is_in_parking_zone.parkingZone))
                {
                    //兩台車 2 - 1 < 2 (一台包含自己)
                    //三台車 3 - 1 < 2 (一台包含自己)
                    if (check_vh_is_in_parking_zone.parkingZone.UsedCount - 1 < check_vh_is_in_parking_zone.parkingZone.Capacity)
                    {
                        enough_position_parking_zone.Add(check_vh_is_in_parking_zone.parkingZone);
                    }
                }
            }
            return enough_position_parking_zone.SelectMany(PZ => PZ.ParkAddressIDs).ToList();
            //return cache.LoadAllParkingZoneInfo().Where(PZ => PZ.AllowedVehicleTypes.Contains(avoidVH.VEHICLE_TYPE) && (PZ.UsedCount < (PZ.Capacity)))
            //                                 .SelectMany(PZ => PZ.ParkAddressIDs).ToList();
        }

        public class Database
        {
            ParkZoneMasterDao ParkZoneMasterDao = null;
            ParkZoneDetailDao ParkZoneDetailDao = null;
            public Database(ParkZoneMasterDao PZMasterDao, ParkZoneDetailDao PZDetailDao)
            {
                ParkZoneMasterDao = PZMasterDao;
                ParkZoneDetailDao = PZDetailDao;
            }
            public List<APARKZONEMASTER> loadAllParkingZoneMaster()
            {
                List<APARKZONEMASTER> rtnlist = null;
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    rtnlist = ParkZoneMasterDao.loadAll(con);
                }
                return rtnlist;
            }
            public Dictionary<string, List<string>> loadAllParkingZoneAddress()
            {
                Dictionary<string, List<string>> rtnlist = null;
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    rtnlist = ParkZoneDetailDao.loadGroupByPZidAndPZaddresslist(con);
                }
                return rtnlist;
            }
        }
        public class Cache
        {
            CommObjCacheManager CommObjCacheManager = null;
            public Cache(CommObjCacheManager commObjCacheManager)
            {
                CommObjCacheManager = commObjCacheManager;
            }
            public List<ParkingZone> LoadAllParkingZoneInfo()
            {
                return CommObjCacheManager.GetAllParkingZonesInfos();
            }
            public (bool isFind, ParkingZone parkingZone) tryGetParkingZonebyAdr(string address)
            {
                List<ParkingZone> parkingZones = LoadAllParkingZoneInfo();

                foreach (ParkingZone pz in parkingZones)
                {
                    if (pz.ParkAddressIDs.Contains(address))
                    {
                        return (true, pz);
                    }
                }
                return (false, null);
            }
        }
    }
}
