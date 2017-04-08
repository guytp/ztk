using System;
using Ztk.Drawing;

namespace Ztk
{
    public class Button : Control
    {
        private static Brush _backgroundBrush;

        private static Brush _mouseOverBrush;

        private bool _isMouseOver;

        private readonly ClickStateTracker _clickStateTracker;

        public object Content { get; set; }

        public FourSidedNumber Padding { get; set; }

        static Button()
        {
            _backgroundBrush = new SolidColorBrush(new Color(0x1F / 255f));
            _mouseOverBrush = new SolidColorBrush(new Color(0x35/255f));
        }


        public event EventHandler Click;
        public event EventHandler DoubleClick;

        public Button()
        {
            VerticalAlignment = VerticalAlignment.Top;
            HorizontalAlignment = HorizontalAlignment.Left;
            Padding = new FourSidedNumber(10, 5, 10, 5);
            _clickStateTracker = new ClickStateTracker(this);
            _clickStateTracker.Click += OnClick;
            _clickStateTracker.DoubleClick += OnDoubleClick;
            MouseEnter += OnMouseEnter;
            MouseLeave += Button_MouseLeave;
        }

        private void Button_MouseLeave(object sender, EventArgs e)
        {
            _isMouseOver = false;
        }
        private void OnMouseEnter(object sender, EventArgs e)
        {
            _isMouseOver = true;
        }

        private void OnClick(object sender, EventArgs e)
        {
            Click?.Invoke(this, new EventArgs());
        }

        private void OnDoubleClick(object sender, EventArgs e)
        {
            DoubleClick?.Invoke(this, new EventArgs());
        }

        public override Size MeasureDesiredSize(Size availableSize)
        {
            // First figure out size of our text and padding
            string text = Content.ToString();
            Size desiredSize = text.GetSize("Sans", 14, FontSlant.Normal, FontWeight.Normal);
            double desiredHeight = desiredSize.Height + Padding.Top + Padding.Bottom;
            double desiredWidth = desiredSize.Width + Padding.Left + Padding.Right;
            Size newSize = new Size(desiredWidth <= availableSize.Width ? desiredWidth : availableSize.Width, desiredHeight <= availableSize.Height ? desiredHeight : availableSize.Height);

            // Now add our button border on to this
            newSize.Width = newSize.Width + 6;
            newSize.Height = newSize.Height + 6;

            // Ensure we are within bounds
            if (newSize.Width > availableSize.Width)
                newSize.Width = availableSize.Width;
            if (newSize.Height > availableSize.Height)
                newSize.Height = availableSize.Height;

            // Adjust desired size if we're stretch
            if (HorizontalAlignment == HorizontalAlignment.Stretch)
                newSize.Width = availableSize.Width;
            if (VerticalAlignment == VerticalAlignment.Stretch)
                newSize.Height = availableSize.Height;
            return newSize;
        }

        public override void Render(GraphicsContext g)
        {
            // First draw and fill our background
            Brush brush = _isMouseOver ? _mouseOverBrush : _backgroundBrush;
            g.Rectangle(0, 0, ActualWidth, ActualHeight);
            brush.ApplyBrushToContext(g);
            g.Fill();
            //g.RoundedRect(0, 0, ActualWidth, ActualHeight, 5, brush, brush, 1);

            // Check we have text to draw
            string text = Content?.ToString();
            if (string.IsNullOrWhiteSpace(text))
                return;

            // Now draw the text
            Brushes.White.ApplyBrushToContext(g);
            g.SelectFontFace("Sans", FontSlant.Normal, FontWeight.Normal);
            g.SetFontSize(32);
            TextExtents te = g.TextExtents(text);
            double y = te.YBearing * -1 + Padding.Top;
            double x = te.XBearing * -1 + Padding.Left;
            double xRemaining = ActualWidth - Padding.Left - te.Width - Padding.Right;
            double yRemaining = ActualHeight - Padding.Top - te.Height - Padding.Bottom;
            g.MoveTo(x + (xRemaining / 2), y + (yRemaining / 2));
            g.ShowText(text);
        }
    }
}