using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ztk
{
    /// <summary>
    /// The Wayland client provides a managed wrapper around the Wayland client interface.
    /// </summary>
    public partial class App : IDisposable
    {
        #region Declarations
        /// <summary>
        /// Defines a handle to the Wayland display.
        /// </summary>
        private IntPtr _display;

        /// <summary>
        /// Defines a handle to the Wayland registry.
        /// </summary>
        private IntPtr _registry;

        private readonly WaylandWrapperInterop.RegistryAnnounceListener _announceListener;

        private readonly WaylandWrapperInterop.RegistryRemoveListener _removeListener;
        private readonly WaylandWrapperInterop.SharedMemoryFormatListener _sharedMemoryFormatListener;
        private readonly WaylandWrapperInterop.SeatCapabilitiesListener _seatCapabilitiesListener;
        private readonly WaylandWrapperInterop.SeatNameListener _seatNameListener;

        private readonly List<Window> _windows = new List<Window>();

        private Window _initialWindow;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the current instance of the application.
        /// </summary>
        public static App CurrentApplication { get; private set; }

        /// <summary>
        /// Gets the handle to the compositor.
        /// </summary>
        internal IntPtr Compositor { get; private set; }

        /// <summary>
        /// Gets the handle to the shell.
        /// </summary>
        internal IntPtr Shell { get; private set; }

        /// <summary>
        /// Gets a handle to the shared memory global object.
        /// </summary>
        internal IntPtr SharedMemory { get; private set; }

        /// <summary>
        /// Gets a handle to the seat global object.
        /// </summary>
        internal IntPtr Seat { get; private set; }

        /// <summary>
        /// Gets whether or not we have started yet.
        /// </summary>
        internal bool IsStarted { get; private set; }

        /// <summary>
        /// Gets the supported shared memory formats to this compositor.
        /// </summary>
        internal SharedMemoryFormat[] SharedMemoryFormats { get; private set; }

        internal List<Seat> Seats { get; private set; }
        #endregion

        #region Events
        /// <summary>
        /// Fired when the Startup routine has completed.
        /// </summary>
        public event EventHandler Started;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        public App(Window initialWindow = null)
        {
            // Ensure we only have a single instance
            if (CurrentApplication != null)
                throw new Exception("Only a single instance of App may exist within a process");

            // Setup app-wide delegates so that we do not lose references to them
            _removeListener = OnRegistryRemove;
            _announceListener = OnRegistryAnnounce;
            _sharedMemoryFormatListener = OnSharedMemoryFormat;
            _seatCapabilitiesListener = OnSeatCapabilities;
            _seatNameListener = OnSeatName;

            // Ensure arrays are non-empty
            SharedMemoryFormats = new SharedMemoryFormat[0];
            Seats = new List<Seat>();

            // Store initial window
            _initialWindow = initialWindow;

            // Assign this as the current instance
            CurrentApplication = this;
        }
        #endregion

        /// <summary>
        /// Perform a one-time startup of the application connecting to Wayland and getting access to any required resources.
        /// </summary>
        public void Startup()
        {
            // Ensure we have not already started up
            if (IsStarted)
                throw new Exception("Cannot start the application more than once");

            // Connect to the Wayland display
            if (_display == IntPtr.Zero) // Check for zero incase previous partial startup
            {
                _display = WaylandWrapperInterop.DisplayConnect();
                if (_display == IntPtr.Zero)
                    throw new Exception("Failed to get Wayland display");
            }

            // Now hookup to the registry
            if (_registry == IntPtr.Zero) // Check for zero incase previous partial startup
            {
                _registry = WaylandWrapperInterop.RegistryGet(_display);
                if (_registry == IntPtr.Zero)
                    throw new Exception("Failed to get Wayland registry");

                // Hookup to registry event handlers
                WaylandWrapperInterop.RegistryAddListeners(_registry, _announceListener, _removeListener);

                // Perform a single blocking call to get the registry data we need
                WaylandWrapperInterop.Dispatch(_display);
                WaylandWrapperInterop.Roundtrip(_display);
            }

            // Ensure our compositor and shell interfaces
            if (Compositor == IntPtr.Zero)
                throw new Exception("Unable to get a handle to compositor");
            if (Shell == IntPtr.Zero)
                throw new Exception("Unable to get a handle to shell");
            if (SharedMemory == IntPtr.Zero)
                throw new Exception("Unable to get a handle to shared memory");
            if (SharedMemoryFormats.Length < 1)
                throw new Exception("No supported shared memory formats with compositor");
            if (Seat == IntPtr.Zero)
                throw new Exception("Unable to get a handle to seat");

            // Mark us as started
            IsStarted = true;

            // If we had an initial window show it now and remove our handle to it
            if (_initialWindow != null)
            {
                _initialWindow.Show();
                _initialWindow = null;
            }

            // We are ready so trigger the Started event
            Started?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Perform a single spin of the event pump.
        /// </summary>
        public void Run()
        {
            // Startup if required
            if (!IsStarted)
                Startup();

            // Now spin as long as the application is alive
            while (WaylandWrapperInterop.Dispatch(_display))
            {
            }
        }

        internal void NotifyNewWindow(Window window)
        {
            if (_windows.Contains(window))
                return;
            _windows.Add(window);
        }

        /// <summary>
        /// Gets a window tied to a surface.
        /// </summary>
        /// <param name="surface">
        /// The handle to the Wayland pointer for the surface.
        /// </param>
        /// <returns>
        /// A window object if found, otherwise an exception is thrown.
        /// </returns>
        internal Window GetWindow(IntPtr surface)
        {
            Window window = _windows.FirstOrDefault(w => w.Surface == surface);
            if (window == null)
                throw new Exception("Unable to locate window for surface " + surface.ToHexString());
            return window;
        }

        #region Event Handlers
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
            if (interfaceName == "wl_compositor" && Compositor == IntPtr.Zero)
                Compositor = WaylandWrapperInterop.RegistryBindCompositor(registry, id);
            else if (interfaceName == "wl_shell" && Shell == IntPtr.Zero)
                Shell = WaylandWrapperInterop.RegistryBindShell(registry, id);
            else if (interfaceName == "wl_shm" && SharedMemory == IntPtr.Zero)
            {
                SharedMemory = WaylandWrapperInterop.RegistryBindSharedMemory(registry, id);
                WaylandWrapperInterop.SharedMemoryAddListeners(SharedMemory, _sharedMemoryFormatListener);
            }
            else if (interfaceName == "wl_seat" && Seat == IntPtr.Zero)
            {
                Seat = WaylandWrapperInterop.RegistryBindSeat(registry, id);
                WaylandWrapperInterop.SeatAddListeners(Seat, _seatCapabilitiesListener, _seatNameListener);
            }
        }
        
        private void OnSeatCapabilities(IntPtr data, IntPtr seatHandle, SeatCapability seatCapabilities)
        {
            // Get a handle to, or add, this seat
            Seat seat = Seats.FirstOrDefault(s => s.Handle == seatHandle);
            if (seat == null)
            {
                seat = new Seat(seatHandle);
                Seats.Add(seat);
            }

            seat.UpdateCapabilities(seatCapabilities);
        }

        private void OnSeatName(IntPtr data, IntPtr seatHandle, string name)
        {
            // Get a handle to, or add, this seat
            Seat seat = Seats.FirstOrDefault(s => s.Handle == seatHandle);
            if (seat == null)
            {
                seat = new Seat(seatHandle);
                Seats.Add(seat);
            }

            // Update the name
            seat.Name = name;
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

        /// <summary>
        /// Handle the registry removing something.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="registry"></param>
        /// <param name="id"></param>
        void OnRegistryRemove(IntPtr data, IntPtr registry, uint id)
        {
            // TODO: Support closing resources here
        }
        #endregion

        #region IDisposable Interface
        /// <summary>
        /// Free up our resources.
        /// </summary>
        public void Dispose()
        {
            if (_registry != IntPtr.Zero)
            {
                _registry = IntPtr.Zero;
            }
            if (_display != IntPtr.Zero)
            {
                WaylandWrapperInterop.DisplayDisconnect(_display);
                _display = IntPtr.Zero;
            }
            CurrentApplication = null;
        }
        #endregion
    }
}