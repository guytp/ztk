using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Ztk.Wayland
{
    internal class SharedMemory : WaylandObject
    {
        [DllImport("wayland-wrapper", EntryPoint = "wlw_fd_allocate")]
        private static extern int SharedMemoryFileDescriptorAllocate(int size);


        [DllImport("wayland-wrapper", EntryPoint = "wlw_shm_mmap")]
        private static extern IntPtr SharedMemoryMap(int size, int fileDescriptor);


        [DllImport("wayland-wrapper", EntryPoint = "wlw_shm_pool_create")]
        private static extern IntPtr SharedMemoryPoolCreate(IntPtr sharedMemory, int size, int fileDescriptor);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SharedMemoryFormatListener(IntPtr data, IntPtr sharedMemory, SharedMemoryFormat format);

        [DllImport("wayland-wrapper", EntryPoint = "wlw_shm_add_listener")]
        private static extern void SharedMemoryAddListeners(IntPtr sharedMemory, SharedMemoryFormatListener formatListener);

        private readonly SharedMemoryFormatListener _sharedMemoryFormatListener;

        /// <summary>
        /// Gets the supported shared memory formats to this compositor.
        /// </summary>
        internal SharedMemoryFormat[] SharedMemoryFormats { get; private set; }

        public SharedMemory(IntPtr handle)
            : base(handle)
        {
            SharedMemoryFormats = new SharedMemoryFormat[0];
            _sharedMemoryFormatListener = OnSharedMemoryFormat;
            SharedMemoryAddListeners(handle, _sharedMemoryFormatListener);
        }

        protected override void ReleaseWaylandObject()
        {
            // TODO: Close FDs too
        }

        public Buffer CreateBuffer(int width, int height)
        {
            // Get a handle to our shared memory location
            SharedMemoryFormat sharedMemoryFormat = SharedMemoryFormat.ARGB8888;
            int stride = width * 4;
            int size = height * stride;
            int fileDescriptor = SharedMemoryFileDescriptorAllocate(size);
            if (fileDescriptor < 0)
                throw new Exception("Error generating file descriptor for shared memory of size " + size);
            IntPtr sharedMemoryPointer = SharedMemoryMap(size, fileDescriptor);
            if (sharedMemoryPointer == IntPtr.Zero)
                throw new Exception("Failed to generate shared memory mapping for " + width + " x " + height + " / " + stride + " = " + size);

            // Now create a pool for this shared memory
            SharedMemoryPool pool = new SharedMemoryPool(SharedMemoryPoolCreate(Handle, size, fileDescriptor));
            return pool.CreateBuffer(width, height, stride, sharedMemoryFormat, sharedMemoryPointer);
        }
        /// <summary>
        /// Handle a new shared memory format being detected from the compositor.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sharedMemory"></param>
        /// <param name="format"></param>
        private void OnSharedMemoryFormat(IntPtr data, IntPtr sharedMemory, SharedMemoryFormat format)
        {
            List<SharedMemoryFormat> sharedMemoryFormats = new List<SharedMemoryFormat>();
            if (SharedMemoryFormats != null)
                sharedMemoryFormats.AddRange(SharedMemoryFormats);
            if (!sharedMemoryFormats.Contains(format))
                sharedMemoryFormats.Add(format);
            SharedMemoryFormats = sharedMemoryFormats.ToArray();
        }

    }
}