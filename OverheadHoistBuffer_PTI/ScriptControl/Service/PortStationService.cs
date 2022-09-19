using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.BLL;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using com.mirle.ibg3k0.sc.Data.SECS.ASE;
using System.ServiceModel;
using System.Data;

namespace com.mirle.ibg3k0.sc.Service
{
    public class PortStationService
    {

        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        NLog.Logger portInfoLogger = NLog.LogManager.GetLogger("PortInfoLogger");
        NLog.Logger portInfoCsvLogger = NLog.LogManager.GetLogger("PortInfoCsvLogger");
        private SCApplication scApp = null;
        private ReportBLL reportBLL = null;
        private LineBLL lineBLL = null;
        private ALINE line = null;
        public PortStationService()
        {

        }
        public void start(SCApplication _app)
        {
            scApp = _app;
            reportBLL = _app.ReportBLL;
        }

        public bool doUpdatePortStationPriority(string portID, int priority)
        {
            bool isSuccess = true;
            string result = string.Empty;
            try
            {
                if (isSuccess)
                {
                    using (TransactionScope tx = SCUtility.getTransactionScope())
                    {
                        using (DBConnection_EF con = DBConnection_EF.GetUContext())
                        {
                            isSuccess = scApp.PortStationBLL.OperateDB.updatePriority(portID, priority);
                            if (isSuccess)
                            {
                                tx.Complete();
                                scApp.PortStationBLL.OperateCatch.updatePriority(portID, priority);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error(ex, "Execption:");
            }
            return isSuccess;
        }

        public bool doUpdatePortDefPriority(string portID, int priority)
        {
            bool isSuccess = true;
            string result = string.Empty;
            try
            {
                if (isSuccess)
                {
                    using (TransactionScope tx = SCUtility.getTransactionScope())
                    {
                        using (DBConnection_EF con = DBConnection_EF.GetUContext())
                        {
                            isSuccess = scApp.PortDefBLL.updatePriority(portID, priority);
                            if (isSuccess)
                            {
                                tx.Complete();
                                scApp.PortStationBLL.OperateCatch.updatePriority(portID, priority);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error(ex, "Execption:");
            }
            return isSuccess;
        }

        public bool doUpdatePortStatus(string portID, E_PORT_STATUS status)
        {
            bool isSuccess = true;
            string result = string.Empty;
            try
            {
                if (isSuccess)
                {
                    using (TransactionScope tx = SCUtility.getTransactionScope())
                    {
                        using (DBConnection_EF con = DBConnection_EF.GetUContext())
                        {
                            isSuccess = scApp.PortStationBLL.OperateDB.updatePortStatus(portID, status);
                            if (isSuccess)
                            {
                                tx.Complete();
                                scApp.PortStationBLL.OperateCatch.updatePortStatus(portID, status);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error(ex, "Execption:");
            }
            return isSuccess;
        }
        public bool doUpdatePortStationServiceStatus(string portID, E_PORT_STATUS status)
        {
            bool isSuccess = true;
            var port = scApp.PortStationBLL.OperateDB.get(portID);
            var portDef = scApp.PortDefBLL.GetPortDataByID(portID.Trim());
            try
            {
                if (port != null && port.PORT_SERVICE_STATUS != status)
                {
                    portInfoLogger.Info($"Port {portID} service status changed, old status: {port.PORT_SERVICE_STATUS}, new status: {status}");
                    isSuccess = scApp.PortStationBLL.OperateDB.updateServiceStatus(portID, status);
                    switch (status)
                    {
                        case E_PORT_STATUS.InService:
                            scApp.ReportBLL.ReportPortInService(portID, null);
                            break;
                        case E_PORT_STATUS.OutOfService:
                            scApp.ReportBLL.ReportPortOutOfService(portID, null);
                            break;
                        default:
                            break;
                    }
                }
                if (portDef != null && portDef.State != status)
                    scApp.PortDefBLL.UpdataPortService(portID, status);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error(ex, "Execption:");
            }
            return isSuccess;
        }

        public bool doUpdateEqPortRequestStatus(string change_port_id, E_EQREQUEST_STATUS type)
        {
            var port = scApp.PortStationBLL.OperateDB.get(change_port_id);
            var portDef = scApp.PortDefBLL.GetPortDataByID(change_port_id.Trim());
            bool is_success = false;
            try
            {
                if (port != null && port.PORT_TYPE != type)
                {
                    portInfoLogger.Info($"Port {change_port_id} status changed, old status: {port.PORT_TYPE}, new status: {type}");
                    is_success = scApp.PortStationBLL.OperateDB.updateEqPortRequestStatus(port, type);
                    switch (type)
                    {
                        case E_EQREQUEST_STATUS.LoadRequest:
                            scApp.ReportBLL.ReportLoadReq(change_port_id, null);
                            break;
                        case E_EQREQUEST_STATUS.UnloadRequest:
                            scApp.ReportBLL.ReportUnLoadReq(change_port_id, null);
                            break;
                        case E_EQREQUEST_STATUS.NoRequest:
                            scApp.ReportBLL.ReportNoReq(change_port_id, null);
                            break;
                    }
                }
                if (portDef != null && portDef.RequestStatus != type)
                    scApp.PortDefBLL.UpdateEqRequestStatus(change_port_id, type);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Execption:");
            }

            return is_success;
        }
        public bool doUpdateEqPortErrorStatus(string portId, bool isError)
        {
            var port = scApp.PortStationBLL.OperateDB.get(portId);
            var portDef = scApp.PortDefBLL.GetPortDataByID(portId.Trim());
            bool is_success = false;
            try
            {
                if (port != null && port.ERROR_FLAG != isError)
                {
                    portInfoLogger.Info($"Port {portId} error flag changed, old status: {port.PORT_TYPE}, new status: {isError}");
                    is_success = scApp.PortStationBLL.OperateDB.updateEqPortErrorStatus(port, isError);
                    if (isError)
                    {
                        //report unit alarm set
                        reportBLL.ReportUnitAlarmSet(portId, "100998", $"{portId} Error");
                    }
                    else
                    {
                        //report unit alarm clear
                        reportBLL.ReportUnitAlarmCleared(portId, "100998", $"{portId} Error");
                    }
                }

                if (portDef != null && portDef.ErrorFlag != isError)
                    scApp.PortDefBLL.UpdateEqErrorStatus(portId, isError);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Execption:");
            }

            return is_success;
        }
        public bool doUpdateEqIgnoreStatusFlag(string portId, bool isIgnore)
        {
            var port = scApp.PortStationBLL.OperateDB.get(portId);
            var portDef = scApp.PortDefBLL.GetPortDataByID(portId.Trim());
            bool is_success = false;
            try
            {
                if (port != null && port.IGNORE_STATUS_FLAG != isIgnore)
                    is_success = scApp.PortStationBLL.OperateDB.updateEqIgnoreStatusFlag(port, isIgnore);
                if (portDef != null && portDef.IgnoreStatusFlag != isIgnore)
                    is_success = scApp.PortDefBLL.UpdateEqIgnoreStatusFlag(portId, isIgnore);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Execption:");
            }

            return is_success;
        }

        #region Get port data from WebService
        class MesPortInfo
        {
            public string PortId { get; set; }
            public string ControlModeString { get; set; }
            public string EqStatusString { get; set; }
            public string ErrorString { get; set; }
            public E_PORT_STATUS ControlMode
            {
                get
                {
                    switch (ControlModeString)
                    {
                        case "ONLINE":
                            return E_PORT_STATUS.InService;
                        case "OFFLINE":
                            return E_PORT_STATUS.OutOfService;
                        default:
                            return E_PORT_STATUS.NoDefinition;
                    }
                }
            }
            public E_EQREQUEST_STATUS EqStatus
            {
                get
                {
                    switch (EqStatusString)
                    {
                        case "LDRQ":
                            return E_EQREQUEST_STATUS.LoadRequest;
                        case "UDRQ":
                            return E_EQREQUEST_STATUS.UnloadRequest;
                        default:
                            return E_EQREQUEST_STATUS.NoRequest;
                    }
                }
            }
            //public bool IsError => ErrorString != null && ErrorString.Equals("ON");
            public bool IsError => "ON".Equals(ErrorString);
            public override string ToString()
            {
                return $"ENT_NAME:{PortId}," +
                    $"CONTROLMODE:{ControlModeString}," +
                    $"PORTSTATUS:{EqStatusString}," +
                    $"ERROR:{ErrorString}";
            }
        }
        //2022.7.21
        public void GetPortDataFromWebService(string targetUrl = null, char splitChar = '-', char replaceFrom = 'P', char replaceTo = '0')
        {
            if (targetUrl is null)
                targetUrl = "http://localhost:44396/WLPService.asmx";

            try
            {
                BasicHttpBinding binding = new BasicHttpBinding();
                EndpointAddress endPoint = new EndpointAddress(targetUrl);
                portInfoLogger.Info($"Get port data, WebService url = {targetUrl}");
                var serv = new EqServiceReference.ExecuteWLPServiceSoapClient(binding, endPoint);
                DataTable dt = serv.Entity_QueryPortInfo("");

                foreach (DataRow data in dt.Rows)
                {
                    string portIdRaw = data["ENT_NAME"].ToString();
                    var portIdArr = portIdRaw.Split(splitChar);
                    string portId;
                    if (portIdArr.Count() < 1)
                        portId = portIdRaw;
                    else
                        portId = portIdArr[0] + portIdArr[1].Replace(replaceFrom, replaceTo);

                    var portInfo = new MesPortInfo()
                    {
                        PortId = portId,
                        ControlModeString = data["CONTROLMODE"].ToString(),
                        EqStatusString = data["PORTSTATUS"].ToString(),
                        ErrorString = data["ERROR"].ToString(),
                    };
                    portInfoCsvLogger.Info(portInfo.ToString());
                    doUpdateEqPortRequestStatus(portInfo.PortId, portInfo.EqStatus);
                    doUpdatePortStationServiceStatus(portInfo.PortId, portInfo.ControlMode);
                    doUpdateEqPortErrorStatus(portInfo.PortId, portInfo.IsError);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Execption:");
            }
        }
        #endregion Get port data from WebService
    }
}
