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
    /// Class S9F5.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S9F5 : SXFY
    {
        /// <summary>
        /// The MHEAD
        /// </summary>
        [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_BINARY, Length = 1)]
        public string MHEAD;



        /// <summary>
        /// Initializes a new instance of the <see cref="S9F5"/> class.
        /// </summary>
        public S9F5()
        {
            StreamFunction = "S9F5";
            StreamFunctionName = "Unrecognized Function Type";
            W_Bit = 1;
        }
    }
}
