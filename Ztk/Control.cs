using Ztk.Drawing;
using System;
using System.Collections.Generic;
using Ztk.Wayland;
using System.Linq;

namespace Ztk
{
    public abstract class Control
    {

        private Control _currentMouseFocus;

        public Brush Background { get; set; }

        public double ActualWidth { get; private set; }

        public double ActualHeight { get; private set; }

        protected bool AutoRenderBackground { get; set; }

        public double Opacity { get; set; }

        public FourSidedNumber Margin { get; set; }

        public string Id { get; set; }

        public virtual HorizontalAlignment HorizontalAlignment { get; set; }

        public virtual VerticalAlignment VerticalAlignment { get; set; }


        internal WaylandPointerButtonEventArgs LastPointerButtonEventArgs { get; private set; }

        protected List<Control> ChildrenInternal { get; private set; }
        protected List<LayoutInformation> LayoutInformation { get; set; }

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

        protected Control()
        {
            AutoRenderBackground = true;
            ChildrenInternal = new List<Control>();
            LayoutInformation = new List<LayoutInformation>();
            Opacity = 1;
        }

        internal void SetActualSize(Size size)
        {
            ActualWidth = size.Width;
            ActualHeight = size.Height;
        }

        protected void PaintBackground(GraphicsContext g, Brush background)
        {
            if (background == null)
                return;
            background.ApplyBrushToContext(g);
            g.Rectangle(0, 0, ActualWidth, ActualHeight);
            g.Fill();
        }

        public abstract Size MeasureDesiredSize(Size availableSize);
        

        #region Wayland Pointer Triggering
        internal virtual void TriggerWaylandPointerButton(WaylandPointerButtonEventArgs e)
        {
            // First store this as we need a reference if we want to enable certain things sucha s drag move
            LastPointerButtonEventArgs = e.IsButtonDown ? e : null;

            // Recursively walk through tree until we find our child
            PointD childCoordinates = new PointD();
            Control visualChild = FindChild(this, e.X, e.Y, ref childCoordinates);

            // Convert the mouse button from Wayland to ZTK terms
            MouseButton mouseButton;
            switch (e.MouseButton)
            {
                case WaylandMouseButton.Left:
                    mouseButton = MouseButton.Left;
                    break;
                case WaylandMouseButton.Right:
                    mouseButton = MouseButton.Right;
                    break;
                case WaylandMouseButton.Middle:
                    mouseButton = MouseButton.Middle;
                    break;
                case WaylandMouseButton.Side:
                    mouseButton = MouseButton.Side;
                    break;
                case WaylandMouseButton.Extra:
                    mouseButton = MouseButton.Extra;
                    break;
                case WaylandMouseButton.Forward:
                    mouseButton = MouseButton.Forward;
                    break;
                case WaylandMouseButton.Back:
                    mouseButton = MouseButton.Back;
                    break;
                case WaylandMouseButton.Task:
                    mouseButton = MouseButton.Task;
                    break;
                default:
                    throw new Exception("Unknown mouse button");
            }

            // Now trigger the mouse event in non-Wayland terms in there
            if (e.IsButtonDown)
                visualChild.MouseButtonDown?.Invoke(this, new MouseButtonEventArgs(mouseButton));
            else
                visualChild.MouseButtonUp?.Invoke(this, new MouseButtonEventArgs(mouseButton));
            if (mouseButton == MouseButton.Left && e.IsButtonDown)
                visualChild.MouseLeftButtonDown?.Invoke(this, new EventArgs());
            else if (mouseButton == MouseButton.Left && !e.IsButtonDown)
                visualChild.MouseLeftButtonUp?.Invoke(this, new EventArgs());
            else if (mouseButton == MouseButton.Right && e.IsButtonDown)
                visualChild.MouseRightButtonDown?.Invoke(this, new EventArgs());
            else if (mouseButton == MouseButton.Right && !e.IsButtonDown)
                visualChild.MouseRightButtonUp?.Invoke(this, new EventArgs());
        }

        internal void TriggerWaylandMouseEnter(double x, double y)
        {
            // Mouse has entered our window at specified co-ordinates, terminate any existing focus to start with
            _currentMouseFocus?.MouseLeave?.Invoke(this, new EventArgs());
            _currentMouseFocus = null;

            // Detrmine who, within our view hierarchy, has the focus
            PointD childCoordinates = new PointD();
            _currentMouseFocus = FindChild(this, x, y, ref childCoordinates);
            //Console.WriteLine("MouseEnter to " + _currentMouseFocus.GetType().Name);

            // Store this focus and trigger mouse enter
            _currentMouseFocus.MouseEnter?.Invoke(this, new EventArgs());
        }

        internal void TriggerWaylandMouseLeave()
        {
            // If we recorded who had focus remove it now then null it out
            _currentMouseFocus?.MouseLeave?.Invoke(this, new EventArgs());
            //Console.WriteLine("MouseLeave to " + _currentMouseFocus.GetType().Name);
            _currentMouseFocus = null;
        }

        internal void TriggerWaylandMouseMove(double x, double y)
        {
            // First determine where this move would put us in terms of visual child
            PointD localCoordinates = new PointD(x, y);
            Control visualChild = FindChild(this, x, y, ref localCoordinates);

            // If mouse focus has changed trigger appropriate mouse in/out events down the chain
            if (visualChild != _currentMouseFocus)
            {
                // If old control was not null then it needs a leave event
                _currentMouseFocus?.MouseLeave?.Invoke(this, new EventArgs());
                //Console.WriteLine("MouseLeave to " + _currentMouseFocus.GetType().Name);

                // Trigger a mouse enter into new child
                _currentMouseFocus = visualChild;
                _currentMouseFocus.MouseEnter?.Invoke(this, new EventArgs());
                //Console.WriteLine("MouseEnter to " + _currentMouseFocus.GetType().Name);

                // Store this as the current focus
            }
            _currentMouseFocus.MouseMove?.Invoke(this, new MouseMoveEventArgs(localCoordinates.X, localCoordinates.Y));
            //Console.WriteLine("MouseMove to " + _currentMouseFocus.GetType().Name + ", local " + localCoordinates.X + ", " + localCoordinates.Y + "   glob " + x + ", " + y);
        }
        #endregion

        private Control FindChild(Control child, double x, double y, ref PointD outputChildCoordinates)
        {
            // Enumerate through potential children by highest Z order
            //Console.WriteLine("Checking " + child.GetType().Name + " for mouse at " + x + ", " + y);
            foreach (LayoutInformation layoutInformation in child.LayoutInformation.OrderByDescending(li => li.ZIndex))
            {
                Rectangle r = layoutInformation.Rectangle;
                if (x >= r.X && x <= r.X + r.Width && y >= r.Y && y <= r.Y + r.Height)
                {
                    // Pass down and look for children here recursively - adjust X/Y co-ordinates to be local to that control
                    //Console.WriteLine("Seems to be within " + layoutInformation.Control.GetType().Name + " for mouse - checking in there");
                    return FindChild(layoutInformation.Control, x - r.X, y - r.Y, ref outputChildCoordinates);
                }
            }

            // We didn't find anything so assume it's this one
            //Console.WriteLine("Returning " + child.GetType().Name + " for mouse - not found in descendent");
            outputChildCoordinates.X = x;
            outputChildCoordinates.Y = y;
            return child;
        }

        public virtual void Render(GraphicsContext g)
        {
            // Paint our background
            if (AutoRenderBackground)
                PaintBackground(g, Background);

            RenderChildren(g);
        }

        protected virtual void RenderChildren(GraphicsContext g)
        {
            foreach (LayoutInformation layoutInformation in LayoutInformation.OrderByDescending(li => li.ZIndex))
            {
                // Now render the child
                int width = (int)Math.Round(layoutInformation.Rectangle.Width);
                int height = (int)Math.Round(layoutInformation.Rectangle.Height);
                if (width ==0 || height == 0)
                    continue;
                using (ImageSurface surface = new ImageSurface(Format.ARGB32, width, height))
                {
                    using (GraphicsContext childContext = new GraphicsContext(surface))
                    {
                        layoutInformation.Control.Render(childContext);
                        g.SetSourceSurface(surface, (int)Math.Round(layoutInformation.Rectangle.X), (int)Math.Round(layoutInformation.Rectangle.Y));
                        g.PaintWithAlpha(layoutInformation.Control.Opacity);
                    }
                }
            }
        }
    }
}