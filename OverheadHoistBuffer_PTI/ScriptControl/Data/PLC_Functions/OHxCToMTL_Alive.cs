using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.PLC_Functions
{
    class OHxCToMTL_Alive : PLC_FunBase
    {
        [PLCElement(ValueName = "OHXC_TO_MTL_ALIVE_INDEX_PH2")]
        public uint Index;
    }
}
