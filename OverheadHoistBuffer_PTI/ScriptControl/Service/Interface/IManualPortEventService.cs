using com.mirle.ibg3k0.sc.BLL.Interface;
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction.Interface;
using System.Collections.Generic;

namespace com.mirle.ibg3k0.sc.Service.Interface
{
    public interface IManualPortEventService
    {
        void Start(IEnumerable<IManualPortValueDefMapAction> ports, IManualPortReportBLL reportBll, IManualPortDefBLL portDefBLL, IManualPortShelfDefBLL shelfDefBLL, IManualPortCassetteDataBLL cassetteDataBLL, IManualPortCMDBLL commandBLL, IManualPortAlarmBLL alarmBLL);
    }
}
