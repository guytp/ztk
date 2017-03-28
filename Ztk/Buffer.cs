using System;

namespace Ztk
{
    internal class Buffer
    {
        #region Declarations

        private IntPtr _pool;

        private int _fileDescriptor;
        #endregion

        #region Properties
        public IntPtr SharedMemoryPointer { get; private set; }

        public IntPtr BufferPointer { get; private set; }
        #endregion

        public Buffer(IntPtr sharedMemory, int width, int height, SharedMemoryFormat sharedMemoryFormat)
        {
            // Get a handle to our shared memory location
            // TODO: Stride dynamic based on format
            int stride = width * 4;
            int size = height * stride;
            _fileDescriptor = WaylandWrapperInterop.SharedMemoryFileDescriptorAllocate(size);
            if (_fileDescriptor < 0)
                throw new Exception("Error generating file descriptor for shared memory of size " + size);
            SharedMemoryPointer = WaylandWrapperInterop.SharedMemoryMap(size, _fileDescriptor);
            if (SharedMemoryPointer == IntPtr.Zero)
                throw new Exception("Failed to generate shared memory mapping for " + width + " x " + height + " / " + stride + " = " + size);

			// Now create a pool for this shared memory
            _pool = WaylandWrapperInterop.SharedMemoryPoolCreate(sharedMemory, size, _fileDescriptor);
            if (_pool == IntPtr.Zero)
                throw new Exception("Failed to generate pool for shared memory");

			// Finally generate our buffer from the pool
            BufferPointer = WaylandWrapperInterop.SharedMemoryPoolBufferCreate(_pool, width, height, stride, sharedMemoryFormat);
        }

		~Buffer()
        {
            // Remove our counterparts in the non-managed world
			if (_pool != IntPtr.Zero)
                WaylandWrapperInterop.SharedMemoryPoolDestroy(_pool);

			// TODO: Close buffer and FD as well
        }
    }
}