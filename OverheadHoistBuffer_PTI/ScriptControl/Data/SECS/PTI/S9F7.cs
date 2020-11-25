using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;
using com.mirle.ibg3k0.stc.Data.SecsData;

namespace com.mirle.ibg3k0.sc.Data.SECS.PTI
{
    /// <summary>
    /// Class S9F7.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S9F7 : SXFY
    {
        /// <summary>
        /// The MHEAD
        /// </summary>
        [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_BINARY, Length = 1)]
        public string MHEAD;



        /// <summary>
        /// Initializes a new instance of the <see cref="S9F7"/> class.
        /// </summary>
        public S9F7()
        {
            StreamFunction = "S9F7";
            StreamFunctionName = "Illegal Data";
            W_Bit = 1;
        }
    }
}
