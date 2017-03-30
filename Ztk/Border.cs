using Ztk.Drawing;
using System;

namespace Ztk
{
    public class Border : ContainerControl
    {
        public double BorderThickness { get; set; }

        public Brush BorderBrush { get; set; }

        public double CornerRadius { get; set; }


        protected override FourSidedNumber InternalSpacing { get { return new FourSidedNumber(BorderThickness); } }

        public Border()
        {
            BorderBrush = Brushes.Black;
            AutoRenderBackground = false;
            BorderThickness = 1;
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
                g.LineWidth = BorderThickness + 1;
                BorderBrush.ApplyBrushToContext(g);
                g.Stroke();
            }

            base.Render(g);
        }
    }
}