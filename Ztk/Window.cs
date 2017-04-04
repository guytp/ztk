using System;
using System.Collections.Generic;
using Ztk.Drawing;
using Ztk.Wayland;

namespace Ztk
{
    public class Window : ContainerControl
    {
        #region Declarations
        /// <summary>
        /// Defines whether or not the window is shown.
        /// </summary>
        private bool _isShown;

        /// <summary>
        /// Gets a handle to the surface itself.
        /// </summary>
        private ShellSurface _surface;

        private readonly Queue<DateTime> _fpsQueue = new Queue<DateTime>();

        private DateTime _lastFpsOutput;
        #endregion

        protected override FourSidedNumber InternalSpacing
        {
            get { return new FourSidedNumber(0); }
        }
        public override HorizontalAlignment HorizontalAlignment
        {
            get { return base.HorizontalAlignment; }
            set
            {
                if (value != HorizontalAlignment.Stretch)
                    throw new Exception("Window must have HorizontalAlignment set as Stretch");
                base.HorizontalAlignment = value;
            }
        }

        public override VerticalAlignment VerticalAlignment
        {
            get { return base.VerticalAlignment; }
            set
            {
                if (value != VerticalAlignment.Stretch)
                    throw new Exception("Window must have VerticalAlignment set as Stretch");
                base.VerticalAlignment = value;
            }
        }

        protected double Fps { get; private set; }

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        public Window()
            : this(new Size(640, 480))
        {
        }

        public Window(Size initialSize)
        {

            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            Background = Brushes.White;
            SetActualSize(initialSize);
        }
        #endregion

        public override void Render(GraphicsContext g)
        {
            DateTime now = DateTime.UtcNow;
            while (_fpsQueue.Count > 0 && (_fpsQueue.Count > 500 || now.Subtract(_fpsQueue.Peek()).TotalSeconds > 30))
                _fpsQueue.Dequeue();
            _fpsQueue.Enqueue(now);

            base.Render(g);

            DateTime first = _fpsQueue.Peek();
            double totalSeconds = now.Subtract(first).TotalSeconds;
            if (_fpsQueue.Count > 50 && totalSeconds > 0)
            {
                Fps = _fpsQueue.Count / totalSeconds;
                if (now.Subtract(_lastFpsOutput).TotalSeconds > 3)
                {
                    Console.WriteLine("FPS: " + Fps);
                    _lastFpsOutput = now;
                }
            }
        }

        /// <summary>
        /// Show the window in the current application, starting a new application if required.
        /// </summary>
        public void Show()
        {
            // Ignore request if already shown
            if (_isShown)
                return;

            // Ensure application is running and if not start it although we are not resposible for the main loop
            if (App.CurrentApplication == null)
                new App();
            App app = App.CurrentApplication;
            if (!app.IsStarted)
                app.Startup();

            // Create our surface and shell surfaces accordingly - we trust App is initialised correctly
            Registry registry = app.Registry;
            _surface = registry.Compositor.CreateShellSurface(registry.SharedMemory, registry.Shell);
            _surface.RenderTarget = this;
            _surface.StartRenderingEvents();

            // Mark as initialised
            _isShown = true;
        }
        
        protected void DragMove()
        {
            if (LastPointerButtonEventArgs != null)
                _surface.Move(LastPointerButtonEventArgs);
        }
    }
}