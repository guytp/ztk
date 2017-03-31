using Ztk.Drawing;
using System;

namespace Ztk
{
    public class LinearGradientBrush : Brush
    {
        public double X0 { get; set; }

        public double Y0 { get; set; }

        public double X1 { get; set; }

        public double Y1 { get; set; }

        public GradientStop[] GradientStops { get; set; }

        public LinearGradientBrush(double x0, double y0, double x1, double y1, GradientStop[] gradientStops)
        {
            X0 = x0;
            Y0 = y0;
            X1 = x1;
            Y1 = y1;
            GradientStops = gradientStops;
        }

        public override void ApplyBrushToContext(GraphicsContext g)
        {
            using (ImageSurface surface = ((ImageSurface)g.GetTarget()))
            {
                double actualWidth = surface.Width;
                double actualHeight = surface.Height;
                using (LinearGradient lg = new LinearGradient(X0 * actualWidth, Y0 * actualHeight, X1 * actualWidth, Y1 * actualHeight))
                {
                    if (GradientStops != null)
                        foreach (GradientStop gs in GradientStops)
                            lg.AddColorStop(gs.Offset, gs.Color);
                    g.SetSource(lg);
                }
            }
        }
    }
}
