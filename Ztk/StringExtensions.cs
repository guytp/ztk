using Ztk.Drawing;

namespace Ztk
{
    public static class StringExtensions
    {
        public static Size GetSize(this string value, string fontFamily, double fontSize, FontSlant fontSlant, FontWeight fontWeight)
        {
            using (ImageSurface surface = new ImageSurface(Format.Argb32, 0, 0))
            {
                using (GraphicsContext context = new GraphicsContext(surface))
                {
                    context.SelectFontFace(fontFamily, fontSlant, fontWeight);
                    context.SetFontSize(fontSize);
                    TextExtents te = context.TextExtents(value);
                    double width = te.Width;
                    double height = te.Height;
                    return new Size(width, height);
                }
            }
        }
    }
}
