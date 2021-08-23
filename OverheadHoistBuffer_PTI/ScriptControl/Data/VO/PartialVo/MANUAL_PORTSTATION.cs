using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using com.mirle.ibg3k0.sc.Data.PLC_Functions.MGV;
using com.mirle.ibg3k0.sc.Data.PLC_Functions.MGV.Enums;
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction;
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction.Interface;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc
{
    public partial class MANUAL_PORTSTATION : APORTSTATION
    {
        public MANUAL_PORTSTATION() : base()
        {

        }
        public IManualPortValueDefMapAction getExcuteMapAction()
        {
            IManualPortValueDefMapAction mapAction = this.getMapActionByIdentityKey(typeof(MGVDefaultValueDefMapAction).Name) as IManualPortValueDefMapAction;
            return mapAction;
        }

        public ManualPortPLCInfo getManualPortPLCInfo()
        {
            ICommonPortInfoValueDefMapAction portValueDefMapAction = getICommonPortInfoValueDefMapAction();
            if (portValueDefMapAction == null) return null;

            var manual_port_info = portValueDefMapAction.GetPortState() as ManualPortPLCInfo;
            return manual_port_info;
        }

        public override PortPLCInfo getPortPLCInfo()
        {
            ICommonPortInfoValueDefMapAction portValueDefMapAction = getICommonPortInfoValueDefMapAction();
            if (portValueDefMapAction == null) return null;

            var mgv_port_info = portValueDefMapAction.GetPortState() as ManualPortPLCInfo;

            return MgvPortInfoToPortInfo(mgv_port_info);
        }
        protected override ICommonPortInfoValueDefMapAction getICommonPortInfoValueDefMapAction()
        {
            ICommonPortInfoValueDefMapAction portValueDefMapAction =
                getMapActionByIdentityKey(typeof(MGVDefaultValueDefMapAction).Name) as ICommonPortInfoValueDefMapAction;
            return portValueDefMapAction;
        }

        private IManualPortValueDefMapAction getIManualPortValueDefMapAction()
        {
            IManualPortValueDefMapAction portValueDefMapAction =
                getMapActionByIdentityKey(typeof(MGVDefaultValueDefMapAction).Name) as IManualPortValueDefMapAction;
            return portValueDefMapAction;
        }

        private PortPLCInfo MgvPortInfoToPortInfo(ManualPortPLCInfo mgvPortInfo)
        {
            return new PortPLCInfo()
            {
                OpAutoMode = mgvPortInfo.IsRun,
                OpManualMode = mgvPortInfo.IsDown,
                OpError = mgvPortInfo.IsAlarm,
                IsInputMode = mgvPortInfo.IsInMode,
                IsOutputMode = mgvPortInfo.IsOutMode,
                IsModeChangable = mgvPortInfo.IsDirectionChangable,
                IsAGVMode = false,
                IsMGVMode = false,
                PortWaitIn = mgvPortInfo.IsWaitIn,
                PortWaitOut = mgvPortInfo.IsWaitOut,
                IsAutoMode = mgvPortInfo.RunEnable,
                IsReadyToLoad = mgvPortInfo.IsLoadOK,
                IsReadyToUnload = mgvPortInfo.IsUnloadOK,
                LoadPosition1 = mgvPortInfo.LoadPosition1,
                LoadPosition2 = mgvPortInfo.LoadPosition2,
                LoadPosition3 = mgvPortInfo.LoadPosition3,
                LoadPosition4 = mgvPortInfo.LoadPosition4,
                LoadPosition5 = mgvPortInfo.LoadPosition5,
                LoadPosition7 = false,
                LoadPosition6 = false,
                IsCSTPresence = false,
                AGVPortReady = false,
                CanOpenBox = false,
                IsBoxOpen = false,
                BCRReadDone = mgvPortInfo.IsBcrReadDone,
                CSTPresenceMismatch = false,
                IsTransferComplete = mgvPortInfo.IsTransferComplete,
                CstRemoveCheck = mgvPortInfo.IsRemoveCheck,
                //ErrorCode = mgvPortInfo.IsBcrReadDone,
                BoxID = mgvPortInfo.CarrierIdOfStage1,
                LoadPositionBOX1 = "",
                LoadPositionBOX2 = "",
                LoadPositionBOX3 = "",
                LoadPositionBOX4 = "",
                LoadPositionBOX5 = "",
                CassetteID = "",
                FireAlarm = false,
                cim_on = false,
                preLoadOK = false,
            };
        }

        #region Control Port
        public async Task MoveBackAsync()
        {
            var manualPortValueDefMapAction = getIManualPortValueDefMapAction();
            if (manualPortValueDefMapAction == null) return;
            await manualPortValueDefMapAction.MoveBackAsync();
        }
        public void SetMoveBackReasonAsync(MoveBackReasons moveBackReasons)
        {
            var manualPortValueDefMapAction = getIManualPortValueDefMapAction();
            if (manualPortValueDefMapAction == null) return;
            manualPortValueDefMapAction.SetMoveBackReasonAsync(moveBackReasons);
        }

        public void ShowReadyToWaitOutCarrierOnMonitorAsync(string carrierID1, string carrierID2)
        {
            var manualPortValueDefMapAction = getIManualPortValueDefMapAction();
            if (manualPortValueDefMapAction == null) return;
            manualPortValueDefMapAction.ShowReadyToWaitOutCarrierOnMonitorAsync(carrierID1, carrierID2);
        }

        public void ShowComingOutCarrierOnMonitorAsync(string carrierID)
        {
            var manualPortValueDefMapAction = getIManualPortValueDefMapAction();
            if (manualPortValueDefMapAction == null) return;
            manualPortValueDefMapAction.ShowComingOutCarrierOnMonitorAsync(carrierID);
        }

        public void TimeCalibrationAsync()
        {
            var manualPortValueDefMapAction = getIManualPortValueDefMapAction();
            if (manualPortValueDefMapAction == null) return;
            manualPortValueDefMapAction.TimeCalibrationAsync();
        }

        public void SetCommandingOnAsync()
        {
            var manualPortValueDefMapAction = getIManualPortValueDefMapAction();
            if (manualPortValueDefMapAction == null) return;
            manualPortValueDefMapAction.SetCommandingAsync(setOn: true);
        }

        public void SetCommandingOffAsync()
        {
            var manualPortValueDefMapAction = getIManualPortValueDefMapAction();
            if (manualPortValueDefMapAction == null) return;
            manualPortValueDefMapAction.SetCommandingAsync(setOn: false);
        }
        #endregion Control Port
    }
}
