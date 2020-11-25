using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;
using com.mirle.ibg3k0.stc.Data.SecsData;

namespace com.mirle.ibg3k0.sc.Data.SECS.PTI
{
    public class S10F3 : SXFY
    {
        /// <summary>
        /// Terminal ID for Terminal Display, 1-byte
        /// </summary>
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_BINARY, Length = 1)]
        public string TID;
        /// <summary>
        /// Text Terminal Display, 1-byte
        /// </summary>
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string TEXT;

        public S10F3()
        {
            StreamFunction = "S10F3";
            W_Bit = 1;
            IsBaseType = true;
        }

    }
}
