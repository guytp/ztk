using System;
using Ztk.Drawing;

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
                {
                    RemoveLayoutInformationForChild(existingChild);
                    existingChild.Parent = null;
                }
                ChildrenInternal.Clear();
                if (value != null)
                {
                    ChildrenInternal.Add(value);
                    value.Parent = this;
                }
            }
        }

        protected abstract FourSidedNumber InternalSpacing { get; }
        
        public override Size MeasureDesiredSize(Size availableSize)
        {
            // If we have no children this one is easy
            if (Child == null)
                return HorizontalAlignment == HorizontalAlignment.Stretch && VerticalAlignment == VerticalAlignment.Stretch ? availableSize : new Size(0, 0);

            // Determine actual available size for child by taking off our own internal spacings (i.e. borders etc)
            double availableWidth = availableSize.Width - Child.Margin.Left - Child.Margin.Right - InternalSpacing.Left - InternalSpacing.Right;
            double availableHeight = availableSize.Height - Child.Margin.Top - Child.Margin.Bottom - InternalSpacing.Top - InternalSpacing.Bottom;

            // Perform a measure pass on this child and set its size
            Size childSize = Child.MeasureDesiredSize(new Size(availableWidth, availableHeight));
            Child.SetActualSize(childSize);

            // Decide the width we will use
            Size finalSizeToUse = new Size(childSize.Width + InternalSpacing.Left + InternalSpacing.Right + Child.Margin.Left + Child.Margin.Right, childSize.Height + InternalSpacing.Top + InternalSpacing.Bottom + Child.Margin.Top + Child.Margin.Bottom);
            if (HorizontalAlignment == HorizontalAlignment.Stretch)
                finalSizeToUse.Width = availableSize.Width;
            if (VerticalAlignment == VerticalAlignment.Stretch)
                finalSizeToUse.Height = availableSize.Height;

            // Now we have the desired sizing information we can setup its layout information for painting later on
            double x;
            switch (Child.HorizontalAlignment)
            {
                case HorizontalAlignment.Stretch:
                case HorizontalAlignment.Left:
                    x = Child.Margin.Left + InternalSpacing.Left;
                    break;
                case HorizontalAlignment.Right:
                    x = finalSizeToUse.Width - Child.Margin.Right - childSize.Width - InternalSpacing.Right;
                    break;
                case HorizontalAlignment.Middle:
                    x = (finalSizeToUse.Width / 2) - ((Child.Margin.Left + Child.Margin.Right + childSize.Width + InternalSpacing.Left + InternalSpacing.Right) / 2);
                    break;
                default:
                    throw new Exception("Unsupported horizontal alignment");
            }
            if (x < 0)
                x = 0;
            if (x > finalSizeToUse.Width)
                x = finalSizeToUse.Width;
            double y;
            switch (Child.VerticalAlignment)
            {
                case VerticalAlignment.Stretch:
                case VerticalAlignment.Top:
                    y = Child.Margin.Top + InternalSpacing.Top;
                    break;
                case VerticalAlignment.Bottom:
                    y = finalSizeToUse.Height - Child.Margin.Bottom - childSize.Height - InternalSpacing.Bottom;
                    break;
                case VerticalAlignment.Middle:
                    y = (finalSizeToUse.Height / 2) - ((Child.Margin.Top + Child.Margin.Bottom + childSize.Height + InternalSpacing.Top + InternalSpacing.Bottom) / 2);
                    break;
                default:
                    throw new Exception("Unsupported vertical alignment");
            }
            if (y < 0)
                y = 0;
            if (y > finalSizeToUse.Height)
                y = finalSizeToUse.Height;
            LayoutInformation layoutInformation = GetLayoutInformationForChild(Child);
            layoutInformation.Rectangle = new Rectangle(x, y, childSize.Width, childSize.Height);
            layoutInformation.ZIndex = 0;

            // Return size including our borders and child margins factoring in any stretch
            return finalSizeToUse;
        }
    }
}