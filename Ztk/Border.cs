using Ztk.Drawing;
using System;

namespace Ztk
{
    public class Border : ContainerControl
    {
        public double BorderThickness { get; set; }

        public Brush BorderBrush { get; set; }

        public double CornerRadius { get; set; }

        public Border()
        {
            BorderBrush = Brushes.Black;
            BorderThickness = 1;
        }

        public override Size MeasureDesiredSize(Size availableSize)
        {
            // If no child just return our border size
            if (Child == null)
            {
                Size borderSize = new Size(BorderThickness * 2, BorderThickness * 2);
                if (borderSize.Width > availableSize.Width)
                    borderSize.Width = availableSize.Width;
                if (borderSize.Height > availableSize.Height)
                    borderSize.Height = availableSize.Height;
                return borderSize;
            }

            // Subtract the size of border away from available size
            availableSize.Width -= BorderThickness * 2 + Child.Margin.Left + Child.Margin.Right;
            availableSize.Height -= BorderThickness * 2 + Child.Margin.Top + Child.Margin.Bottom;
            if (availableSize.Width < 0)
                availableSize.Width = 0;
            if (availableSize.Height < 0)
                availableSize.Height = 0;

            // Use this for child size
            Size childSize = Child.MeasureDesiredSize(availableSize);
            Child.SetActualSize(childSize);

            // Now add our sizing and return - although if we Child is stretch then adjust accordingly
            childSize.Width += BorderThickness * 2 + Child.Margin.Left + Child.Margin.Right;
            childSize.Height += BorderThickness * 2 + Child.Margin.Top + Child.Margin.Bottom;
            if (Child.HorizontalAlignment == HorizontalAlignment.Stretch)
                childSize.Width = availableSize.Width;
            if (Child.VerticalAlignment == VerticalAlignment.Stretch)
                childSize.Height = availableSize.Height;
            return childSize;
        }

        public override void Render(GraphicsContext g)
        {
            // Draw our rectangle
            double x = 0;
            double y = 0;

            double degrees = Math.PI / 180;
            g.NewSubPath();
            g.Arc(x + ActualWidth - CornerRadius, y + CornerRadius, CornerRadius, -90 * degrees, 0 * degrees);
            g.Arc(x + ActualWidth - CornerRadius, y + ActualHeight - CornerRadius, CornerRadius, 0 * degrees, 90 * degrees);
            g.Arc(x + CornerRadius, y + ActualHeight - CornerRadius, CornerRadius, 90 * degrees, 180 * degrees);
            g.Arc(x + CornerRadius, y + CornerRadius, CornerRadius, 180 * degrees, 270 * degrees);
            g.ClosePath();


            if (Background != null)
            {
                Background.ApplyBrushToContext(g);
                g.FillPreserve();
            }
            if (BorderBrush != null)
            {
                g.LineWidth = BorderThickness;
                BorderBrush.ApplyBrushToContext(g);
                g.Stroke();
            }

            if (Child == null)
                return;

            // Now draw the child
            double width = ActualWidth - (2 * BorderThickness);
            double height = ActualWidth - (2 * BorderThickness);
            using (ImageSurface childSurface = new ImageSurface(Format.ARGB32, (int)Math.Round(width), (int)Math.Round(height)))
            {
                using (GraphicsContext childContext = new GraphicsContext(childSurface))
                {
                    switch (Child.HorizontalAlignment)
                    {
                        case HorizontalAlignment.Stretch:
                        case HorizontalAlignment.Left:
                            x = Child.Margin.Left + BorderThickness;
                            break;
                        case HorizontalAlignment.Right:
                            x = ActualWidth - Child.Margin.Right - Child.ActualWidth - BorderThickness;
                            break;
                        case HorizontalAlignment.Middle:
                            x = ((width / 2) - ((Child.ActualWidth + Child.Margin.Left + Child.Margin.Right) / 2)) + BorderThickness;
                            break;
                        default:
                            throw new Exception("Unsupported horizontal alignment");
                    }
                    if (x < 0)
                        x = 0;
                    if (x > width)
                        x = width;
                    switch (Child.VerticalAlignment)
                    {
                        case VerticalAlignment.Stretch:
                        case VerticalAlignment.Top:
                            y = Child.Margin.Top + BorderThickness;
                            break;
                        case VerticalAlignment.Bottom:
                            y = ActualHeight - Child.Margin.Bottom - Child.ActualHeight - BorderThickness;
                            break;
                        case VerticalAlignment.Middle:
                            y = (height / 2) - ((Child.Margin.Top + Child.Margin.Bottom + Child.ActualHeight) / 2) + BorderThickness;
                            break;
                        default:
                            throw new Exception("Unsupported vertical alignment");
                    }
                    if (y < 0)
                        y = 0;
                    if (y > height)
                        y = height;
                    Child.Render(childContext);
                    g.SetSourceSurface(childSurface, (int)Math.Round(x), (int)Math.Round(y));
                    g.PaintWithAlpha(Child.Opacity);
                }
            }
        }
    }
}