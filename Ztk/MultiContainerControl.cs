using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Ztk
{
    /// <summary>
    /// A control that houses more than one child - generally a layout manager.
    /// </summary>
    public abstract class MultiContainerControl : BaseContainerControl
    {
        public ObservableCollection<Control> Children { get; private set; }

        public MultiContainerControl()
        {
            Children = new ObservableCollection<Control>();
            Children.CollectionChanged += ChildrenOnCollectionChanged;
        }

        private void ChildrenOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (Control child in e.NewItems)
                {
                    ChildrenInternal.Add(child);
                    child.Parent = this;
                }
            if (e.OldItems != null)
                foreach (Control child in e.OldItems)
                {
                    ChildrenInternal.Remove(child);
                    RemoveLayoutInformationForChild(child);
                    child.Parent = this;
                }
        }
    }
}