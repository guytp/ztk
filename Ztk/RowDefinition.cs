namespace Ztk
{
    public class RowDefinition
    {
        public GridLength Height { get; set; }
        public double ActualHeight { get; internal set; }

        internal double ActualOffset { get; set; }

        public RowDefinition()
            : this(new GridLength())
        {
        }
        public RowDefinition(GridLength height)
        {
            Height = height;
        }
    }
}