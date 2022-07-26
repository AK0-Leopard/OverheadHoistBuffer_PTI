using com.mirle.ibg3k0.bcf.Data.TimerAction;
using com.mirle.ibg3k0.sc.App;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.TimerAction
{
    public class GetEqPortRequestTimerAction : ITimerAction
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        protected SCApplication scApp = null;

        public GetEqPortRequestTimerAction(string name, long intervalMilliSec) : base(name, intervalMilliSec)
        {
        }

        public override void doProcess(object obj)
        {
            try
            {
                scApp.PortStationService.GetPortDataFromWebService(scApp.WebServiceUrl);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }

        public override void initStart()
        {
            scApp = SCApplication.getInstance();
        }
    }
}
