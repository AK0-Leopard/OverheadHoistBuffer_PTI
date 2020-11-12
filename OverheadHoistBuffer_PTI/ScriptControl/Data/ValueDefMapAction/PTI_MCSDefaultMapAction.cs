//*********************************************************************************
//      PTI_MCSDefaultMapAction.cs
//*********************************************************************************
// File Name: PTI_MCSDefaultMapAction.cs
// Description: 與PTI_MCS (Apply)通訊細節
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 20201104      JasonWu        N/A            N/A     新增各需要SECS 格式
//**********************************************************************************
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.bcf.Data.ValueDefMapAction;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.BLL;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.SECS.PTI;
using com.mirle.ibg3k0.sc.Data.SECSDriver;
using com.mirle.ibg3k0.stc.Common;
using com.mirle.ibg3k0.stc.Data.SecsData;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.ValueDefMapAction
{
    public class PTI_MCSDefaultMapAction : IBSEMDriver, IValueDefMapAction
    {
        private ReportBLL reportBLL = null;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public virtual void doShareMemoryInit(BCFAppConstants.RUN_LEVEL runLevel)
        {
            try
            {
                switch (runLevel)
                {
                    case BCFAppConstants.RUN_LEVEL.ZERO:
                        SECSConst.setDicCEIDAndRPTID(scApp.CEIDBLL.loadDicCEIDAndRPTID());
                        SECSConst.setDicRPTIDAndVID(scApp.CEIDBLL.loadDicRPTIDAndVID());
                        break;
                    case BCFAppConstants.RUN_LEVEL.ONE:
                        break;
                    case BCFAppConstants.RUN_LEVEL.TWO:
                        break;
                    case BCFAppConstants.RUN_LEVEL.NINE:
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
            }
        }

        public virtual void doInit()
        {
            string eapSecsAgentName = scApp.EAPSecsAgentName;
            reportBLL = scApp.ReportBLL;
            ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S1F1", S1F1ReceiveAreYouThere); // virtual in GEM Driver
            ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S1F3", S1F3ReceiveSelectedEquipmentStatusRequest); // virtual in GEM Driver
            ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S1F13", S1F13ReceiveEstablishCommunicationRequest); // virtual in GEM Driver
            ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S1F17", S1F17ReceiveRequestOnLine);
            ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S2F15", S2F15ReceiveNewEquipmentConstantSend);
            ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S2F31", S2F31ReceiveDateTimeSetReq);
            ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S2F33", S2F33ReceiveDefineReport);
            ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S2F35", S2F35ReceiveLinkEventReport);
            ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S2F37", S2F37ReceiveEnableDisableEventReport);
            ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S2F41", S2F41ReceiveHostCommand);
            ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S2F49", S2F49ReceiveEnhancedRemoteCommandExtension);
            ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S5F3", S5F3ReceiveEnableDisableAlarm);
            // Not Sure Use or Not
            // ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S9F9", S9F9ReceiveTransactionTimerTimeout);
            // ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S10F3", S10F3ReceiveTerminalDisplaySingle);

            ISECSControl.addSECSConnectedHandler(bcfApp, eapSecsAgentName, secsConnected);
            ISECSControl.addSECSDisconnectedHandler(bcfApp, eapSecsAgentName, secsDisconnected);
        }

        public virtual void unRegisterEvent()
        {

        }

        public virtual void setContext(BaseEQObject baseEQ)
        {
            this.line = baseEQ as ALINE;
        }

        public virtual string getIdentityKey()
        {
            return this.GetType().Name;
        }
        protected override void S1F1ReceiveAreYouThere(object sender, SECSEventArgs e)
        {
            try
            {
                S1F1 s1f1 = ((S1F1)e.secsHandler.Parse<S1F1>(e));
                SCUtility.secsActionRecordMsg(scApp, true, s1f1);
                SCUtility.actionRecordMsg(scApp, s1f1.StreamFunction, line.Real_ID, "Receive Are You There From MES.", "");

                scApp.PTI_TransferService.PTI_TransferServiceLogger.Info(
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|s1f1:\n" + s1f1.toSECSString());

                //if (!isProcess(s1f1)) { return; }
                S1F2 s1f2 = new S1F2()
                {
                    SECSAgentName = scApp.EAPSecsAgentName,
                    SystemByte = s1f1.SystemByte,
                    MDLN = bcfApp.BC_ID,
                    SOFTREV = SCApplication.getMessageString("SYSTEM_VERSION")
                };
                SCUtility.secsActionRecordMsg(scApp, false, s1f2);
                TrxSECS.ReturnCode rtnCode = ISECSControl.replySECS(bcfApp, s1f2);
                SCUtility.actionRecordMsg(scApp, s1f1.StreamFunction, line.Real_ID,
                        "Reply Are You There To MES.", rtnCode.ToString());

                scApp.PTI_TransferService.PTI_TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "MCS >> OHB|s1f2 MDLN:" + s1f2.MDLN + "   SCES_ReturnCode:" + rtnCode);

                if (rtnCode != TrxSECS.ReturnCode.Normal)
                {
                    logger.Warn("Reply EAP S1F2 Error:{0}", rtnCode);
                }
            }
            catch (Exception ex)
            {
                scApp.PTI_TransferService.PTI_TransferServiceLogger.Error(
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "  S1F1ReceiveAreYouThere\n" + ex);

                logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}",
                    line.LINE_ID, "S1F1_Receive_AreYouThere", ex.ToString());
            }
        }
        
        protected void S2F15ReceiveNewEquipmentConstantSend(object sender, SECSEventArgs e)
        {
            try
            {
                S2F15 s2f15 = (S2F15)e.secsHandler.Parse<S2F15>(e);
                SCUtility.secsActionRecordMsg(scApp, true, s2f15);
                //if (isProcess(s2f15)) { return; }
                scApp.TransferService.TransferServiceLogger.Info(
                                DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|s2f15:\n" + s2f15.toSECSString());

                S2F16 s2f16 = null;
                s2f16 = new S2F16();
                s2f16.SystemByte = s2f15.SystemByte;
                s2f16.SECSAgentName = scApp.EAPSecsAgentName;
                s2f16.EAC = "0";

                TrxSECS.ReturnCode rtnCode = ISECSControl.replySECS(bcfApp, s2f16);
                SCUtility.secsActionRecordMsg(scApp, false, s2f16);

                scApp.TransferService.TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "MCS >> OHB|s2f16 EAC:" + s2f16.EAC + "   SCES_ReturnCode:" + rtnCode);

                if (rtnCode != TrxSECS.ReturnCode.Normal)
                {
                    logger.Warn("Reply EQPT S2F16 Error:{0}", rtnCode);
                }

                //scApp.CEIDBLL.DeleteRptInfoByBatch();

                //if (s2f33.RPTITEMS != null && s2f33.RPTITEMS.Length > 0)
                //    scApp.CEIDBLL.buildReportIDAndVid(s2f33.ToDictionary());



                //SECSConst.setDicRPTIDAndVID(scApp.CEIDBLL.loadDicRPTIDAndVID());

            }
            catch (Exception ex)
            {
                scApp.TransferService.TransferServiceLogger.Error(
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "  S2F15ReceiveNewEquipmentConstantSend\n" + ex);

                logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}", line.LINE_ID, "S2F17_Receive_Date_Time_Req", ex.ToString());
            }
        }
        public override bool S6F11SendPortInService(string port_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        public override bool S6F11SendLoadReq(string port_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }

        public override bool S6F11SendUnLoadReq(string port_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        public override bool S6F11SendNoReq(string port_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }

        public override bool S6F11SendPortTypeInput(string port_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }

        public override bool S6F11SendPortTypeOutput(string port_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }

        public override bool S6F11SendPortTypeChanging(string port_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }

        public override bool S6F11SendCarrierBoxIDRename(string cstID, string boxID, string cstLOC, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }

        public override bool S6F11SendEmptyBoxSupply(string ReqCount, string zoneName, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }

        public override bool S6F11SendEmptyBoxRecycling(string boxID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }

        public override bool S6F11SendQueryLotID(string cstID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        public override bool S6F11SendClearBoxMoveReq(string boxID, string portID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        public override bool S6F11SendCarrierTransferring(ACMD_MCS cmd, CassetteData cassette, string ohtName, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        public override bool S6F11SendAlarmCleared(ACMD_MCS CMD_MCS, ALARM ALARM, string unitid, string unitstate)
        {
            return true;
        }
        public override bool S6F11SendAlarmSet(ACMD_MCS CMD_MCS, ALARM ALARM, string unitid, string unitstate, string RecoveryOption)
        {
            return true;
        }

        public override AMCSREPORTQUEUE S6F11BulibMessage(string ceid, object Vids)
        {
            return null;
        }

        //public override bool S6F11SendCarrierInstalled(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendCarrierInstalled(string vhID, string carrierID, string transferPort, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendCarrierRemoved(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendCarrierRemoved(string vhID, string carrierID, string transferPort, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        public override bool S6F11SendControlStateLocal()
        {
            return true;

        }

        public override bool S6F11SendControlStateRemote()
        {
            return true;

        }

        public override bool S6F11SendEquiptmentOffLine()
        {
            return true;

        }

        public override bool S6F11SendTransferCompleted(ACMD_MCS cmd, CassetteData cassette, string result_code, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        public override bool S6F11SendTransferInitiated(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        public override bool S6F11SendTransferPaused(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        public override bool S6F11SendTransferResume(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        public override bool S6F11SendMessage(AMCSREPORTQUEUE queue)
        {
            return true;

        }

        public override bool S6F11SendCarrierRemovedCompleted(string cstid, string boxid, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendCarrierInstallCompleted(CassetteData cst, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendCarrierRemovedFromPort(CassetteData cst, string Handoff_Type, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendCarrierResumed(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendCarrierStored(CassetteData cst, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendCarrierStoredAlt(ACMD_MCS cmd, CassetteData cassette, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendCarrierWaitIn(CassetteData cst, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendCarrierWaitOut(CassetteData cst, string portType, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendUnitAlarmSet(string unitID, string alarmID, string alarmTest, List<AMCSREPORTQUEUE> reportQueues = null)

        {
            return true;

        }
        public override bool S6F11SendUnitAlarmCleared(string unitID, string alarmID, string alarmTest, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendCraneActive(string cmdID, string craneID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendCraneIdle(string craneID, string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendCraneInEscape(string craneID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendCraneOutEscape(string craneID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendCraneOutServce(string craneID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendCraneInServce(string craneID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendCarrierIDRead(CassetteData cst, string IDreadStatus, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendZoneCapacityChange(string loc, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendOperatorInitiatedAction(string cmd_id, string cmd_type, List<AMCSREPORTQUEUE> reportQueues = null)

        {
            return true;

        }

        public override bool S6F11SendPortOutOfService(string port_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendShelfStatusChange(ZoneDef zone, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        //public override bool S6F11SendTransferAbortCompleted(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendTransferAbortFailed(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendTransferAbortInitiated(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendTransferCancelCompleted(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendTransferCancelInitial(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}
        public override bool S6F11SendTransferAbortCompleted(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        public override bool S6F11SendTransferAbortFailed(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        public override bool S6F11SendTransferAbortInitiated(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        public override bool S6F11SendTransferCancelCompleted(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        public override bool S6F11SendTransferCancelFailed(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        public override bool S6F11SendTransferCancelInitial(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }

        //public override bool S6F11SendTransferring(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        public override bool S6F11SendTSCAutoCompleted()
        {
            return true;
        }

        public override bool S6F11SendTSCAutoInitiated()
        {
            return true;
        }

        public override bool S6F11SendTSCPauseCompleted()
        {
            return true;
        }

        public override bool S6F11SendTSCPaused()
        {
            return true;
        }

        public override bool S6F11SenSCPauseInitiated()
        {
            return true;
        }

        //public override bool S6F11SendVehicleAcquireCompleted(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendVehicleAcquireStarted(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendVehicleArrived(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendVehicleAssigned(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendVehicleDeparted(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendVehicleDepositCompleted(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendVehicleDepositStarted(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendVehicleInstalled(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendVehicleRemoved(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendVehicleUnassinged(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}




        protected override void S2F41ReceiveHostCommand(object sender, SECSEventArgs e)
        {
        }

        protected override void S2F49ReceiveEnhancedRemoteCommandExtension(object sender, SECSEventArgs e)
        {
        }
    }
}
