namespace Ztk
{
    public class ColumnDefinition
    {
        public GridLength Width { get; set; }

        public double ActualWidth { get; internal set; }

        internal double ActualOffset { get; set; }

        public ColumnDefinition()
            : this(new GridLength())
        {

        }
        public ColumnDefinition(GridLength width)
        {
            Width = width;
        }
    }
}