using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.bcf.Data.TimerAction;
using com.mirle.ibg3k0.sc.App;
using NLog;
using System;

namespace com.mirle.ibg3k0.sc.Data.TimerAction
{
    public class PortTimerAction : ITimerAction
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        protected SCApplication scApp = null;
        protected MPLCSMControl smControl;

        public PortTimerAction(string name, long intervalMilliSec) : base(name, intervalMilliSec)
        {
        }

        public override void initStart()
        {
            scApp = SCApplication.getInstance();
        }

        public override void doProcess(object obj)
        {
            try
            {
                scApp.ManualPortControlService?.RefreshState();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }
    }
}
