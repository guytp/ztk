using System.Collections.Generic;

namespace Ztk
{
    /// <summary>
    /// A control that houses more than one child - generally a layout manager.
    /// </summary>
    public abstract class MultiContainerControl : BaseContainerControl
    {
        public List<Control> Children { get { return ChildrenInternal; } }

    }
}