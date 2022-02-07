using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Data.ValueDefMapAction;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
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
    public partial class APARKZONEDETAIL
    {
        public bool HasVh(BLL.VehicleBLL vehicleBLL)
        {
            try
            {
                var vh = vehicleBLL.cache.getVhByAddressID(this.ADR_ID);
                if (vh == null) return false;
                if (vh.isAuto && vh.ACT_STATUS == ProtocolFormat.OHTMessage.VHActionStatus.NoCommand)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
