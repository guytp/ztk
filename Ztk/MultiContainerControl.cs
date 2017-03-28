using System.Collections.Generic;

namespace Ztk
{
    /// <summary>
    /// A control that houses more than one child - generally a layout manager.
    /// </summary>
    public abstract class MultiContainerControl : Control
    {
        public List<Control> Children { get; private set; }

        public Brush Background { get; set; }

        public MultiContainerControl()
        {
            Children = new List<Control>();
        }
    }
}