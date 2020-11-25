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
    /// Class S9F9.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S9F9 : SXFY
    {
        /// <summary>
        /// The MHEAD
        /// </summary>
        [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_BINARY, Length = 1)]
        public string SHEAD;



        /// <summary>
        /// Initializes a new instance of the <see cref="S9F9"/> class.
        /// </summary>
        public S9F9()
        {
            StreamFunction = "S9F9";
            StreamFunctionName = "Transaction Timer Timeout";
            W_Bit = 1;
        }
    }
}
