using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using com.mirle.ibg3k0.sc.RouteKit;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.BLL
{
    public class GuideBLL
    {
        SCApplication scApp;
        Logger logger = LogManager.GetCurrentClassLogger();

        public HashSet<string> ErrorVehicleSections { get; private set; } = new HashSet<string>();

        public void start(SCApplication _scApp)
        {
            scApp = _scApp;
        }
        public (bool isSuccess, List<string> guideSegmentIds, List<string> guideSectionIds, List<string> guideAddressIds, int totalCost)
            getGuideInfo(string startAddress, string targetAddress, List<string> byPassSectionIDs = null)
        {
            if (SCUtility.isMatche(startAddress, targetAddress))
            {
                return (true, new List<string>(), new List<string>(), new List<string>(), 0);
            }

            bool is_success = false;
            int.TryParse(startAddress, out int i_start_address);
            int.TryParse(targetAddress, out int i_target_address);

            List<RouteInfo> stratFromRouteInfoList = null;
            //if (byPassSectionIDs == null || byPassSectionIDs.Count == 0)
            //{
            //    stratFromRouteInfoList = scApp.NewRouteGuide.getFromToRoutesAddrToAddr(i_start_address, i_target_address);
            //}
            //else
            //{
            //    stratFromRouteInfoList = scApp.NewRouteGuide.getFromToRoutesAddrToAddr(i_start_address, i_target_address, byPassSectionIDs);
            //}
            List<string> bypassSections = new List<string>(ErrorVehicleSections);
            if (byPassSectionIDs != null)
                bypassSections.AddRange(byPassSectionIDs);
            stratFromRouteInfoList = scApp.NewRouteGuide.getFromToRoutesAddrToAddr(i_start_address, i_target_address, bypassSections);

            RouteInfo min_stratFromRouteInfo = null;
            if (stratFromRouteInfoList != null && stratFromRouteInfoList.Count > 0)
            {
                min_stratFromRouteInfo = stratFromRouteInfoList.First();
                is_success = true;
            }

            return (is_success, null, min_stratFromRouteInfo.GetSectionIDs(), min_stratFromRouteInfo.GetAddressesIDs(), min_stratFromRouteInfo.total_cost);
        }

        public ASEGMENT OpenSegment(string strSegCode, ASEGMENT.DisableType disableType)
        {
            ASEGMENT seg_vo = null;
            ASEGMENT seg_do = null;
            try
            {
                seg_vo = scApp.SegmentBLL.cache.GetSegment(strSegCode);
                lock (seg_vo)
                {
                    seg_do = scApp.MapBLL.EnableSegment(strSegCode, disableType);
                    unbanRouteTwoDirect(strSegCode);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            return seg_do;
        }
        public ASEGMENT CloseSegment(string strSegCode, ASEGMENT.DisableType disableType)
        {
            ASEGMENT seg_vo = null;
            ASEGMENT seg_do = null;
            try
            {
                seg_vo = scApp.SegmentBLL.cache.GetSegment(strSegCode);
                lock (seg_vo)
                {
                    seg_do = scApp.MapBLL.DisableSegment(strSegCode, disableType);
                    banRouteTwoDirect(strSegCode);

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            return seg_do;
        }
        public ASEGMENT unbanRouteTwoDirect(string segmentID)
        {
            ASEGMENT segment_do = null;
            ASEGMENT segment_vo = scApp.SegmentBLL.cache.GetSegment(segmentID);
            if (segment_vo != null)
            {
                foreach (var sec in segment_vo.Sections)
                    scApp.NewRouteGuide.unbanRouteTwoDirect(sec.SEC_ID);
            }
            segment_do = scApp.MapBLL.EnableSegment(segmentID);
            return segment_do;
        }
        public ASEGMENT banRouteTwoDirect(string segmentID)
        {
            ASEGMENT segment = null;
            ASEGMENT segment_vo = scApp.SegmentBLL.cache.GetSegment(segmentID);
            if (segment_vo != null)
            {
                foreach (var sec in segment_vo.Sections)
                    scApp.NewRouteGuide.banRouteTwoDirect(sec.SEC_ID);
            }
            segment = scApp.MapBLL.DisableSegment(segmentID);
            return segment;
        }

        public (bool isSuccess, int distance) IsRoadWalkable(string startAddress, string targetAddress, List<string> byPassSectionIDs = null)
        {
            try
            {
                if (SCUtility.isMatche(startAddress, targetAddress))
                    return (true, 0);

                var guide_info = getGuideInfo(startAddress, targetAddress, byPassSectionIDs);
                //if ((guide_info.guideAddressIds != null && guide_info.guideAddressIds.Count != 0) &&
                //    ((guide_info.guideSectionIds != null && guide_info.guideSectionIds.Count != 0)))
                if (guide_info.isSuccess)
                {
                    return (true, guide_info.totalCost);
                }
                else
                {
                    return (false, int.MaxValue);
                }
            }
            catch
            {
                return (false, int.MaxValue);
            }
        }
        public int GetDistance(string startAddress, string targetAddress)
        {
            try
            {
                if (SCUtility.isMatche(startAddress, targetAddress))
                    return 0;

                var guide_info = getGuideInfo(startAddress, targetAddress);
                //if ((guide_info.guideAddressIds != null && guide_info.guideAddressIds.Count != 0) &&
                //    ((guide_info.guideSectionIds != null && guide_info.guideSectionIds.Count != 0)))
                if (guide_info.isSuccess)
                {
                    return guide_info.totalCost;
                }
                else
                {
                    return int.MaxValue;
                }
            }
            catch
            {
                return int.MaxValue;
            }
        }

    }

}

