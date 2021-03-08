using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Mirle.AK0.Hlt.Utils;
using Mirle.AK0.Hlt.ReserveSection.Map.ViewModels;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;

namespace com.mirle.ibg3k0.sc.BLL
{
    public class ReserveBLL
    {
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private MapViewModel mapAPI { get; set; }
        private sc.Common.CommObjCacheManager commObjCacheManager { get; set; }


        private EventHandler reserveStatusChange;
        private object _reserveStatusChangeEventLock = new object();
        public event EventHandler ReserveStatusChange
        {
            add
            {
                lock (_reserveStatusChangeEventLock)
                {
                    reserveStatusChange -= value;
                    reserveStatusChange += value;
                }
            }
            remove
            {
                lock (_reserveStatusChangeEventLock)
                {
                    reserveStatusChange -= value;
                }
            }
        }

        private void onReserveStatusChange()
        {
            reserveStatusChange?.Invoke(this, EventArgs.Empty);
        }

        public ReserveBLL()
        {
        }
        public void start(SCApplication _app)
        {
            mapAPI = _app.getReserveSectionAPI();
            commObjCacheManager = _app.getCommObjCacheManager();
        }

        public bool DrawAllReserveSectionInfo()
        {
            bool is_success = false;
            try
            {
                mapAPI.RedrawBitmap(false);
                mapAPI.DrawOverlapRR();
                mapAPI.RefreshBitmap();
                mapAPI.ClearOverlapRR();
                is_success = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public System.Windows.Media.Imaging.BitmapSource GetCurrentReserveInfoMap()
        {
            return mapAPI.MapBitmapSource;
        }

        public (double x, double y, bool isTR50) GetHltMapAddress(string adrID)
        {
            var adr_obj = mapAPI.GetAddressObjectByID(adrID);
            return (adr_obj.X, adr_obj.Y, adr_obj.IsTR50);
        }

        public virtual HltResult TryAddVehicleOrUpdate(string vhID, string currentSectionID, double vehicleX, double vehicleY, float vehicleAngle, double speedMmPerSecond,
                                           HltDirection sensorDir, HltDirection forkDir)
        {
            LogHelper.Log(logger: logger, LogLevel: NLog.LogLevel.Debug, Class: nameof(ReserveBLL), Device: "AGV",
               Data: $"add vh in reserve system: vh:{vhID},x:{vehicleX},y:{vehicleY},angle:{vehicleAngle},speedMmPerSecond:{speedMmPerSecond},sensorDir:{sensorDir},forkDir:{forkDir}",
               VehicleID: vhID);
            var hlt_vh = new HltVehicle(vhID, vehicleX, vehicleY, vehicleAngle, speedMmPerSecond, sensorDirection: sensorDir, forkDirection: forkDir, currentSectionID: currentSectionID);
            HltResult result = mapAPI.TryAddOrUpdateVehicle(hlt_vh, isKeepRestSection: false);
            onReserveStatusChange();

            return result;
        }
        public virtual HltResult TryAddVehicleOrUpdate(string vhID, string adrID, float angle = 0)
        {
            var adr_obj = mapAPI.GetAddressObjectByID(adrID);
            var hlt_vh = new HltVehicle(vhID, adr_obj.X, adr_obj.Y, angle, sensorDirection: HltDirection.NESW);
            HltResult result = mapAPI.TryAddOrUpdateVehicle(hlt_vh);
            onReserveStatusChange();

            return result;
        }
        public void RemoveManyReservedSectionsByVIDSID(string vhID, string sectionID)
        {
            //int sec_id = 0;
            //int.TryParse(sectionID, out sec_id);
            string sec_id = SCUtility.Trim(sectionID);
            mapAPI.RemoveManyReservedSectionsByVIDSID(vhID, sec_id);
            onReserveStatusChange();
        }

        public void RemoveVehicle(string vhID)
        {
            var vh = mapAPI.GetVehicleObjectByID(vhID);
            if (vh != null)
            {
                mapAPI.RemoveVehicle(vh);
            }
        }

        public string GetCurrentReserveSection()
        {
            StringBuilder sb = new StringBuilder();
            var current_reserve_sections = mapAPI.HltReservedSections;
            sb.AppendLine("Current reserve section");
            foreach (var reserve_section in current_reserve_sections)
            {
                sb.AppendLine($"section id:{reserve_section.RSMapSectionID} vh id:{reserve_section.RSVehicleID}");
            }
            return sb.ToString();
        }
        public List<string> loadCurrentReserveSections(string vhID)
        {
            StringBuilder sb = new StringBuilder();
            var current_reserve_sections = mapAPI.HltReservedSections;
            var vh_of_current_reserve_sections = current_reserve_sections.
                                                 Where(reserve_info => SCUtility.isMatche(reserve_info.RSVehicleID, vhID)).
                                                 Select(reserve_info => reserve_info.RSMapSectionID).
                                                 ToList();
            return vh_of_current_reserve_sections;
        }
        public HltVehicle GetHltVehicle(string vhID)
        {
            return mapAPI.GetVehicleObjectByID(vhID);
        }

        public virtual HltResult TryAddReservedSection(string vhID, string sectionID,
            HltDirection sensorDir = HltDirection.ForwardReverse, HltDirection forkDir = HltDirection.None,
            DriveDirction driveDirection = DriveDirction.DriveDirNone, bool isAsk = false)
        {
            string sec_id = SCUtility.Trim(sectionID);
            int vehicle_direction = getVehicleDirection(driveDirection);
            HltResult result = mapAPI.TryAddReservedSection(vhID, sec_id, sensorDir, forkDir, vehicle_direction, isAsk);
            LogHelper.Log(logger: logger, LogLevel: NLog.LogLevel.Info, Class: nameof(ReserveBLL), Device: "AGV",
               Data: $"vh:{vhID} Try add reserve section:{sectionID} dir:{sensorDir},result:{result}",
               VehicleID: vhID);
            onReserveStatusChange();

            return result;
        }

        private int getVehicleDirection(ProtocolFormat.OHTMessage.DriveDirction driveDirction)
        {
            switch (driveDirction)
            {
                case ProtocolFormat.OHTMessage.DriveDirction.DriveDirNone:
                    return 0;
                case ProtocolFormat.OHTMessage.DriveDirction.DriveDirForward:
                    return 1;
                case ProtocolFormat.OHTMessage.DriveDirction.DriveDirReverse:
                    return -1;
                default:
                    return 0;
            }
        }

        private (bool isEnhanceInfo, Data.VO.ReserveEnhanceInfo info) IsReserveEnhanceSection(string sectionID)
        {
            var ReserveEnhanceInfos = commObjCacheManager.getReserveEnhanceInfos();
            if (ReserveEnhanceInfos == null) return (false, null);
            var reserve_enhance_info = ReserveEnhanceInfos.
                              Where(info => info.EnhanceControlSections.Contains(sectionID)).
                              FirstOrDefault();
            return (reserve_enhance_info != null, reserve_enhance_info);
        }


        public HltResult RemoveAllReservedSectionsBySectionID(string sectionID)
        {
            //int sec_id = 0;
            //int.TryParse(sectionID, out sec_id);
            string sec_id = SCUtility.Trim(sectionID);
            HltResult result = mapAPI.RemoveAllReservedSectionsBySectionID(sec_id);
            onReserveStatusChange();
            return result;

        }

        public void RemoveAllReservedSectionsByVehicleID(string vhID)
        {
            mapAPI.RemoveAllReservedSectionsByVehicleID(vhID);
            onReserveStatusChange();
        }
        public void RemoveAllReservedSections()
        {
            mapAPI.RemoveAllReservedSections();
            onReserveStatusChange();
        }



        public bool IsR2000Address(string adrID)
        {
            var hlt_r2000_section_objs = mapAPI.HltMapSections.Where(sec => SCUtility.isMatche(sec.Type, HtlSectionType.R2000.ToString())).ToList();
            bool is_r2000_address = hlt_r2000_section_objs.Where(sec => SCUtility.isMatche(sec.StartAddressID, adrID) || SCUtility.isMatche(sec.EndAddressID, adrID))
                                                          .Count() > 0;
            return is_r2000_address;
        }
        public bool IsR2000Section(string sectionID)
        {
            var hlt_section_obj = mapAPI.HltMapSections.Where(sec => SCUtility.isMatche(sec.ID, sectionID)).FirstOrDefault();
            return SCUtility.isMatche(hlt_section_obj.Type, HtlSectionType.R2000.ToString());
        }

        enum HtlSectionType
        {
            Horizontal,
            Vertical,
            R2000
        }
    }

}