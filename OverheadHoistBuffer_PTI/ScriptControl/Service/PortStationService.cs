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
        public bool doUpdatePortStationServiceStatus(string portID, int status)
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
                            isSuccess = scApp.PortStationBLL.OperateDB.updateServiceStatus(portID, status);
                            if (isSuccess)
                            {
                                tx.Complete();
                                scApp.PortStationBLL.OperateCatch.updateServiceStatus(portID, status);
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

        public bool doUpdateEqPortRequestStatus(string change_port_id, E_EQREQUEST_STATUS type)
        {
            var port = scApp.PortStationBLL.OperateDB.get(change_port_id);
            bool is_success = true;
            if (port.PORT_TYPE != type)
            {
                scApp.PortStationBLL.OperateDB.updateEqPortRequestStatus(port, type);
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
        //2022.7.21
        public void GetPortDataFromWebService(string targetUrl = null)
        {
            if (targetUrl is null)
                targetUrl = "http://localhost:44396/WLPService.asmx";

            try
            {
                BasicHttpBinding binding = new BasicHttpBinding();
                EndpointAddress endPoint = new EndpointAddress(targetUrl);
                var serv = new EqServiceReference.ExecuteWLPServiceSoapClient(binding, endPoint);
                DataTable dt = serv.Entity_QueryPortInfo("");

                foreach (DataRow data in dt.Rows)
                {
                    string portId = data["ENT_NAME"].ToString();
                    string eqStatusString = data["PORTSTATUS"].ToString();
                    E_EQREQUEST_STATUS eqStatus;
                    switch (eqStatusString)
                    {
                        case "LDRQ":
                            eqStatus = E_EQREQUEST_STATUS.LoadRequest;
                            break;
                        case "UDRQ":
                            eqStatus = E_EQREQUEST_STATUS.UnloadRequest;
                            break;
                        default:
                            eqStatus = E_EQREQUEST_STATUS.NoRequest;
                            break;
                    }
                    doUpdateEqPortRequestStatus(portId, eqStatus);
                    //Console.WriteLine($"portId={portId}, eqStatus={eqStatus}");
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
