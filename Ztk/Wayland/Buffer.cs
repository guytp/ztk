using System;

namespace Ztk.Wayland
{
    internal class Buffer : WaylandObject
    {
        #region Properties
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Stride { get; private set; }
        public IntPtr SharedMemoryPointer { get; private set; }
        #endregion

        public Buffer(IntPtr handle, int width, int height, int stride, IntPtr sharedMemoryPointer)
            : base(handle)
        {
            SharedMemoryPointer = sharedMemoryPointer;
            Handle = handle;
            Width = width;
            Height = height;
            Stride = stride;
        }

        protected override void ReleaseWaylandObject()
        {
        }
    }
}