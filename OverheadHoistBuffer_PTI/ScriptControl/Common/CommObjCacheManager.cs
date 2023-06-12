//*********************************************************************************
//      EQObjCacheManager.cs
//*********************************************************************************
// File Name: EQObjCacheManager.cs
// Description: Equipment Cache Manager
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.ConfigHandler;
using com.mirle.ibg3k0.bcf.Data.FlowRule;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Data.VO;
using NLog;
using com.mirle.ibg3k0.sc.ConfigHandler;
using com.mirle.ibg3k0.sc.Data;

namespace com.mirle.ibg3k0.sc.Common
{

    public class CommObjCacheManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static CommObjCacheManager instance = null;
        private static Object _lock = new Object();
        private SCApplication scApp = null;
        //Cache Object
        //Section
        private List<AADDRESS> Addresses;
        private List<ASECTION> Sections;
        //Segment
        private List<ASEGMENT> Segments;
        private List<PortDef> PortDefs;
        private List<ReserveEnhanceInfo> ReserveEnhanceInfos;
        //BlockMaster
        private List<ABLOCKZONEMASTER> BlockZoneMasters;
        private List<AHIDZONEMASTER> HIDZoneMasters;
        private List<APARKZONEDETAIL> ParkZoneDetails;
        private List<APARKZONEMASTER> ParkZoneMasters;
        private CommonInfo CommonInfo;
        private List<ParkingZone> ParkingZones = null;

        private CommObjCacheManager() { }
        public static CommObjCacheManager getInstance()
        {
            lock (_lock)
            {
                if (instance == null)
                {
                    instance = new CommObjCacheManager();
                }
                return instance;
            }
        }


        public void setContext()
        {
        }

        public void start(SCApplication _app)
        {
            scApp = _app;
            Addresses = scApp.MapBLL.loadAllAddress();
            Segments = scApp.MapBLL.loadAllSegments();
            Sections = scApp.MapBLL.loadAllSection();
            BlockZoneMasters = scApp.MapBLL.loadAllBlockZoneMaster();
            HIDZoneMasters = scApp.HIDBLL.loadAllHidZoneMaster();
            ParkZoneDetails = scApp.ParkBLL.LoadAllParkZoneDetails();
            ParkZoneMasters = scApp.ParkBLL.LoadAllParkZoneMaster();
            ParkZoneDetails.ForEach(detail => SCUtility.TrimAllParameter(detail));
            ParkZoneMasters.ForEach(master => SCUtility.TrimAllParameter(master));

            ParkingZones = scApp.ParkingZoneBLL.getAllParkingZoneData();

            ReserveEnhanceInfos = scApp.ReserveEnhanceInfoDao.loadReserveInfos(scApp);
            foreach (ASEGMENT segment in Segments)
            {
                segment.SetSectionList(scApp.SectionBLL);
            }
            foreach (ABLOCKZONEMASTER block_zone_master in BlockZoneMasters)
            {
                block_zone_master.SetBlockDetailList(scApp.MapBLL);
            }
            foreach (AADDRESS addresses in Addresses)
            {
                addresses.initialAddressType();
            }
            foreach (var park_zone_master in ParkZoneMasters)
            {
                park_zone_master.setParkDetails(ParkZoneDetails);
            }
            CommonInfo = new CommonInfo();
        }


        public void setPortDefsInfo()
        {
            PortDefs = scApp.PortDefBLL.GetOHB_ALLPortData_WithoutShelf(scApp.getEQObjCacheManager().getLine().LINE_ID);
        }


        public void stop()
        {
            clearCache();
        }


        private void clearCache()
        {
            Sections.Clear();
        }


        private void removeFromDB()
        {
            //not implement yet.
        }

        #region 取得各種EQ Object的方法
        //Section
        public ASECTION getSection(string sec_id)
        {
            return Sections.Where(z => z.SEC_ID.Trim() == sec_id.Trim()).FirstOrDefault();
        }
        public ASECTION getSection(string adr1, string adr2)
        {
            return Sections.Where(s => (s.FROM_ADR_ID.Trim() == adr1.Trim() && s.TO_ADR_ID.Trim() == adr2.Trim())
                                    || (s.FROM_ADR_ID.Trim() == adr2.Trim() && s.TO_ADR_ID.Trim() == adr1.Trim())).FirstOrDefault();
        }
        public List<ASECTION> getSections()
        {
            return Sections;
        }
        //Segment
        public List<ASEGMENT> getSegments()
        {
            return Segments;
        }
        public List<PortDef> getPortDefs()
        {
            return PortDefs.ToList();
        }
        public List<ReserveEnhanceInfo> getReserveEnhanceInfos()
        {
            return ReserveEnhanceInfos;
        }
        public List<AADDRESS> GetAddresses()
        {
            return Addresses == null ? new List<AADDRESS>() : Addresses.ToList();
        }

        #endregion


        private void setValueToPropety<T>(ref T sourceObj, ref T destinationObj)
        {
            BCFUtility.setValueToPropety(ref sourceObj, ref destinationObj);
        }
        //Block Master Zone
        public List<ABLOCKZONEMASTER> getBlockMasterZone()
        {
            return BlockZoneMasters;
        }
        public List<AHIDZONEMASTER> getHIDMasterZone()
        {
            return HIDZoneMasters;
        }

        public List<APARKZONEMASTER> LoadParkZoneMater()
        {
            return ParkZoneMasters.ToList();
        }
        public List<APARKZONEDETAIL> LoadParkZoneDetail()
        {
            return ParkZoneDetails.ToList();
        }
        public List<ParkingZone> GetAllParkingZonesInfos()
        {
            return ParkingZones;
        }



        #region 將最新物件資料，放置入Cache的方法
        //NotImplemented
        #endregion


        #region 從DB取得最新EQ Object，並更新Cache
        //NotImplemented
        #endregion



    }
}
