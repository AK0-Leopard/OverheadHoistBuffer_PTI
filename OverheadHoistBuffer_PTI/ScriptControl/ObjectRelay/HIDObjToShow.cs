using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.ObjectRelay
{
    public class HIDObjToShow
    {
        public static App.SCApplication app = App.SCApplication.getInstance();
        public HID HID { private set; get; } = null;
        private BLL.VehicleBLL VehicleBLL = null;
        private List<string> VhIDs = new List<string>();
        public HIDObjToShow(BLL.VehicleBLL vehicleBLL, HID hid)
        {
            HID = hid;
            VehicleBLL = vehicleBLL;
        }
        public string EQPT_ID { get { return HID.EQPT_ID; } }
        public string CurrentVh
        {
            get
            {
                var vhs = VehicleBLL.cache.loadVhsBySegmentIDs(HID.getSegments());
                if (vhs == null || vhs.Count == 0) return "";
                VhIDs = vhs.Select(v => v.VEHICLE_ID).ToList();
                return string.Join(",", VhIDs);
            }
        }
        public int Count
        {
            get
            {
                if (VhIDs == null || VhIDs.Count == 0) return 0;
                return VhIDs.Count;
            }
        }
    }
}
