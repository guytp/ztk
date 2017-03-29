using System;
using System.Runtime.InteropServices;

namespace Ztk.Wayland
{
    internal class Compositor : WaylandObject
    {

        [DllImport("wayland-wrapper", EntryPoint = "wlw_compositor_create_surface")]
        public static extern IntPtr CompositorCreateSurface(IntPtr compositor);

        public Compositor(IntPtr handle)
            : base(handle)
        {
        }

        protected override void ReleaseWaylandObject()
        {
        }

        public Surface CreateSurface(SharedMemory sharedMemory)
        {
            return new Surface(CompositorCreateSurface(Handle), sharedMemory);
        }
    }
}