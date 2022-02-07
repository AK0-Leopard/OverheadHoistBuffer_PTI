//*********************************************************************************
//      IdleVehicleTimerAction.cs
//*********************************************************************************
// File Name: IdleVehicleTimerAction.cs
// Description: 
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//**********************************************************************************
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.bcf.Data.TimerAction;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.TimerAction
{
    /// <summary>
    /// Class IdleVehicleTimerAction.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.TimerAction.ITimerAction" />
    class IdleVehicleTimerAction : ITimerAction
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// The sc application
        /// </summary>
        protected SCApplication scApp = null;


        /// <summary>
        /// Initializes a new instance of the <see cref="IdleVehicleTimerAction"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="intervalMilliSec">The interval milli sec.</param>
        public IdleVehicleTimerAction(string name, long intervalMilliSec)
            : base(name, intervalMilliSec)
        {

        }

        /// <summary>
        /// Initializes the start.
        /// </summary>
        public override void initStart()
        {
            //do nothing
            scApp = SCApplication.getInstance();

        }

        private long syncPoint = 0;

        /// <summary>
        /// Timer Action的執行動作
        /// </summary>
        /// <param name="obj">The object.</param>
        public override void doProcess(object obj)
        {
            if (!DebugParameter.isOpenAdjustmentParkingZone) return;
            if (System.Threading.Interlocked.Exchange(ref syncPoint, 1) == 0)
            {
                try
                {
                    //0.先判斷各區域的停車位是否皆已移到最高順位
                    scApp.ParkBLL.cache.tryAdjustTheVhParkingPositionByParkZoneAndPrio();
                    //1.各PackZone水位檢查
                    //  a.找出是否有低於目前水位的PackZone
                    //  b.找出最近的PackZone至少高於水位下限一台，若有則派至此處。
                    List<APARKZONEMASTER> vhNotEnoughParkZones = null;
                    APARKZONEDETAIL nearbyZoneDetail = null;
                    if (!scApp.ParkBLL.cache.checkParkZoneLowerBorder(out vhNotEnoughParkZones))
                    {
                        foreach (APARKZONEMASTER vhNotEnoughParkZone in vhNotEnoughParkZones)
                        {
                            if (scApp.ParkBLL.cache.tryFindNearbyParkZoneHasVhToSupport(vhNotEnoughParkZone, out nearbyZoneDetail))
                            {

                                var vhNotEnoughParkDeatil = vhNotEnoughParkZone.getEntryParkDetail();
                                if (vhNotEnoughParkDeatil == null || nearbyZoneDetail == null)
                                {
                                    continue;
                                }
                                bool isSccess = false;
                                isSccess = scApp.CMDBLL.doCreatTransferCommand(nearbyZoneDetail.CAR_ID, string.Empty, string.Empty, E_CMD_TYPE.Move_Park
                                      , nearbyZoneDetail.ADR_ID, vhNotEnoughParkDeatil.ADR_ID, 0, 0);
                                if (isSccess)
                                {
                                    if (nearbyZoneDetail != null)
                                    {
                                        APARKZONEMASTER nearbyZoneMaster = scApp.ParkBLL.
                                            getParkZoneMasterByParkZoneID(nearbyZoneDetail.PARK_ZONE_ID);
                                        if (nearbyZoneMaster.PARK_TYPE == E_PARK_TYPE.OrderByAsc)
                                        {
                                            scApp.ParkBLL.cache.tryAdjustTheVhParkingPositionByParkZoneAndPrio(nearbyZoneMaster);
                                        }
                                    }
                                    if (vhNotEnoughParkZone != null &&
                                        vhNotEnoughParkZone.PARK_TYPE == E_PARK_TYPE.OrderByDes)
                                    {
                                        scApp.ParkBLL.cache.tryAdjustTheVhParkingPositionByParkZoneAndPrio(vhNotEnoughParkZone);
                                    }

                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                }
                finally
                {
                    System.Threading.Interlocked.Exchange(ref syncPoint, 0);
                }
            }
        }
    }
}