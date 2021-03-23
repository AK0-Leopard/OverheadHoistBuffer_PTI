using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data;
using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.BLL
{
    public class AddressBLL
    {
        SCApplication app = null;
        public DB OperateDB { private set; get; }
        public Catch OperateCatch { private set; get; }
        public AddressBLL()
        {
        }
        public void start(SCApplication _app)
        {
            app = _app;
            OperateDB = new DB();
            OperateCatch = new Catch(_app.getCommObjCacheManager());
        }
        public class DB
        {

        }
        public class Catch
        {
            CommObjCacheManager CacheManager;
            public Catch(CommObjCacheManager _cache_manager)
            {
                CacheManager = _cache_manager;
            }
            public List<string> loadCanAvoidAddressIDs()
            {
                var all_addresses = CacheManager.GetAddresses();
                if (all_addresses == null || all_addresses.Count == 0) return new List<string>();
                var address_ids = all_addresses.Where(adr => adr.IsAvoidAddress)
                                               .Select(adr => adr.ADR_ID)
                                               .ToList();
                return address_ids;
            }
        }
    }
}