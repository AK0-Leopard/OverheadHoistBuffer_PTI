using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Data.ValueDefMapAction;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction.Interface;

namespace com.mirle.ibg3k0.sc
{
    public partial class APORTSTATION : BaseEQObject
    {
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
            ICommonPortInfoValueDefMapAction portValueDefMapAction =
                getMapActionByIdentityKey(typeof(Data.ValueDefMapAction.PortValueDefMapAction).Name) as ICommonPortInfoValueDefMapAction;
            if (portValueDefMapAction == null) return null;
            return portValueDefMapAction.GetPortState() as PortPLCInfo;
        }
    }

}
