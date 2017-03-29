using System;
using System.Runtime.InteropServices;

namespace Ztk.Wayland
{
    internal class SharedMemoryPool : WaylandObject
    {
        [DllImport("wayland-wrapper", EntryPoint = "wlw_shm_pool_buffer_create")]
        public static extern IntPtr SharedMemoryPoolBufferCreate(IntPtr pool, int width, int height, int stride, SharedMemoryFormat sharedMemoryFormat);

        [DllImport("wayland-wrapper", EntryPoint = "wlw_shm_pool_destroy")]
        public static extern void SharedMemoryPoolDestroy(IntPtr pool);

        public SharedMemoryPool(IntPtr handle)
            : base(handle)
        {

        }

        protected override void ReleaseWaylandObject()
        {
            if (Handle != IntPtr.Zero)
            {
                SharedMemoryPoolDestroy(Handle);
                Handle = IntPtr.Zero;
            }
        }

        internal Buffer CreateBuffer(IntPtr sharedMemoryPointer, int width, int height, int stride, SharedMemoryFormat sharedMemoryFormat)
        {
            return new Buffer(SharedMemoryPoolBufferCreate(Handle, width, height, stride, sharedMemoryFormat), sharedMemoryPointer);
        }
    }
}