using Ztk.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Ztk
{
    public class Window
    {
        #region Native Interop
        [DllImport("wayland-wrapper", EntryPoint = "wlw_shell_surface_move")]
        private static extern void ShellSurfaceMove(IntPtr shellSurface, IntPtr seat, uint serial);
        #endregion

        #region Declarations
        /// <summary>
        /// Defines whether or not the window is shown.
        /// </summary>
        private bool _isShown;


        /// <summary>
        /// Defines a handle to the shell surface.
        /// </summary>
        private IntPtr _shellSurface;

        /// <summary>
        /// Defines the buffer we use for drawing on the surfaec.
        /// </summary>
        private Buffer _buffer;

        private Control _currentMouseFocus;

        private WaylandWrapperInterop.CallbackListener _frameListener;
        private readonly WaylandWrapperInterop.ShellSurfacePingListener _surfacePingListener;
        private readonly WaylandWrapperInterop.ShellSurfaceConfigureListener _surfaceConfigureListener;
        private readonly WaylandWrapperInterop.ShellSurfacePopupDoneListener _surfacePopupDoneListener;

        private Pointer _lastButtonPointer;
        private Seat _lastButtonSeat;


        private readonly List<KeyValuePair<Rectangle, Control>> _childRectangles = new List<KeyValuePair<Rectangle, Control>>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets a handle to the surface itself.
        /// </summary>
        internal IntPtr Surface { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public Brush Background { get; set; }

        public Control Child { get; set; }
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
            Width = 640;
            Height = 480;
            Background = Brushes.White;
            _frameListener = OnFrameListener;
            _surfacePingListener = OnShellSurfacePing;
            _surfaceConfigureListener = OnShellSurfaceConfigure;
            _surfacePopupDoneListener = OnShellSurfacePopupDone;
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

            // Ensure ARGB8888 is supported
            if (!App.CurrentApplication.SharedMemoryFormats.Contains(SharedMemoryFormat.ARGB8888))
                throw new Exception("ARGB8888 is not supported by compositor");

            // Create our surface and shell surfaces accordingly - we trust App is initialised correctly
            Surface = WaylandWrapperInterop.CompositorCreateSurface(App.CurrentApplication.Compositor);
            if (Surface == IntPtr.Zero)
                throw new Exception("Failed to create a surface");
            _shellSurface = WaylandWrapperInterop.ShellGetShellSurface(App.CurrentApplication.Shell, Surface);
            if (_shellSurface == IntPtr.Zero)
                throw new Exception("Failed to create shell surface");

            // Now set surface as top level
            WaylandWrapperInterop.ShellSurfaceSetTopLevel(_shellSurface);

            // Hookup to events for the shell surface
            WaylandWrapperInterop.ShellSurfaceAddListeners(_shellSurface, _surfacePingListener, _surfaceConfigureListener, _surfacePopupDoneListener);

            // Hookup to shared memory via buffer
            _buffer = new Buffer(App.CurrentApplication.SharedMemory, Width, Height, SharedMemoryFormat.ARGB8888);

            // Perform initial render
            Render();

            // Mark as initialised
            _isShown = true;

            // Inform the app of our surfaces
            App.CurrentApplication.NotifyNewWindow(this);
            // TODO: Equivalent on close
        }


        internal void Render()
        {
            // TODO: Support multiple window callbacks here - wrapper only accepts one presently
            WaylandWrapperInterop.SurfaceDamage(Surface, 0, 0, Width, Height);
            WaylandWrapperInterop.SurfaceAttach(Surface, _buffer.BufferPointer);
            int pixels = Width * Height * 4;
            Random rand = new Random();
            using (ImageSurface imageSurface = new ImageSurface(Format.ARGB32, Width, Height))
            {
                using (GraphicsContext g = new GraphicsContext(imageSurface))
                {
                    // Paint our background
                    if (Background != null)
                    {
                        Background.ApplyBrushToContext(g);
                        g.Rectangle(0, 0, Width, Height);
                        g.Fill();
                    }

                    if (Child != null)
                    {
                        // Perform a measure pass on this child
                        double availableWidth = Width - Child.Margin.Left - Child.Margin.Right;
                        double availableHeight = Height - Child.Margin.Top - Child.Margin.Bottom;
                        Size desiredSize = Child.MeasureDesiredSize(new Size(Width, Height));
                        if (Child.VerticalAlignment == VerticalAlignment.Stretch)
                            desiredSize.Height = availableHeight;
                        if (Child.HorizontalAlignment == HorizontalAlignment.Stretch)
                            desiredSize.Width = availableWidth;
                        Child.SetActualSize(new Size(desiredSize.Width <= availableWidth ? desiredSize.Width : availableWidth, desiredSize.Height <= availableHeight ? desiredSize.Height : availableHeight));

                        // Clear our child rectangles out
                        _childRectangles.Clear();

                        // Now render the child
                        using (ImageSurface childSurface = new ImageSurface(Format.ARGB32, (int)Math.Round(Child.ActualWidth), (int)Math.Round(Child.ActualHeight)))
                        {
                            using (GraphicsContext childContext = new GraphicsContext(childSurface))
                            {
                                int x;
                                switch (Child.HorizontalAlignment)
                                {
                                    case HorizontalAlignment.Stretch:
                                    case HorizontalAlignment.Left:
                                        x = (int)Math.Round(Child.Margin.Left);
                                        break;
                                    case HorizontalAlignment.Right:
                                        x = (int)Math.Round(Width - Child.Margin.Right - Child.ActualWidth);
                                        break;
                                    case HorizontalAlignment.Middle:
                                        x = (int)Math.Round((Width / 2) - ((Child.Margin.Left + Child.Margin.Right + Child.ActualWidth) / 2));
                                        break;
                                    default:
                                        throw new Exception("Unsupported horizontal alignment");
                                }
                                if (x < 0)
                                    x = 0;
                                if (x > Width)
                                    x = Width;
                                int y;
                                switch (Child.VerticalAlignment)
                                {
                                    case VerticalAlignment.Stretch:
                                    case VerticalAlignment.Top:
                                        y = (int)Math.Round(Child.Margin.Top);
                                        break;
                                    case VerticalAlignment.Bottom:
                                        y = (int)Math.Round(Height - Child.Margin.Bottom - Child.ActualHeight);
                                        break;
                                    case VerticalAlignment.Middle:
                                        y = (int)Math.Round((Height / 2) - ((Child.Margin.Top + Child.Margin.Bottom + Child.ActualHeight) / 2));
                                        break;
                                    default:
                                        throw new Exception("Unsupported vertical alignment");
                                }

                                if (y < 0)
                                    y = 0;
                                if (y > Height)
                                    y = Height;
                                Child.Render(childContext);
                                g.SetSourceSurface(childSurface, x, y);
                                g.PaintWithAlpha(Child.Opacity);
                                _childRectangles.Add(new KeyValuePair<Rectangle, Control>(new Rectangle(x, y, Child.ActualWidth, Child.ActualHeight), Child));
                            }
                        }
                    }

                    Marshal.Copy(imageSurface.Data, 0, _buffer.SharedMemoryPointer, imageSurface.Data.Length);

                }
                WaylandWrapperInterop.SurfaceAddFrameListener(Surface, _frameListener);
                WaylandWrapperInterop.SurfaceCommit(Surface);
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
            Control mouseControl = GetControlAtLocation(x, y);

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
                Rectangle childRectangle = _childRectangles.First(kvp => kvp.Value == _currentMouseFocus).Key;
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

        private Control GetControlAtLocation(double x, double y)
        {
            for (int i = _childRectangles.Count - 1; i >= 0; i--)
            {
                Rectangle rectangle = _childRectangles[i].Key;
                double minX = rectangle.X;
                double minY = rectangle.Y;
                double maxX = minX + rectangle.Width;
                double maxY = minY + rectangle.Height;
                if (x >= minX && x <= maxX && y >= minY && y <= maxY)
                    return _childRectangles[i].Value;
            }
            return null;
        }
        #endregion


        #region Event Handlers
        private void OnFrameListener(IntPtr data, IntPtr callback, uint callbackData)
        {
            Render();
        }

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

        protected void DragMove()
        {
            if (_lastButtonPointer != null && _lastButtonSeat != null)
                ShellSurfaceMove(_shellSurface, _lastButtonSeat.Handle, _lastButtonPointer.Serial);
        }
    }
}