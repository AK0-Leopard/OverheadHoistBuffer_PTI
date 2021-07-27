using com.mirle.AK0.ProtocolFormat;
using com.mirle.ibg3k0.sc.App;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.WebAPI
{
    public class VehicleControlFun : com.mirle.AK0.ProtocolFormat.VehicleControlFun.VehicleControlFunBase
    {
        #region RequestSegmentData
        public override Task<ReplySegmentData> RequestSegmentData(Empty request, ServerCallContext context)
        {
            var app = sc.App.SCApplication.getInstance();
            ReplySegmentData replySegmentData = new ReplySegmentData();
            var segments = app.SegmentBLL.cache.GetSegments();
            List<SegmentInfo> segmentInfos = new List<SegmentInfo>();
            foreach (var seg in segments)
            {
                var info = new SegmentInfo();
                info.ID = seg.SEG_NUM;
                info.Note = seg.NOTE;
                info.Status = converTo(seg.STATUS);
                var sec_ids = seg.Sections.Select(s => s.SEC_ID);
                info.SecIds.AddRange(sec_ids);
                segmentInfos.Add(info);
            }
            replySegmentData.SegmentInfos.AddRange(segmentInfos);
            return Task.FromResult(replySegmentData);
        }
        private SegmentStatus converTo(E_SEG_STATUS status)
        {
            switch (status)
            {
                case E_SEG_STATUS.Active:
                    return SegmentStatus.Active;
                case E_SEG_STATUS.Closed:
                    return SegmentStatus.Closed;
                case E_SEG_STATUS.Inactive:
                    return SegmentStatus.Inactive;
                default:
                    throw new Exception();
            }
        }
        #endregion RequestSegmentData
        #region RequestTranser
        public override Task<ReplyTrnsfer> RequestTrnsfer(VehicleCommandInfo request, ServerCallContext context)
        {
            var scApp = sc.App.SCApplication.getInstance();
            switch (request.Type)
            {
                case CommandEventType.Move:
                    return CommandMove(request, scApp);
                case CommandEventType.LoadUnload:
                case CommandEventType.Load:
                case CommandEventType.Unload:
                    return CommandLoadUnload(request, scApp);
                default:
                    return Task.FromResult(TransferResult(Result.Ng, $"No action"));
            }
        }

        private Task<ReplyTrnsfer> CommandMove(VehicleCommandInfo request, SCApplication scApp)
        {
            var assign_vh = scApp.VehicleBLL.cache.getVhByID(request.VhId);
            if (assign_vh == null)
            {
                return Task.FromResult(TransferResult(Result.Ng, $"vh id:{request.VhId} not exist"));
            }

            scApp.CMDBLL.doCreatTransferCommand(request.VhId, out var cmd_obj,
                                        cmd_type: converTo(request.Type),
                                        source: "",
                                        destination: "",
                                        box_id: request.CarrierId,
                                        destination_address: request.ToPortId,
                                        gen_cmd_type: SCAppConstants.GenOHxCCommandType.Manual);
            sc.BLL.CMDBLL.OHTCCommandCheckResult check_result_info =
                                sc.BLL.CMDBLL.getCallContext<sc.BLL.CMDBLL.OHTCCommandCheckResult>
                               (sc.BLL.CMDBLL.CALL_CONTEXT_KEY_WORD_OHTC_CMD_CHECK_RESULT);
            bool isSuccess = check_result_info.IsSuccess;
            string result = check_result_info.ToString();
            if (isSuccess)
            {
                isSuccess = scApp.VehicleService.doSendOHxCCmdToVh(assign_vh, cmd_obj);
                if (isSuccess)
                {
                    return Task.FromResult(TransferResult(Result.Ok, $"sned command to vh:{request.VhId} success"));
                }
                else
                {
                    return Task.FromResult(TransferResult(Result.Ok, $"Send command to vh:{request.VhId} failed!"));
                }
            }
            else
            {
                return Task.FromResult(TransferResult(Result.Ng, $"Creat command vh:{request.VhId} failed,reason:{result}"));
            }
        }
        private Task<ReplyTrnsfer> CommandLoadUnload(VehicleCommandInfo request, SCApplication scApp)
        {
            var assign_vh = scApp.VehicleBLL.cache.getVhByID(request.VhId);
            if (assign_vh == null)
            {
                return Task.FromResult(TransferResult(Result.Ng, $"vh id:{request.VhId} not exist"));
            }
            string hostsource_portid = request.FromPortId;
            string hostdest_portid = request.ToPortId;
            scApp.PortDefBLL.getAddressID(hostsource_portid, out string from_adr, out E_VH_TYPE vh_type);
            scApp.PortDefBLL.getAddressID(hostdest_portid, out string to_adr);

            string box_id = request.CarrierId;
            string lot_id = "lot_id";

            scApp.CMDBLL.doCreatTransferCommand(request.VhId, out var cmd_obj,
                                        cmd_type: converTo(request.Type),
                                        source: hostsource_portid,
                                        destination: hostdest_portid,
                                        box_id: box_id,
                                        lot_id: lot_id,
                                        source_address: from_adr,
                                        destination_address: to_adr,
                                        gen_cmd_type: SCAppConstants.GenOHxCCommandType.Manual);

            sc.BLL.CMDBLL.OHTCCommandCheckResult check_result_info =
                                sc.BLL.CMDBLL.getCallContext<sc.BLL.CMDBLL.OHTCCommandCheckResult>
                               (sc.BLL.CMDBLL.CALL_CONTEXT_KEY_WORD_OHTC_CMD_CHECK_RESULT);
            bool isSuccess = check_result_info.IsSuccess;
            string result = check_result_info.ToString();
            if (isSuccess)
            {
                isSuccess = scApp.VehicleService.doSendOHxCCmdToVh(assign_vh, cmd_obj);
                if (isSuccess)
                {
                    return Task.FromResult(TransferResult(Result.Ok, $"sned command to vh:{request.VhId} success"));
                }
                else
                {
                    return Task.FromResult(TransferResult(Result.Ok, $"Send command to vh:{request.VhId} failed!"));
                }
            }
            else
            {
                return Task.FromResult(TransferResult(Result.Ng, $"Creat command vh:{request.VhId} failed,reason:{result}"));
            }
        }

        private static ReplyTrnsfer TransferResult(Result result, string reason)
        {
            ReplyTrnsfer replyTrnsfer = new ReplyTrnsfer()
            {
                Result = result,
                Reason = reason
            };
            return replyTrnsfer;
        }

        private E_CMD_TYPE converTo(CommandEventType status)
        {
            switch (status)
            {
                case CommandEventType.Move:
                    return E_CMD_TYPE.Move;
                case CommandEventType.Load:
                    return E_CMD_TYPE.Load;
                case CommandEventType.Unload:
                    return E_CMD_TYPE.Unload;
                case CommandEventType.LoadUnload:
                    return E_CMD_TYPE.LoadUnload;
                default:
                    throw new Exception();
            }
        }

        #endregion RequestTranser
        #region GuideInfo Request
        public override Task<ReplyGuideInfo> RequestGuideInfo(SearchInfo request, ServerCallContext context)
        {
            var app = sc.App.SCApplication.getInstance();
            var guide_infos = app.GuideBLL.getGuideInfo(request.StartAdr, request.EndAdr);

            ReplyGuideInfo reply = new ReplyGuideInfo();
            reply.SecIds.AddRange(guide_infos.guideSectionIds);
            reply.AdrIds.AddRange(guide_infos.guideAddressIds);
            return Task.FromResult(reply);
        }
        private int intConvert(string s)
        {
            int.TryParse(s, out int i);
            return i;
        }
        #endregion GuideInfo Request
        #region VehicleInfo Request
        public override Task<ReplyVehicleSummary> RequestVehicleSummary(Empty request, ServerCallContext context)
        {
            var app = sc.App.SCApplication.getInstance();
            var vhs = app.VehicleBLL.cache.loadVhs();
            ReplyVehicleSummary replyVehicleSummary = new ReplyVehicleSummary();
            foreach (var vh in vhs)
            {
                replyVehicleSummary.VehiclesSummary.Add(new VehicleSummary() { VEHICLEID = vh.VEHICLE_ID });
            }
            return Task.FromResult(replyVehicleSummary);
        }
        #endregion VehicleInfo Request

        #region PortInfo Request
        public override Task<ReplyPortsInfo> RequestPortsInfo(Empty request, ServerCallContext context)
        {
            var app = sc.App.SCApplication.getInstance();
            var ports = app.PortDefBLL.cache.loadALLPortDefs();
            ReplyPortsInfo replyPortsInfo = new ReplyPortsInfo();
            foreach (var port in ports)
            {
                var port_info = new PortInfo();
                port_info.PortId = port.PLCPortID;
                var port_plc_data = app.TransferService.GetPLC_PortData(port.PLCPortID);
                if (port_plc_data != null)
                {
                    port_info.PlcData = convert2PortInfo(port.PLCPortID, port_plc_data);
                }
                replyPortsInfo.PortsInfo.Add(port_info);
            }
            return Task.FromResult(replyPortsInfo);
        }
        public PLCData convert2PortInfo(string portID, Data.PLC_Functions.PortPLCInfo portPLCInfo)
        {
            PLCData port_info = new PLCData();
            port_info.IsAutoMode = portPLCInfo.IsAutoMode;
            port_info.OpAutoMode = portPLCInfo.OpAutoMode;
            port_info.OpManualMode = portPLCInfo.OpManualMode;
            port_info.OpError = portPLCInfo.OpError;
            port_info.IsInputMode = portPLCInfo.IsInputMode;
            port_info.IsOutputMode = portPLCInfo.IsOutputMode;
            port_info.IsModeChangable = portPLCInfo.IsModeChangable;
            port_info.IsAGVMode = portPLCInfo.IsAGVMode;
            port_info.IsMGVMode = portPLCInfo.IsMGVMode;
            port_info.PortWaitIn = portPLCInfo.PortWaitIn;
            port_info.PortWaitOut = portPLCInfo.PortWaitOut;
            port_info.IsReadyToLoad = portPLCInfo.IsReadyToLoad;
            port_info.IsReadyToUnload = portPLCInfo.IsReadyToUnload;
            port_info.LoadPosition1 = portPLCInfo.LoadPosition1;
            port_info.LoadPosition2 = portPLCInfo.LoadPosition2;
            port_info.LoadPosition3 = portPLCInfo.LoadPosition3;
            port_info.LoadPosition4 = portPLCInfo.LoadPosition4;
            port_info.LoadPosition5 = portPLCInfo.LoadPosition5;
            port_info.LoadPosition7 = portPLCInfo.LoadPosition7;
            port_info.LoadPosition6 = portPLCInfo.LoadPosition6;
            port_info.IsCSTPresence = portPLCInfo.IsCSTPresence;
            port_info.AGVPortReady = portPLCInfo.AGVPortReady;
            port_info.CanOpenBox = portPLCInfo.CanOpenBox;
            port_info.IsBoxOpen = portPLCInfo.IsBoxOpen;
            port_info.BCRReadDone = portPLCInfo.BCRReadDone;
            port_info.CSTPresenceMismatch = portPLCInfo.CSTPresenceMismatch;
            port_info.IsTransferComplete = portPLCInfo.IsTransferComplete;
            port_info.CstRemoveCheck = portPLCInfo.CstRemoveCheck;
            port_info.ErrorCode = portPLCInfo.ErrorCode;
            port_info.BoxID = portPLCInfo.BoxID ?? "";
            port_info.CassetteID = portPLCInfo.CassetteID ?? "";
            port_info.PortID = portID ?? "";

            port_info.LoadPositionBOX1 = portPLCInfo.LoadPositionBOX1 ?? "";
            port_info.LoadPositionBOX2 = portPLCInfo.LoadPositionBOX2 ?? "";
            port_info.LoadPositionBOX3 = portPLCInfo.LoadPositionBOX3 ?? "";
            port_info.LoadPositionBOX4 = portPLCInfo.LoadPositionBOX4 ?? "";
            port_info.LoadPositionBOX5 = portPLCInfo.LoadPositionBOX5 ?? "";
            port_info.FireAlarm = portPLCInfo.FireAlarm;
            return port_info;
        }
        #endregion PortInfo Request
    }
}