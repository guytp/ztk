using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Ztk.Wayland
{
    internal class Registry : WaylandObject
    {
        [DllImport("wayland-wrapper", EntryPoint = "wlw_get_registry")]
        private static extern IntPtr RegistryGet(IntPtr display);

        [DllImport("wayland-wrapper", EntryPoint = "wlw_registry_bind_compositor")]
        private static extern IntPtr RegistryBindCompositor(IntPtr registry, uint id);

        [DllImport("wayland-wrapper", EntryPoint = "wlw_registry_bind_shell")]
        private static extern IntPtr RegistryBindShell(IntPtr registry, uint id);

        [DllImport("wayland-wrapper", EntryPoint = "wlw_registry_bind_shm")]
        private static extern IntPtr RegistryBindSharedMemory(IntPtr registry, uint id);

        [DllImport("wayland-wrapper", EntryPoint = "wlw_registry_bind_seat")]
        private static extern IntPtr RegistryBindSeat(IntPtr registry, uint id);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private delegate void RegistryAnnounceListener(IntPtr data, IntPtr registry, uint id, string interfaceName, uint version);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void RegistryRemoveListener(IntPtr data, IntPtr registry, uint id);

        [DllImport("wayland-wrapper", EntryPoint = "wlw_registry_add_listener")]
        private static extern void RegistryAddListeners(IntPtr registry, RegistryAnnounceListener announceListener, RegistryRemoveListener removeListener);

        private readonly RegistryAnnounceListener _announceListener;

        private readonly RegistryRemoveListener _removeListener;

        #region Properties
        /// <summary>
        /// Gets the handle to the compositor.
        /// </summary>
        public Compositor Compositor { get; private set; }

        /// <summary>
        /// Gets the handle to the shell.
        /// </summary>
        public Shell Shell { get; private set; }

        /// <summary>
        /// Gets a handle to the shared memory global object.
        /// </summary>
        public SharedMemory SharedMemory { get; private set; }

        /// <summary>
        /// Gets a handle to the seat global object.
        /// </summary>
        public Seat Seat { get; private set; }
        #endregion


        public static Registry Get(Display display)
        {
            Registry registry = new Registry(RegistryGet(display.Handle));
            display.DispatchAndRoundtrip();
            return registry;
        }

        private Registry(IntPtr handle)
            : base(handle)
        {
            _removeListener = OnRegistryRemove;
            _announceListener = OnRegistryAnnounce;
            RegistryAddListeners(handle, _announceListener, _removeListener);
        }

        internal void EnsureCoreGlobals()
        {
            if (Compositor == null)
                throw new Exception("Unable to get a handle to compositor");
            if (Shell == null)
                throw new Exception("Unable to get a handle to shell");
            if (SharedMemory == null)
                throw new Exception("Unable to get a handle to shared memory");
            if (!SharedMemory.SharedMemoryFormats.Contains(SharedMemoryFormat.ARGB8888))
                throw new Exception("No supported shared memory formats with compositor");
            if (Seat == null)
                throw new Exception("Unable to get a handle to seat");
        }

        /// <summary>
        /// Handle the registry announcing something.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="registry"></param>
        /// <param name="id"></param>
        /// <param name="interfaceName"></param>
        /// <param name="version"></param>
        void OnRegistryAnnounce(IntPtr data, IntPtr registry, uint id, string interfaceName, uint version)
        {
            if (interfaceName == "wl_compositor" && Compositor == null)
                Compositor = new Compositor(RegistryBindCompositor(registry, id));
            else if (interfaceName == "wl_shell" && Shell == null)
                Shell = new Shell(RegistryBindShell(registry, id));
            else if (interfaceName == "wl_shm" && SharedMemory == null)
                SharedMemory = new SharedMemory(RegistryBindSharedMemory(registry, id));
            else if (interfaceName == "wl_seat" && Seat == null)
                Seat = new Seat(RegistryBindSeat(registry, id));
        }

        /// <summary>
        /// Handle the registry removing something.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="registry"></param>
        /// <param name="id"></param>
        void OnRegistryRemove(IntPtr data, IntPtr registry, uint id)
        {
            if (Compositor != null && registry == Compositor.Handle)
            {
                Compositor.Dispose();
                Compositor = null;
            }
            if (SharedMemory != null && registry == SharedMemory.Handle)
            {
                SharedMemory.Dispose();
                SharedMemory = null;
            }
            if (Shell != null && registry == Shell.Handle)
            {
                Shell.Dispose();
                Shell = null;
            }
            if (Seat != null && registry == Seat.Handle)
            {
                Seat.Dispose();
                Seat = null;
            }
        }

        protected override void ReleaseWaylandObject()
        {
            if (SharedMemory != null)
            {
                SharedMemory.Dispose();
                SharedMemory = null;
            }
            if (Shell != null)
            {
                Shell.Dispose();
                Shell = null;
            }
            if (Seat != null)
            {
                Seat.Dispose();
                Seat = null;
            }
            if (Compositor != null)
            {
                Compositor.Dispose();
                Compositor = null;
            }
        }
    }
}