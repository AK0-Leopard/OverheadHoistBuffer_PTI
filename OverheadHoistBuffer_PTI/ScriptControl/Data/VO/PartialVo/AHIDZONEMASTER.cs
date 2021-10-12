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
    public partial class AHIDZONEMASTER
    {


        public (bool isEnough, int currentVhCount) IsEnough(BLL.EquipmentBLL equipmentBLL, BLL.VehicleBLL vehicleBLL)
        {
            var hid = equipmentBLL.cache.getHID(LEAVE_ADR_ID_1); //暫時先用該欄位作為該EntrySection綁定哪個HID
            if (hid == null)
            {
                return (true, 0);
            }
            var vhs = vehicleBLL.cache.loadVhs();
            int in_hid_zone_vh_count = vhs.Where(v => hid.getSegments().Contains(v.CUR_SEG_ID)).Count();
            if (in_hid_zone_vh_count >= MAX_LOAD_COUNT)
            {
                return (false, in_hid_zone_vh_count);
            }
            return (true, in_hid_zone_vh_count);
        }

    }

}
