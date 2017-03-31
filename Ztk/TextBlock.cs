using Ztk.Drawing;

namespace Ztk
{
    public class TextBlock : Control
    {
        public string Text { get; set; }

        public string FontFamily { get; set; }

        public double FontSize { get; set; }

        public Brush Foreground { get; set; }

        public FontSlant FontSlant { get; set; }

        public FontWeight FontWeight { get; set; }
        public FourSidedNumber Padding { get; set; }

        public Brush Background { get; set; }

        public TextBlock()
        {
            FontFamily = "Sans";
            FontSize = 12;
            FontSlant = FontSlant.Normal;
            FontWeight = FontWeight.Normal;
            Foreground = Brushes.Black;
            VerticalAlignment = VerticalAlignment.Top;
            HorizontalAlignment = HorizontalAlignment.Left;
        }

        public override Size MeasureDesiredSize(Size availableSize)
        {
            Size desiredSize = Text.GetSize(FontFamily, FontSize, FontSlant, FontWeight);
            double desiredHeight = desiredSize.Height + Padding.Top + Padding.Bottom;
            double desiredWidth = desiredSize.Width + Padding.Left + Padding.Right;
            Size newSize = new Size(desiredWidth <= availableSize.Width ? desiredWidth : availableSize.Width, desiredHeight <= availableSize.Height ? desiredHeight : availableSize.Height);
            if (HorizontalAlignment == HorizontalAlignment.Stretch)
                newSize.Width = availableSize.Width;
            if (VerticalAlignment == VerticalAlignment.Stretch)
                newSize.Height = availableSize.Height;
            return newSize;
        }

        public override void Render(GraphicsContext g)
        {
            PaintBackground(g, Background);

            if (Foreground == null || string.IsNullOrWhiteSpace(Text))
                return;

            Foreground.ApplyBrushToContext(g);
            g.SelectFontFace(FontFamily, FontSlant, FontWeight);
            g.SetFontSize(FontSize);
            TextExtents te = g.TextExtents(Text);
            g.MoveTo(te.XBearing * -1 + Padding.Left, te.YBearing * -1 + Padding.Top);
            g.ShowText(Text);
        }
    }
}