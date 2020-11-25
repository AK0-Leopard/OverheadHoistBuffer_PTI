using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;
using com.mirle.ibg3k0.stc.Data.SecsData;

namespace com.mirle.ibg3k0.sc.Data.SECS.PTI
{
    public class S10F4 : SXFY
    {
        /// <summary>
        /// Acknowledge Code
        /// </summary>
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_BINARY, Length = 1)]
        public string ACKC10;
        public S10F4()
        {
            StreamFunction = "S10F4";
            W_Bit = 1;
            IsBaseType = true;
        }

    }
}
