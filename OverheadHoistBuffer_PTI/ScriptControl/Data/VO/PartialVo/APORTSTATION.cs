using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Data.ValueDefMapAction;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction.Interface;

namespace com.mirle.ibg3k0.sc
{
    public partial class APORTSTATION : BaseEQObject
    {
        public APORTSTATION()
        {
            eqptObjectCate = SCAppConstants.EQPT_OBJECT_PORT_STATION;
        }
        public string CST_ID { get; set; }
        public string EQPT_ID { get; set; }
        public override void doShareMemoryInit(BCFAppConstants.RUN_LEVEL runLevel)
        {
            foreach (IValueDefMapAction action in valueDefMapActionDic.Values)
            {
                action.doShareMemoryInit(runLevel);
            }
        }
        public override string ToString()
        {
            return $"{PORT_ID} ({ADR_ID})";
        }

        public virtual PortPLCInfo getPortPLCInfo()
        {
            var portValueDefMapAction = getICommonPortInfoValueDefMapAction();
            if (portValueDefMapAction == null) return null;
            return portValueDefMapAction.GetPortState() as PortPLCInfo;
        }

        protected virtual ICommonPortInfoValueDefMapAction getICommonPortInfoValueDefMapAction()
        {
            ICommonPortInfoValueDefMapAction portValueDefMapAction =
                getMapActionByIdentityKey(typeof(Data.ValueDefMapAction.PortValueDefMapAction).Name) as ICommonPortInfoValueDefMapAction;
            return portValueDefMapAction;
        }
        public void ChangeToInMode(bool isOn)
        {
            var portValueDefMapAction = getICommonPortInfoValueDefMapAction();
            if (portValueDefMapAction == null) return;
            portValueDefMapAction.ChangeToInModeAsync(isOn);
        }
        public void ChangeToOutMode(bool isOn)
        {
            var portValueDefMapAction = getICommonPortInfoValueDefMapAction();
            if (portValueDefMapAction == null) return;
            portValueDefMapAction.ChangeToOutModeAsync(isOn);
        }
        public void ResetAlarm()
        {
            var portValueDefMapAction = getICommonPortInfoValueDefMapAction();
            if (portValueDefMapAction == null) return;
            portValueDefMapAction.ResetAlarmAsync();
        }
        public void StopBuzzer()
        {
            var portValueDefMapAction = getICommonPortInfoValueDefMapAction();
            if (portValueDefMapAction == null) return;
            portValueDefMapAction.StopBuzzerAsync();
        }
        public void SetRun()
        {
            var portValueDefMapAction = getICommonPortInfoValueDefMapAction();
            if (portValueDefMapAction == null) return;
            portValueDefMapAction.SetRunAsync();
        }
        public void SetStop()
        {
            var portValueDefMapAction = getICommonPortInfoValueDefMapAction();
            if (portValueDefMapAction == null) return;
            portValueDefMapAction.SetStopAsync();
        }
        public void SetCommanding(bool isCommanding)
        {
            var portValueDefMapAction = getICommonPortInfoValueDefMapAction();
            if (portValueDefMapAction == null) return;
            portValueDefMapAction.SetCommandingAsync(isCommanding);
        }
        public void SetControllerErrorIndex(int index)
        {
            var portValueDefMapAction = getICommonPortInfoValueDefMapAction();
            if (portValueDefMapAction == null) return;
            portValueDefMapAction.SetControllerErrorIndexAsync(index);
        }
    }

}
