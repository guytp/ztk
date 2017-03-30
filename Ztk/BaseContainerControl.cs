using System.Collections.Generic;
using System.Linq;
using Ztk.Drawing;

namespace Ztk
{
    public abstract class BaseContainerControl : Control
    {
        public Brush Background { get; set; }

        protected List<Control> ChildrenInternal { get; private set; }

        private List<LayoutInformation> LayoutInformation { get; set; }

        protected BaseContainerControl()
        {
            LayoutInformation = new List<LayoutInformation>();
            ChildrenInternal = new List<Control>();
        }

        protected LayoutInformation GetLayoutInformationForChild(Control child)
        {
            LayoutInformation layoutInformation = LayoutInformation.FirstOrDefault(li => li.Control == child);
            if (layoutInformation == null)
            {
                layoutInformation = new LayoutInformation(child);
                LayoutInformation.Add(layoutInformation);
            }
            return layoutInformation;
        }

        protected void RemoveLayoutInformationForChild(Control child)
        {
            LayoutInformation layoutInformation = LayoutInformation.FirstOrDefault(li => li.Control == child);
            if (layoutInformation == null)
                return;
            LayoutInformation.Remove(layoutInformation);
        }


        protected Control GetChildAtLocation(double x, double y)
        {
            foreach (LayoutInformation layoutInformation in LayoutInformation.OrderByDescending(li => li.ZIndex))
            {
                Rectangle rectangle = layoutInformation.Rectangle;
                double minX = rectangle.X;
                double minY = rectangle.Y;
                double maxX = minX + rectangle.Width;
                double maxY = minY + rectangle.Height;
                if (x >= minX && x <= maxX && y >= minY && y <= maxY)
                    return layoutInformation.Control;
            }
            return null;
        }

    }
}