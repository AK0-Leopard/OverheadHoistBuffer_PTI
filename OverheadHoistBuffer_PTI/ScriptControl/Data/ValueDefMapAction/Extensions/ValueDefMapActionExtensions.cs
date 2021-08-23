using System;

namespace com.mirle.ibg3k0.sc.Data.ValueDefMapAction.Extension
{

    public static class ValueDefMapActionExtensions
    {
        public static UInt16 ToBCD(this int value)
        {
            if (value < 0 || value > 99)
                return 0;

            var bcd = (UInt16)(value / 10 * 16 + value % 10);
            return bcd;
        }
    }
}
