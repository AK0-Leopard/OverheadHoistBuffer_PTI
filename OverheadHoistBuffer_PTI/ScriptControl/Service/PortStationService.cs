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
            try
            {
                if (port != null && port.PORT_SERVICE_STATUS != status)
                {
                    portInfoLogger.Info($"Port {portID} service status changed, old status: {port.PORT_SERVICE_STATUS}, new status: {status}");
                    isSuccess = scApp.PortStationBLL.OperateDB.updateServiceStatus(port.PORT_ID, status);
                    switch (status)
                    {
                        case E_PORT_STATUS.InService:
                            break;
                        case E_PORT_STATUS.OutOfService:
                            break;
                        default:
                            break;
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

        public bool doUpdateEqPortRequestStatus(string change_port_id, E_EQREQUEST_STATUS type)
        {
            var port = scApp.PortStationBLL.OperateDB.get(change_port_id);
            bool is_success = false;
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
            public bool IsError => ErrorString.Equals("ON");
            public override string ToString()
            {
                return $"ENT_NAME:{PortId}," +
                    $"CONTROLMODE:{ControlModeString}," +
                    $"PORTSTATUS:{EqStatusString}" +
                    $"ERROR:{ErrorString}";
            }
        }
        //2022.7.21
        public void GetPortDataFromWebService(string targetUrl = null)
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
                    var portInfo = new MesPortInfo()
                    {
                        PortId = data["ENT_NAME"].ToString(),
                        ControlModeString = data["CONTROLMODE"].ToString(),
                        EqStatusString = data["PORTSTATUS"].ToString()
                    };
                    portInfoCsvLogger.Info(portInfo.ToString());
                    doUpdateEqPortRequestStatus(portInfo.PortId, portInfo.EqStatus);
                    doUpdatePortStationServiceStatus(portInfo.PortId, portInfo.ControlMode);
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
