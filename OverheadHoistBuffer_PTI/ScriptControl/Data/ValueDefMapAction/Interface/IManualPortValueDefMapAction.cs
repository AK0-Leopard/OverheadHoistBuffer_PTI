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
        DirectionType PortDirection { get; }
        bool IsWaitingForInputPermission { get; }
        event ManualPortEventHandler OnWaitIn;
        event ManualPortEventHandler OnWaitOut;
        event ManualPortEventHandler OnDirectionChanged;
        event ManualPortEventHandler OnInServiceChanged;
        event ManualPortEventHandler OnBcrReadDone;
        event ManualPortEventHandler OnCstRemoved;
        event ManualPortEventHandler OnLoadPresenceChanged;
        event ManualPortEventHandler OnAlarmHappen;
        event ManualPortEventHandler OnAlarmClear;
        event ManualPortEventHandler OnDoorOpen;
        event ManualPortEventHandler OnInputPermissionFlagChanged;

        Task MoveBackAsync();
        Task MoveBackAsync(MoveBackReasons reason);
        Task SetMoveBackReasonAsync(MoveBackReasons reason);
        Task ShowReadyToWaitOutCarrierOnMonitorAsync(string carrierId_1, string carrierId_2);
        Task ShowComingOutCarrierOnMonitorAsync(string carrierId);
        Task SetInputPermissionAsync(bool isSuccess);

        Task TimeCalibrationAsync();
    }
}
