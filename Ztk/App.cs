using System;
using Ztk.Wayland;

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
        private Display _display;

        private Window _initialWindow;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the current instance of the application.
        /// </summary>
        public static App CurrentApplication { get; private set; }

        /// <summary>
        /// Gets whether or not we have started yet.
        /// </summary>
        internal bool IsStarted { get; private set; }

        /// <summary>
        /// Defines a handle to the Wayland registry.
        /// </summary>
        internal Registry Registry { get; private set; }
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
            if (_display == null) // Check for zero incase previous partial startup
                _display = Display.Connect();

            // Now hookup to the registry
            if (Registry == null) // Check for zero incase previous partial startup
                Registry = Registry.Get(_display);

            // Ensure our compositor and shell interfaces
            Registry.EnsureCoreGlobals();

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

        public void Run()
        {
            // Startup if required
            if (!IsStarted)
                Startup();

            // Now spin as long as the application is alive
            _display.RunLoop();
        }

        #region IDisposable Interface
        /// <summary>
        /// Free up our resources.
        /// </summary>
        public void Dispose()
        {
            Console.WriteLine("App disposed");
            if (_display != null)
            {
                _display.Dispose();
                _display = null;
            }
            if (Registry != null)
            {
                Registry.Dispose();
                Registry = null;
            }
            CurrentApplication = null;
        }
        #endregion
    }
}