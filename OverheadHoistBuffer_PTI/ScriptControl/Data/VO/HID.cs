using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction;
using com.mirle.ibg3k0.sc.Data.VO.Interface;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.VO
{
    public class HID : AEQPT
    {
        List<string> Segments { get; set; } = new List<string>();
        public void setSegments(List<string> segIDs)
        {
            Segments = segIDs;
        }
        public List<string> getSegments()
        {
            return Segments.ToList();
        }

    }
}
