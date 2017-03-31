using Ztk.Drawing;

namespace Ztk
{
    public class Button : Control
    {
        private static Brush _backgroundBrush;

        public object Content { get; set; }

        public FourSidedNumber Padding { get; set; }

        static Button()
        {
            _backgroundBrush = new LinearGradientBrush(0.5, 0, 0.5, 1, new GradientStop[]
            {
                new GradientStop(new Color(0.8, 0.8, 0.8), 0),
                new GradientStop(new Color(0.7, 0.7, 0.7), 0.5),
                new GradientStop(new Color(0.8, 0.8, 0.8), 1),
            });
        }

        public Button()
        {
            VerticalAlignment = VerticalAlignment.Top;
            HorizontalAlignment = HorizontalAlignment.Left;
            Padding = new FourSidedNumber(10, 5, 10, 5);
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
            g.RoundedRect(0, 0, ActualWidth, ActualHeight, 5, _backgroundBrush, Brushes.Black, 2);

            // Check we have text to draw
            string text = Content?.ToString();
            if (string.IsNullOrWhiteSpace(text))
                return;

            // Now draw the text
            Brushes.Black.ApplyBrushToContext(g);
            g.SelectFontFace("Sans", FontSlant.Normal, FontWeight.Normal);
            g.SetFontSize(14);
            TextExtents te = g.TextExtents(text);
            g.MoveTo(3 + te.XBearing * -1 + Padding.Left, 3 + te.YBearing * -1 + Padding.Top);
            g.ShowText(text);
        }
    }
}