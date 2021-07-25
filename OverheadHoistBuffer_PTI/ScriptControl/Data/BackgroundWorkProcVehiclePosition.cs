//*********************************************************************************
//      BackgroundPLCWorkProcessData.cs
//*********************************************************************************
// File Name: BackgroundPLCWorkProcessData.cs
// Description: 背景執行上報Process Data至MES的實際作業
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// <summary>
    /// Class BackgroundWorkSample.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Schedule.IBackgroundWork" />
    public class BackgroundWorkProcVehiclePosition : IBackgroundWork
    {
        /// <summary>
        /// The logger
        /// </summary>
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets the maximum background queue count.
        /// </summary>
        /// <returns>System.Int64.</returns>
        public long getMaxBackgroundQueueCount()
        {
            return 1000;
        }

        /// <summary>
        /// Gets the name of the driver.
        /// </summary>
        /// <returns>System.String.</returns>
        public string getDriverName()
        {
            return this.GetType().Name;
        }

        /// <summary>
        /// Does the work.
        /// </summary>
        /// <param name="workKey">The work key.</param>
        /// <param name="item">The item.</param>
        public void doWork(string workKey, BackgroundWorkItem item)
        {
            try
            {
                App.SCApplication scapp = item.Param[0] as App.SCApplication;
                AVEHICLE vh = item.Param[1] as AVEHICLE;
                ProtocolFormat.OHTMessage.ID_134_TRANS_EVENT_REP recive_str = item.Param[2] as ProtocolFormat.OHTMessage.ID_134_TRANS_EVENT_REP;

                //Do something.
                EventType eventType = recive_str.EventType;
                string current_adr_id = SCUtility.isEmpty(recive_str.CurrentAdrID) ? string.Empty : recive_str.CurrentAdrID;
                string current_sec_id = SCUtility.isEmpty(recive_str.CurrentSecID) ? string.Empty : recive_str.CurrentSecID;
                uint sec_dis = recive_str.SecDistance;
                if (sec_dis == 0 || SCUtility.isEmpty(current_sec_id))
                {
                    current_sec_id = "";

                }
                ASECTION current_sec = scapp.SectionBLL.cache.GetSection(current_sec_id);
                string current_seg_id = "";
                if (current_sec != null)
                {
                    current_seg_id = current_sec.SEG_NUM;
                }
                else
                {
                    current_sec = scapp.SectionBLL.cache.GetSectionsByToAddress(current_adr_id).FirstOrDefault();
                    current_seg_id = current_sec.SEG_NUM;
                }

                string last_adr_id = vh.CUR_ADR_ID;
                string last_sec_id = vh.CUR_SEC_ID;
                string last_seg_id = "";
                if (!vh.IsOnAdr)
                {
                    ASECTION lase_sec = scapp.SectionBLL.cache.GetSection(last_sec_id);
                    last_seg_id = lase_sec == null ? string.Empty : lase_sec.SEG_NUM;
                }
                else
                {
                    last_seg_id = vh.CUR_SEG_ID;
                }
                scapp.VehicleService.doUpdateVheiclePositionAndCmdSchedule(vh, current_adr_id, current_sec_id, current_seg_id, last_adr_id, last_sec_id, last_seg_id, sec_dis, eventType);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }
    }
}