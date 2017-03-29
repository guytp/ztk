using System;

namespace Ztk.Wayland
{
    internal class Buffer : WaylandObject
    {
        #region Properties
        public IntPtr SharedMemoryPointer { get; private set; }
        #endregion

        public Buffer(IntPtr handle, IntPtr sharedMemoryPointer)
            : base(handle)
        {
            Handle = handle;
            SharedMemoryPointer = sharedMemoryPointer;
        }

        protected override void ReleaseWaylandObject()
        {
        }
    }
}