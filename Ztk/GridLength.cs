namespace Ztk
{
    public class GridLength
    {
        public double Length { get; set; }
        public GridLengthType Type { get; set; }

        public GridLength()
            :this(1, GridLengthType.Auto)
        {
        }
        public GridLength(double length, GridLengthType type = GridLengthType.Pixel)
        {
            Length = length;
            Type = type;
        }
    }
}