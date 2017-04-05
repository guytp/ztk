using System;
using System.Runtime.InteropServices;

namespace Ztk.Wayland
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct WaylandArray
    {
        public uint Size;
        public uint Alloc;
        public IntPtr Data;
    }
}