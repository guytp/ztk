using System;
using System.Runtime.InteropServices;

namespace Ztk.Wayland
{
    internal static class WaylandWrapperInterop
    {
        private const string WAYLAND = "wayland-wrapper";

        [DllImport(WAYLAND, EntryPoint = "wlw_fixed_to_double")]
        public static extern double FixedToDouble(int fixedNumber);

        #region Display
        [DllImport(WAYLAND, EntryPoint = "wlw_display_connect")]
        public static extern IntPtr DisplayConnect();

        [DllImport(WAYLAND, EntryPoint = "wlw_display_disconnect")]
        public static extern void DisplayDisconnect(IntPtr display);
        #endregion

        #region Registry
        [DllImport(WAYLAND, EntryPoint = "wlw_get_registry")]
        public static extern IntPtr RegistryGet(IntPtr display);

        [DllImport(WAYLAND, EntryPoint = "wlw_registry_bind_compositor")]
        public static extern IntPtr RegistryBindCompositor(IntPtr registry, uint id);

        [DllImport(WAYLAND, EntryPoint = "wlw_registry_bind_shell")]
        public static extern IntPtr RegistryBindShell(IntPtr registry, uint id);

        [DllImport(WAYLAND, EntryPoint = "wlw_registry_bind_shm")]
        public static extern IntPtr RegistryBindSharedMemory(IntPtr registry, uint id);

        [DllImport(WAYLAND, EntryPoint = "wlw_registry_bind_seat")]
        public static extern IntPtr RegistryBindSeat(IntPtr registry, uint id);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void RegistryAnnounceListener(IntPtr data, IntPtr registry, uint id, string interfaceName, uint version);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RegistryRemoveListener(IntPtr data, IntPtr registry, uint id);

        [DllImport(WAYLAND, EntryPoint = "wlw_registry_add_listener")]
        public static extern void RegistryAddListeners(IntPtr registry, RegistryAnnounceListener announceListener, RegistryRemoveListener removeListener);
        #endregion

        #region Surface

        [DllImport(WAYLAND, EntryPoint = "wlw_shell_get_shell_surface")]
        public static extern IntPtr ShellGetShellSurface(IntPtr shell, IntPtr surface);

        [DllImport(WAYLAND, EntryPoint = "wlw_shell_surface_pong")]
        public static extern void ShellSurfacePong(IntPtr shellSurface, uint serial);

        #endregion
        
        #region Event Loop
        [DllImport(WAYLAND, EntryPoint = "wlw_dispatch")]
        public static extern bool Dispatch(IntPtr display);

        [DllImport(WAYLAND, EntryPoint = "wlw_roundtrip")]
        public static extern void Roundtrip(IntPtr display);
        #endregion

        #region Seat
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SeatCapabilitiesListener(IntPtr data, IntPtr seatHandle, SeatCapability seatCapabilities);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void SeatNameListener(IntPtr data, IntPtr seatHandle, string name);


        [DllImport(WAYLAND, EntryPoint = "wlw_seat_get_pointer")]
        public static extern IntPtr SeatPointerGet(IntPtr seatHandle);


        [DllImport(WAYLAND, EntryPoint = "wlw_seat_add_listener")]
        internal static extern void SeatAddListeners(IntPtr seat, SeatCapabilitiesListener seatCapabilitiesListener, SeatNameListener seatNameListener);
        #endregion

        #region Pointers
        [DllImport(WAYLAND, EntryPoint = "wlw_pointer_destroy")]
        public static extern void PointerDestroy(IntPtr pointerHandle);
        #endregion
    }
}