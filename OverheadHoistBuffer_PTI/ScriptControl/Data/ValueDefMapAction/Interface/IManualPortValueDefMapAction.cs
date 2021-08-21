using com.mirle.ibg3k0.bcf.Data.ValueDefMapAction;
using com.mirle.ibg3k0.sc.Data.PLC_Functions.MGV;
using com.mirle.ibg3k0.sc.Data.PLC_Functions.MGV.Enums;
using System.Threading.Tasks;
using static com.mirle.ibg3k0.sc.Data.ValueDefMapAction.Events.ManualPortEvents;

namespace com.mirle.ibg3k0.sc.Data.ValueDefMapAction.Interface
{
    public interface IManualPortValueDefMapAction : ICommonPortInfoValueDefMapAction
    {
        string PortName { get; }
        event ManualPortEventHandler OnWaitIn;
        event ManualPortEventHandler OnWaitOut;
        event ManualPortEventHandler OnDirectionChanged;
        event ManualPortEventHandler OnInServiceChanged;
        event ManualPortEventHandler OnBcrReadDone;
        event ManualPortEventHandler OnCstRemoved;
        event ManualPortEventHandler OnLoadPresenceChanged;
        event ManualPortEventHandler OnAlarmHappen;
        event ManualPortEventHandler OnAlarmClear;
        Task MoveBackAsync();
        Task SetMoveBackReasonAsync(MoveBackReasons reason);
    }
}
