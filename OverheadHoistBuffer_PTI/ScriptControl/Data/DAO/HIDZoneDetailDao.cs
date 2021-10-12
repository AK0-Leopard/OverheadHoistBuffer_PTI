//*********************************************************************************
//      ZoneDao.cs
//*********************************************************************************
// File Name: ZoneDao.cs
// Description: Zone DAO
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2014/02/19    Hayes Chen     N/A            N/A     Initial Release
//
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Data;
using com.mirle.ibg3k0.sc.Data.VO;
using NLog;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    /// <summary>
    /// Class ZoneDao.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.DaoBase" />
    public class HIDZoneDetailDao : DaoBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Inserts the zone.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="zone">The zone.</param>
        public void insertZone(DBConnection_EF conn, AZONE zone)
        {
            try
            {
                conn.AZONE.Add(zone);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// Updates the zone.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="zone">The zone.</param>
        public void updateZone(DBConnection_EF conn, AZONE zone)
        {
            try
            {
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the zone by zone identifier.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="readLock">The read lock.</param>
        /// <param name="zone_id">The zone_id.</param>
        /// <returns>Zone.</returns>
        public AZONE getZoneByZoneID(DBConnection_EF conn, Boolean readLock, string zone_id)
        {
            AZONE rtnZone = null;
            try
            {
                var query = from zone in conn.AZONE
                            where zone.ZONE_ID == zone_id.Trim()
                            select zone;
                rtnZone = query.SingleOrDefault();

            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return rtnZone;
        }

        /// <summary>
        /// Loads the zone list by line identifier.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="line_id">The line_id.</param>
        /// <returns>List&lt;Zone&gt;.</returns>
        public List<string> loadHIDDetailSegmentIDByEntrySectionID(DBConnection_EF conn, string entrySecID)
        {
            List<string> rtnList = null;
            try
            {
                var query = from detail in conn.AHIDZONEDETAIL
                            where detail.ENTRY_SEC_ID.Trim() == entrySecID.Trim()
                            select detail.SEC_ID.Trim();
                rtnList = query.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return rtnList;
        }

    }
}
