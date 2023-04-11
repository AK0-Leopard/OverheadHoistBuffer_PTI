using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Data.ValueDefMapAction;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Data.SECS;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.Data.VO.Interface;
using com.mirle.ibg3k0.sc.ObjectRelay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc
{
    public partial class ABLOCKZONEMASTER
    {
        public event EventHandler<string> VehicleLeave;
        public event EventHandler<string> VehicleEntry;

        public string CurrentRequestVhID { get; private set; }

        private void onSectinoLeave(string vh_id)
        {
            VehicleLeave?.Invoke(this, vh_id);
        }
        private void onSectinoEntry(string vh_id)
        {
            VehicleEntry?.Invoke(this, vh_id);
        }

        public void Leave(string vh_id)
        {
            onSectinoLeave(vh_id);
        }
        public void Entry(string vh_id)
        {
            onSectinoEntry(vh_id);
        }


        List<string> BlockZoneDetailSectionIDs;
        public void SetBlockDetailList(BLL.MapBLL mapBLL)
        {
            var sections = mapBLL.loadBlockZoneDetailSecIDsByEntrySecID(ENTRY_SEC_ID);
            sections = sections.Select(sec => sc.Common.SCUtility.Trim(sec)).ToList();
            BlockZoneDetailSectionIDs = sections;
        }
        public List<string> GetBlockZoneDetailSectionIDs()
        {
            if (BlockZoneDetailSectionIDs == null)
            {
                return new List<string>();
            }
            else
            {
                return BlockZoneDetailSectionIDs;
            }
        }

        object block_reserve_release_lock_obj = new object();
        public void BlockReserve(string reserveVhID)
        {
            lock (block_reserve_release_lock_obj)
            {
                CurrentRequestVhID = reserveVhID;
            }
        }
        public void BlockRelease(string releaseVhID)
        {
            lock (block_reserve_release_lock_obj)
            {
                if (sc.Common.SCUtility.isMatche(releaseVhID, CurrentRequestVhID))
                {
                    CurrentRequestVhID = "";
                }
                else
                {

                }
            }
        }
    }

}
