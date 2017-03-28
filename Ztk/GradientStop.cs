using Ztk.Drawing;

namespace Ztk
{
    public class GradientStop
    {
        public Color Color { get; set; }

        public double Offset { get; set; }

        public GradientStop(Color color, double offset)
        {
            Color = color;
            Offset = offset;
        }
    }
}