using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Ztk.Wayland
{
    internal class Compositor : WaylandObject
    {

        [DllImport("wayland-wrapper", EntryPoint = "wlw_compositor_create_surface")]
        private static extern IntPtr CompositorCreateSurface(IntPtr compositor);

        [DllImport("wayland-wrapper", EntryPoint = "wlw_shell_get_shell_surface")]
        private static extern IntPtr ShellGetShellSurface(IntPtr shell, IntPtr surface);

        public List<Surface> Surfaces { get; private set; }

        public Compositor(IntPtr handle)
            : base(handle)
        {
            Surfaces = new List<Surface>();
        }

        protected override void ReleaseWaylandObject()
        {
            foreach (Surface surface in Surfaces)
                surface.Dispose();
            Surfaces.Clear();
        }

        public ShellSurface CreateShellSurface(SharedMemory sharedMemory, Shell shell)
        {
            IntPtr surfaceHandle = CompositorCreateSurface(Handle);
            ShellSurface surface = new ShellSurface(ShellGetShellSurface(shell.Handle, surfaceHandle), surfaceHandle, sharedMemory);
            Surfaces.Add(surface);
            return surface;
        }

        public Surface SurfaceForHandle(IntPtr handle)
        {
            return Surfaces.FirstOrDefault(s => s.Handle == handle);
        }
    }
}