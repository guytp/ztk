using Ztk.Drawing;
using System;
using System.Collections.Generic;

namespace Ztk
{
    public abstract class Control
    {
        public double ActualWidth { get; private set; }

        public double ActualHeight { get; private set; }

        public double Opacity { get; internal set; }

        public FourSidedNumber Margin { get; set; }

        public HorizontalAlignment HorizontalAlignment { get; set; }

        public VerticalAlignment VerticalAlignment { get; set; }


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

        public abstract void Render(GraphicsContext g);

        #region External Event Notification
        internal void TriggerMouseEnter()
        {
            MouseEnter?.Invoke(this, new EventArgs());
        }

        internal void TriggerMouseLeave()
        {
            MouseLeave?.Invoke(this, new EventArgs());
        }

        internal void TriggerMouseMove(double x, double y)
        {
            MouseMove?.Invoke(this, new MouseMoveEventArgs(x, y));
        }

        internal void TriggerMouseButton(MouseButton mouseButton, bool isDown)
        {
            if (mouseButton == MouseButton.Left && isDown)
                MouseLeftButtonDown?.Invoke(this, new EventArgs());
            else if (mouseButton == MouseButton.Left && !isDown)
                MouseLeftButtonUp?.Invoke(this, new EventArgs());
            else if (mouseButton == MouseButton.Right && isDown)
                MouseRightButtonDown?.Invoke(this, new EventArgs());
            else if (mouseButton == MouseButton.Right && !isDown)
                MouseRightButtonUp?.Invoke(this, new EventArgs());
            if (isDown)
                MouseButtonDown?.Invoke(this, new MouseButtonEventArgs(mouseButton));
            else
                MouseButtonUp?.Invoke(this, new MouseButtonEventArgs(mouseButton));
        }
        #endregion
    }
}