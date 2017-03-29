using System.Runtime.InteropServices;

namespace Ztk.Wayland
{
    internal class ShellSurface : WaylandObject
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ShellSurfacePingListener(IntPtr data, IntPtr shellSurface, uint serial);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ShellSurfaceConfigureListener(IntPtr data, IntPtr shellSurface, uint edges, int width, int height);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ShellSurfacePopupDoneListener(IntPtr data, IntPtr shellSurface);

        [DllImport("wayland-wrapper", EntryPoint = "wlw_shell_surface_set_toplevel")]
        public static extern void ShellSurfaceSetTopLevel(IntPtr shellSurface);


        [DllImport("wayland-wrapper", EntryPoint = "wlw_shell_surface_add_listener")]
        public static extern void ShellSurfaceAddListeners(IntPtr registry, ShellSurfacePingListener pingListener, ShellSurfaceConfigureListener configureListener, ShellSurfacePopupDoneListener popupDoneListener);

        [DllImport("wayland-wrapper", EntryPoint = "wlw_shell_surface_move")]
        private static extern void ShellSurfaceMove(IntPtr shellSurface, IntPtr seat, uint serial);

        private readonly ShellSurfacePingListener _surfacePingListener;
        private readonly ShellSurfaceConfigureListener _surfaceConfigureListener;
        private readonly ShellSurfacePopupDoneListener _surfacePopupDoneListener;

        public ShellSurface(IntPtr handle)
            : base(handle)
        {

            _surfacePingListener = OnShellSurfacePing;
            _surfaceConfigureListener = OnShellSurfaceConfigure;
            _surfacePopupDoneListener = OnShellSurfacePopupDone;
            ShellSurfaceSetTopLevel(handle);
            ShellSurfaceAddListeners(handle, _surfacePingListener, _surfaceConfigureListener, _surfacePopupDoneListener);
        }

        public void Move(PointerButtonEventArgs e)
        {
            ShellSurfaceMove(Handle, e.Seat.Handle, e.Pointer.Serial);
        }

        #region Event Handlers
        private void OnShellSurfacePopupDone(IntPtr data, IntPtr shellSurface)
        {
            Console.Write("Popup done for window");
        }

        private void OnShellSurfaceConfigure(IntPtr data, IntPtr shellSurface, uint edges, int width, int height)
        {
            Console.WriteLine("Window configure hint to " + edges + " / " + width + " / " + height);
        }

        private void OnShellSurfacePing(IntPtr data, IntPtr shellSurface, uint serial)
        {
            WaylandWrapperInterop.ShellSurfacePong(shellSurface, serial);
        }
        #endregion
    }
}