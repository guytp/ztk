using Ztk.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ztk
{
    public class SolidColorBrush : Brush
    {
        public Color Color { get; set; }

        public SolidColorBrush(Color color)
        {
            Color = color;
        }

        public override void ApplyBrushToContext(GraphicsContext g)
        {
            g.SetSourceColor(Color);
        }
    }
}
