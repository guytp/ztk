using System;

namespace Ztk
{
    public static class IntPtrExtensions
    {
        public static string ToHexString(this IntPtr pointer)
        {
            return string.Format("0x{0:x}", pointer.ToInt64());
        }
    }
}
