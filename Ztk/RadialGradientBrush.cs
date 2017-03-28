using Ztk.Drawing;

namespace Ztk
{
    public class RadialGradientBrush : Brush
    {
        public double CX0 { get; set; }

        public double CY0 { get; set; }

        public double Radius0 { get; set; }

        public double CX1 { get; set; }

        public double CY1 { get; set; }

        public double Radius1 { get; set; }

        public GradientStop[] GradientStops { get; set; }

        public RadialGradientBrush(double cx0, double cy0, double radius0, double cx1, double cy1, double radius1, GradientStop[] gradientStops)
        {
            CX0 = cx0;
            CY0 = cy0;
            Radius0 = radius0;
            CX1 = cx1;
            CY1 = cy1;
            Radius1 = radius1;
            GradientStops = gradientStops;
        }

        public override void ApplyBrushToContext(GraphicsContext g)
        {
            using (ImageSurface surface = ((ImageSurface)g.GetTarget()))
            {
                double actualWidth = surface.Width;
                double actualHeight = surface.Height;
                double maximumRadius = actualWidth > actualHeight ? actualWidth : actualHeight;
                using (RadialGradient lg = new RadialGradient(CX0 * actualWidth, CY0 * actualHeight, Radius0 * maximumRadius, CX1 * actualWidth, CY1 * actualHeight, Radius1 * maximumRadius))
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