using System;

namespace Ztk
{
    [Flags]
    public enum SeatCapability : uint
    {
        Pointer = 1,
        Keyboard = 2,
        Touch = 4
    }
}