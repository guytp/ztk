namespace Ztk
{
    /// <summary>
    /// A control that houses one child.
    /// </summary>
    public abstract class ContainerControl : BaseContainerControl
    {
        public Control Child
        {
            get
            {
                return ChildrenInternal.Count < 1 ? null : ChildrenInternal[0];
            }
            set
            {
                Control existingChild = Child;
                if (existingChild != null)
                    RemoveLayoutInformationForChild(existingChild);
                ChildrenInternal.Clear();
                if (value != null)
                    ChildrenInternal.Add(value);
            }
        }
    }
}