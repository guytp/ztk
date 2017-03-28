namespace Ztk
{
    /// <summary>
    /// A control that houses one child.
    /// </summary>
    public abstract class ContainerControl : Control
    {
        public Control Child { get; set; }

        public Brush Background { get; set; }
    }
}