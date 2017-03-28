using Ztk.Drawing;

namespace Ztk
{
    public abstract class Brush
    {
        public abstract void ApplyBrushToContext(GraphicsContext g);
    }
}