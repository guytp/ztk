using System;
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

        private WaylandPointerButtonEventArgs _lastPointerButtonEventArgs;
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

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        public Window()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            Background = Brushes.White;
            SetActualSize(new Size(640, 480));
        }
        #endregion

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
        

        #region External Event Notification
        internal void TriggerMouseEnter(double x, double y)
        {
            /*
            _currentMouseFocus?.TriggerMouseLeave();
            _currentMouseFocus = null;
            TriggerMouseMove(x, y);
            MouseEnter?.Invoke(this, new EventArgs());
            */
        }

        internal void TriggerWaylandMouseLeave()
        {
            /*
            if (_currentMouseFocus != null)
                _currentMouseFocus.TriggerMouseLeave();
            MouseLeave?.Invoke(this, new EventArgs());
            _currentMouseFocus = null;
            */
        }

        internal void TriggerWaylandMouseMove(double x, double y)
        {
            /*
            Control mouseControl = GetChildAtLocation(x, y);

            // If mouse focus has changed trigger appropriate mouse in/out events down the chain
            if (mouseControl != _currentMouseFocus)
            {
                // If old control was not null then it needs a leave event
                _currentMouseFocus?.TriggerMouseLeave();

                // If new control is not null then trigger a mouse enter
                mouseControl?.TriggerMouseEnter();

                // Store this as the current focus
                _currentMouseFocus = mouseControl;
            }
            if (_currentMouseFocus != null)
            {
                Rectangle childRectangle = GetLayoutInformationForChild(Child).Rectangle;
                _currentMouseFocus.TriggerMouseMove(x - childRectangle.X, y - childRectangle.Y);
            }
            else
                MouseMove?.Invoke(this, new MouseMoveEventArgs(x, y));
                */
        }

        internal void TriggerWaylandPointerButton(WaylandPointerButtonEventArgs e)
        {
            // First store this as we need a reference if we want to enable certain things sucha s drag move
            _lastPointerButtonEventArgs = e;

            // Now convert this into a ZTK event and start it firing up the tree
            /*
            if (_currentMouseFocus != null)
            {
                _currentMouseFocus.TriggerMouseButton(e.MouseButton, e.IsDown);
                return;
            }
            if (isDown)
            {
                _lastButtonPointer = pointer;
                _lastButtonSeat = seat;
                MouseButtonDown?.Invoke(this, new MouseButtonEventArgs(mouseButton));
            }
            else
            {
                _lastButtonPointer = null;
                _lastButtonSeat = null;
                MouseButtonUp?.Invoke(this, new MouseButtonEventArgs(mouseButton));
            }
            if (mouseButton == MouseButton.Left && isDown)
                MouseLeftButtonDown?.Invoke(this, new EventArgs());
            else if (mouseButton == MouseButton.Left && !isDown)
                MouseLeftButtonUp?.Invoke(this, new EventArgs());
            else if (mouseButton == MouseButton.Right && isDown)
                MouseRightButtonDown?.Invoke(this, new EventArgs());
            else if (mouseButton == MouseButton.Right && !isDown)
                MouseRightButtonUp?.Invoke(this, new EventArgs());
                */
        }
        #endregion

        protected void DragMove()
        {
            if (_lastPointerButtonEventArgs != null)
                _surface.Move(_lastPointerButtonEventArgs);
        }
    }
}