using System;
using System.Runtime.InteropServices;

namespace Ztk.Wayland
{
    internal class ShellSurface : Surface
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void ShellSurfacePingListener(IntPtr data, IntPtr shellSurface, uint serial);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void ShellSurfaceConfigureListener(IntPtr data, IntPtr shellSurface, uint edges, int width, int height);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void ShellSurfacePopupDoneListener(IntPtr data, IntPtr shellSurface);

        [DllImport("wayland-wrapper", EntryPoint = "wlw_shell_surface_set_toplevel")]
        private static extern void ShellSurfaceSetTopLevel(IntPtr shellSurface);


        [DllImport("wayland-wrapper", EntryPoint = "wlw_shell_surface_add_listener")]
        private static extern void ShellSurfaceAddListeners(IntPtr registry, ShellSurfacePingListener pingListener, ShellSurfaceConfigureListener configureListener, ShellSurfacePopupDoneListener popupDoneListener);

        [DllImport("wayland-wrapper", EntryPoint = "wlw_shell_surface_move")]
        private static extern void ShellSurfaceMove(IntPtr shellSurface, IntPtr seat, uint serial);


        [DllImport("wayland-wrapper", EntryPoint = "wlw_shell_surface_pong")]
        private static extern void ShellSurfacePong(IntPtr shellSurface, uint serial);

        private readonly ShellSurfacePingListener _surfacePingListener;
        private readonly ShellSurfaceConfigureListener _surfaceConfigureListener;
        private readonly ShellSurfacePopupDoneListener _surfacePopupDoneListener;

        protected IntPtr ShellSurfaceHandle { get; private set; }

        public ShellSurface(IntPtr handle, IntPtr surfaceHandle, SharedMemory sharedMemory)
            : base(surfaceHandle, sharedMemory)
        {
            ShellSurfaceHandle = handle;
            SurfaceType = SurfaceType.WaylandShell;
            _surfacePingListener = OnShellSurfacePing;
            _surfaceConfigureListener = OnShellSurfaceConfigure;
            _surfacePopupDoneListener = OnShellSurfacePopupDone;
            ShellSurfaceSetTopLevel(handle);
            ShellSurfaceAddListeners(handle, _surfacePingListener, _surfaceConfigureListener, _surfacePopupDoneListener);
        }

        public void Move(WaylandPointerButtonEventArgs e)
        {
            ShellSurfaceMove(ShellSurfaceHandle, e.Seat.Handle, e.Serial);
        }

        #region Event Handlers
        private void OnShellSurfacePopupDone(IntPtr data, IntPtr shellSurface)
        {
        }

        private void OnShellSurfaceConfigure(IntPtr data, IntPtr shellSurface, uint edges, int width, int height)
        {
        }

        private void OnShellSurfacePing(IntPtr data, IntPtr shellSurface, uint serial)
        {
            ShellSurfacePong(shellSurface, serial);
        }

        protected override void ReleaseWaylandObject()
        {
        }
        #endregion
    }
}