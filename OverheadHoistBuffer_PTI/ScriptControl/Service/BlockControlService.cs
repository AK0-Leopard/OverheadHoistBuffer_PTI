using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.BLL;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data;
using com.mirle.ibg3k0.sc.Data.SECS;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using com.mirle.iibg3k0.ttc.Common;
using Google.Protobuf;
using KingAOP;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace com.mirle.ibg3k0.sc.Service
{
    public class BlockControlService
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        MapBLL mapBLL = null;
        SectionBLL sectionBLL = null;
        ReserveBLL reserveBLL = null;
        BlockControlBLL blockControlBLL = null;

        VehicleService vehicleService = null;
        public BlockControlService()
        {

        }
        public void start(SCApplication app)
        {
            mapBLL = app.MapBLL;
            sectionBLL = app.SectionBLL;
            vehicleService = app.VehicleService;
            reserveBLL = app.ReserveBLL;
            blockControlBLL = app.BlockControlBLL;

            //RegisterReleaseAddressOfKeySection();
            RegisterBlockLeaveEventForBlockRelease();

        }


        private void RegisterBlockLeaveEventForBlockRelease()
        {
            List<ABLOCKZONEMASTER> block_zone_masters = blockControlBLL.cache.loadAllBlockZoneMaster();
            foreach (var block_zone in block_zone_masters)
            {
                block_zone.VehicleLeave += Block_zone_VehicleLeave;
            }
        }

        private void Block_zone_VehicleLeave(object sender, string vhID)
        {
            string vh_id = vhID;
            ABLOCKZONEMASTER block_master = sender as ABLOCKZONEMASTER;
            var block_detail = block_master.GetBlockZoneDetailSectionIDs();
            foreach (string block_detail_sec_id in block_detail)
            {
                string sec_id = SCUtility.Trim(block_detail_sec_id);
                reserveBLL.RemoveManyReservedSectionsByVIDSID(vh_id, sec_id);
            }
            block_master.BlockRelease(vh_id);
        }
        /// <summary>
        /// 用來註冊可能可以釋放Block的Section路段
        /// 當觸發的Leave Section，就會使用To Address來確認是否為某的Block的釋放點
        /// 當觸發的Entry Section，就會使用From Address來確認是否為某的Block的釋放點
        /// </summary>
        private void RegisterReleaseAddressOfKeySection()
        {
            List<ABLOCKZONEMASTER> block_zone_masters = mapBLL.loadAllBlockZoneMaster();
            List<ASECTION> from_adr_of_sections = new List<ASECTION>();
            List<ASECTION> to_adr_of_sections = new List<ASECTION>();
            foreach (var block_zone_master in block_zone_masters)
            {
                if (!SCUtility.isEmpty(block_zone_master.LEAVE_ADR_ID_1))
                {
                    from_adr_of_sections.AddRange(sectionBLL.cache.GetSectionsByFromAddress(block_zone_master.LEAVE_ADR_ID_1));
                    to_adr_of_sections.AddRange(sectionBLL.cache.GetSectionsByToAddress(block_zone_master.LEAVE_ADR_ID_1));
                }
                if (!SCUtility.isEmpty(block_zone_master.LEAVE_ADR_ID_2))
                {
                    from_adr_of_sections.AddRange(sectionBLL.cache.GetSectionsByFromAddress(block_zone_master.LEAVE_ADR_ID_2));
                    to_adr_of_sections.AddRange(sectionBLL.cache.GetSectionsByToAddress(block_zone_master.LEAVE_ADR_ID_2));
                }
            }
            from_adr_of_sections = from_adr_of_sections.Distinct().ToList();
        }
    }
}
