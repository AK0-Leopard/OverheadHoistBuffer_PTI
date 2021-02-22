//*********************************************************************************
//      BackgroundWorkBlockQueue.cs
//*********************************************************************************
// File Name: BackgroundWorkBlockQueue.cs
// Description: 背景執行通行權的實際作業
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2020/08/06    Mark Chou      N/A            Inital  
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Common.MPLC;
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.bcf.Data;
using com.mirle.ibg3k0.bcf.Schedule;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;

namespace com.mirle.ibg3k0.sc.Data
{
    public class BackgroundWorkBlockQueue : IBackgroundWork
    {
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public long getMaxBackgroundQueueCount()
        {
            return 1000;
        }

        public string getDriverName()
        {
            return this.GetType().Name;
        }

        public void doWork(string workKey, BackgroundWorkItem item)
        {
            try
            {
                using (TransactionScope tx = SCUtility.getTransactionScope())
                {
                    bool can_block_pass = true;
                    bool can_hid_pass = true;
                    bool isSuccess = false;
                    SCApplication scApp = SCApplication.getInstance();
                    //BCFApplication bcfApp, AVEHICLE eqpt, EventType eventType, int seqNum, string req_block_id, string req_hid_secid
                    //Node node = item.Param[0] as Node;
                    BCFApplication bcfApp = item.Param[0] as BCFApplication;
                    AVEHICLE eqpt = item.Param[1] as AVEHICLE;
                    EventType eventType = (EventType)item.Param[2];
                    int seqNum = (int)item.Param[3];
                    string req_block_id = item.Param[4] as string;
                    string req_hid_secid = item.Param[5] as string;
                    //can_block_pass = scApp.VehicleService.ProcessBlockReqNewNew(bcfApp, eqpt, req_block_id);
                    can_block_pass = scApp.VehicleService.ProcessBlockReqByReserveModule(bcfApp, eqpt, req_block_id);
                    isSuccess = scApp.VehicleService.replyTranEventReport(bcfApp, eventType, eqpt, seqNum, canBlockPass: can_block_pass, canHIDPass: can_hid_pass);
                    if (isSuccess)
                    {
                        tx.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }

    }
}
