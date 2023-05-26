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
    public class HIDHeartbeatCheckTimerAction : ITimerAction
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// The sc application
        /// </summary>
        private SCApplication scApp = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="HIDHeartbeatCheckTimerAction"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="intervalMilliSec">The interval milli sec.</param>
        public HIDHeartbeatCheckTimerAction(string name, long intervalMilliSec) : base(name, intervalMilliSec)
        {
        }

        /// <summary>
        /// Timer Action的執行動作
        /// </summary>
        /// <param name="obj">The object.</param>
        public override void doProcess(object obj)
        {
            try
            {
                var hids = scApp.EquipmentBLL.cache.loadHID();
                foreach (var hid in hids)
                {
                    hid?.CheckHeartbeatTimedOut(scApp.HIDHeartbeatLostThreshold);
                    //if (!hid.IsHeartbeatLoss)
                    //    hid.SendHeartbeatCommand();
                    hid?.SendHeartbeatCommand();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// Initializes the start.
        /// </summary>
        public override void initStart()
        {
            scApp = SCApplication.getInstance();
        }
    }
}
