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
            g.RoundedRect(x, y, ActualWidth, ActualHeight, CornerRadius, Background, BorderBrush, BorderThickness);

            base.Render(g);
        }
    }
}