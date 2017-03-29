using Ztk.Drawing;
using System;
using Ztk.Wayland;
using WaylandSurface = Ztk.Wayland.Surface;

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
        /// Defines a handle to the shell surface.
        /// </summary>
        private IntPtr _shellSurface;

        private Control _currentMouseFocus;

        private Pointer _lastButtonPointer;

        private Seat _lastButtonSeat;

        /// <summary>
        /// Gets a handle to the surface itself.
        /// </summary>
        private WaylandSurface _surface;
        #endregion

        #region Events
        public event EventHandler MouseEnter;
        public event EventHandler MouseLeave;
        public event EventHandler<MouseMoveEventArgs> MouseMove;
        public event EventHandler MouseLeftButtonDown;
        public event EventHandler MouseLeftButtonUp;
        public event EventHandler MouseRightButtonDown;
        public event EventHandler MouseRightButtonUp;
        public event EventHandler<MouseButtonEventArgs> MouseButtonDown;
        public event EventHandler<MouseButtonEventArgs> MouseButtonUp;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        public Window()
        {
            Background = Brushes.White;
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
            if (!App.CurrentApplication.IsStarted)
                App.CurrentApplication.Startup();

            // Create our surface and shell surfaces accordingly - we trust App is initialised correctly
            _surface = App.CurrentApplication.Compositor.CreateSurface(App.CurrentApplication.SharedMemory);
            _surface.ConvertToShell();
            _surface.StartRenderingEvents();

            // Mark as initialised
            _isShown = true;

            // Inform the app of our surfaces
            App.CurrentApplication.NotifyNewWindow(this);
        }

        public override void Render(GraphicsContext g)
        {
            // Paint our background
            PaintBackground(g, Background);

            int zIndex = 0;
            foreach (Control child in ChildrenInternal)
            {
                // Perform a measure pass on this child
                double availableWidth = ActualWidth - Child.Margin.Left - Child.Margin.Right;
                double availableHeight = ActualHeight - Child.Margin.Top - Child.Margin.Bottom;
                Size desiredSize = Child.MeasureDesiredSize(new Size(ActualWidth, ActualHeight));
                if (Child.VerticalAlignment == VerticalAlignment.Stretch)
                    desiredSize.Height = availableHeight;
                if (Child.HorizontalAlignment == HorizontalAlignment.Stretch)
                    desiredSize.Width = availableWidth;
                Child.SetActualSize(new Size(desiredSize.Width <= availableWidth ? desiredSize.Width : availableWidth, desiredSize.Height <= availableHeight ? desiredSize.Height : availableHeight));

                // Now render the child
                using (ImageSurface childSurface = new ImageSurface(Format.ARGB32, (int)Math.Round(Child.ActualWidth), (int)Math.Round(Child.ActualHeight)))
                {
                    using (GraphicsContext childContext = new GraphicsContext(childSurface))
                    {
                        double x;
                        switch (Child.HorizontalAlignment)
                        {
                            case HorizontalAlignment.Stretch:
                            case HorizontalAlignment.Left:
                                x = Child.Margin.Left;
                                break;
                            case HorizontalAlignment.Right:
                                x = ActualWidth - Child.Margin.Right - Child.ActualWidth;
                                break;
                            case HorizontalAlignment.Middle:
                                x = (ActualWidth / 2) - ((Child.Margin.Left + Child.Margin.Right + Child.ActualWidth) / 2);
                                break;
                            default:
                                throw new Exception("Unsupported horizontal alignment");
                        }
                        if (x < 0)
                            x = 0;
                        if (x > ActualWidth)
                            x = ActualWidth;
                        double y;
                        switch (Child.VerticalAlignment)
                        {
                            case VerticalAlignment.Stretch:
                            case VerticalAlignment.Top:
                                y = Child.Margin.Top;
                                break;
                            case VerticalAlignment.Bottom:
                                y = ActualHeight - Child.Margin.Bottom - Child.ActualHeight;
                                break;
                            case VerticalAlignment.Middle:
                                y = (ActualHeight / 2) - ((Child.Margin.Top + Child.Margin.Bottom + Child.ActualHeight) / 2);
                                break;
                            default:
                                throw new Exception("Unsupported vertical alignment");
                        }

                        if (y < 0)
                            y = 0;
                        if (y > ActualHeight)
                            y = ActualHeight;
                        Child.Render(childContext);
                        g.SetSourceSurface(childSurface, (int)Math.Round(x), (int)Math.Round(y));
                        g.PaintWithAlpha(Child.Opacity);
                        LayoutInformation layoutInformation = GetLayoutInformationForChild(Child);
                        layoutInformation.Rectangle = new Rectangle(x, y, Child.ActualWidth, Child.ActualHeight);
                        layoutInformation.ZIndex = zIndex++;
                    }
                }
            }

        }


        #region External Event Notification
        internal void TriggerMouseEnter(double x, double y)
        {
            _currentMouseFocus?.TriggerMouseLeave();
            _currentMouseFocus = null;
            TriggerMouseMove(x, y);
            MouseEnter?.Invoke(this, new EventArgs());
        }

        internal void TriggerMouseLeave()
        {
            if (_currentMouseFocus != null)
                _currentMouseFocus.TriggerMouseLeave();
            MouseLeave?.Invoke(this, new EventArgs());
            _currentMouseFocus = null;
        }

        internal void TriggerMouseMove(double x, double y)
        {
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
        }

        internal void TriggerMouseButton(MouseButton mouseButton, bool isDown, Pointer pointer, Seat seat)
        {
            if (_currentMouseFocus != null)
            {
                _currentMouseFocus.TriggerMouseButton(mouseButton, isDown);
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
        }

        #endregion


        protected void DragMove()
        {
            if (_lastPointerButtonEventArgs != null)
                _surface.Move(_lastPointerButtonEventArgs);
        }
    }
}