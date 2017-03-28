using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Ztk
{
    internal static class WaylandWrapperInterop
    {
        private const string WAYLAND = "wayland-wrapper";

        [DllImport(WAYLAND, EntryPoint = "wlw_fixed_to_double")]
        public static extern double FixedToDouble(int fixedNumber);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CallbackListener(IntPtr data, IntPtr callback, uint callbackData);

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
        [DllImport(WAYLAND, EntryPoint = "wlw_compositor_create_surface")]
        public static extern IntPtr CompositorCreateSurface(IntPtr compositor);

        [DllImport(WAYLAND, EntryPoint = "wlw_shell_get_shell_surface")]
        public static extern IntPtr ShellGetShellSurface(IntPtr shell, IntPtr surface);

        [DllImport(WAYLAND, EntryPoint = "wlw_surface_attach")]
        public static extern void SurfaceAttach(IntPtr surface, IntPtr buffer);

        [DllImport(WAYLAND, EntryPoint = "wlw_surface_commit")]
        public static extern void SurfaceCommit(IntPtr surface);

        [DllImport(WAYLAND, EntryPoint = "wlw_shell_surface_set_toplevel")]
        public static extern void ShellSurfaceSetTopLevel(IntPtr shellSurface);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ShellSurfacePingListener(IntPtr data, IntPtr shellSurface, uint serial);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ShellSurfaceConfigureListener(IntPtr data, IntPtr shellSurface, uint edges, int width, int height);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ShellSurfacePopupDoneListener(IntPtr data, IntPtr shellSurface);

        [DllImport(WAYLAND, EntryPoint = "wlw_shell_surface_add_listener")]
        public static extern void ShellSurfaceAddListeners(IntPtr registry, ShellSurfacePingListener pingListener, ShellSurfaceConfigureListener configureListener, ShellSurfacePopupDoneListener popupDoneListener);

        [DllImport(WAYLAND, EntryPoint = "wlw_shell_surface_pong")]
        public static extern void ShellSurfacePong(IntPtr shellSurface, uint serial);

        [DllImport(WAYLAND, EntryPoint = "wlw_surface_damage")]
        public static extern void SurfaceDamage(IntPtr surface, int x, int y, int width, int height);

        [DllImport(WAYLAND, EntryPoint = "wlw_surface_frame_listener")]
        public static extern void SurfaceAddFrameListener(IntPtr surface, CallbackListener frameListener);
        #endregion


        #region Shared Memory
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SharedMemoryFormatListener(IntPtr data, IntPtr sharedMemory, SharedMemoryFormat format);

        [DllImport(WAYLAND, EntryPoint = "wlw_shm_add_listener")]
        public static extern void SharedMemoryAddListeners(IntPtr sharedMemory, SharedMemoryFormatListener formatListener);

        [DllImport(WAYLAND, EntryPoint = "wlw_fd_allocate")]
        public static extern int SharedMemoryFileDescriptorAllocate(int size);

        [DllImport(WAYLAND, EntryPoint = "wlw_shm_mmap")]
        public static extern IntPtr SharedMemoryMap(int size, int fileDescriptor);

        [DllImport(WAYLAND, EntryPoint = "wlw_shm_pool_create")]
        public static extern IntPtr SharedMemoryPoolCreate(IntPtr sharedMemory, int size, int fileDescriptor);

        [DllImport(WAYLAND, EntryPoint = "wlw_shm_pool_buffer_create")]
        public static extern IntPtr SharedMemoryPoolBufferCreate(IntPtr pool, int width, int height, int stride, SharedMemoryFormat sharedMemoryFormat);

        [DllImport(WAYLAND, EntryPoint = "wlw_shm_pool_destroy")]
        public static extern void SharedMemoryPoolDestroy(IntPtr pool);
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