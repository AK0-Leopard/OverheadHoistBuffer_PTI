using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.bcf.Data;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Data.VO;
using NLog;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class MTLMTSInfoDao : DaoBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Loads the alarm maps by eq real identifier.
        /// </summary>
        /// <param name="eqpt_real_id">The eqpt_real_id.</param>
        /// <returns>List&lt;AlarmMap&gt;.</returns>

        public MTLSetting getMTLInfo(SCApplication app, string mtlID)
        {
            try
            {
                DataTable dt = app.OHxCConfig.Tables["MTLINFO"];
                var query = from c in dt.AsEnumerable()
                            where c.Field<string>("ID").Trim() == mtlID.Trim()
                            select new MTLSetting
                            {
                                ID = c.Field<string>("ID"),
                                MTLSegment = c.Field<string>("SEGMENT"),
                                MTLAddress = c.Field<string>("ADDRESS"),
                                CarInBufferAddress = c.Field<string>("CAR_IN_BUFFER_ADDRESS"),
                                SystemInAddress = c.Field<string>("SYSTEM_IN_ADDRESS"),
                                SystemOutAddress = c.Field<string>("SYSTEM_OUT_ADDRESS")
                            };
                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public MTSSetting getMTSInfo(SCApplication app, string mtsID)
        {
            try
            {
                DataTable dt = app.OHxCConfig.Tables["MTSINFO"];
                var query = from c in dt.AsEnumerable()
                            where c.Field<string>("ID").Trim() == mtsID.Trim()
                            select new MTSSetting
                            {
                                ID = c.Field<string>("ID"),
                                MTSSegment = c.Field<string>("SEGMENT"),
                                MTSAddress = c.Field<string>("ADDRESS"),
                                SystemInAddress = c.Field<string>("SYSTEM_IN_ADDRESS"),
                            };
                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
    }
}
