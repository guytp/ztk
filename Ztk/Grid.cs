using Ztk.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ztk
{
    public class Grid : MultiContainerControl
    {
        private List<GridChildInformation> _childInformation = new List<GridChildInformation>();
        // TODO: Empty this as things get removed from Children (observable collection)

        public List<ColumnDefinition> ColumnDefinitions { get; private set; }

        public List<RowDefinition> RowDefinitions { get; private set; }

        private readonly List<KeyValuePair<Rectangle, Control>> _childRectangles = new List<KeyValuePair<Rectangle, Control>>();

        public Grid()
        {
            ColumnDefinitions = new List<ColumnDefinition>();
            RowDefinitions = new List<RowDefinition>();
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
        }

        public override Size MeasureDesiredSize(Size availableSize)
        {
            // Cleanse our child information first
            ConsolidateChildInformation();

            // First step let's sum up all of our row/column constant and star height/widths.  Whilst we're 
            // doing this we we can set the actual widths/heights of constantly defined height columns and rows.
            double heightConstants = 0;
            double heightStars = 0;
            double widthConstants = 0;
            double widthStars = 0;
            for (int i = 0; i < (RowDefinitions.Count < 1 ? 1 : RowDefinitions.Count); i++)
            {
                RowDefinition rd = GetRow(i);
                if (rd.Height.Type == GridLengthType.Pixel)
                {
                    double originalHeightConstants = heightConstants;
                    heightConstants += rd.Height.Length;
                    if (heightConstants <= availableSize.Height)
                        rd.ActualHeight = rd.Height.Length;
                    else if (originalHeightConstants <= availableSize.Height)
                        rd.ActualHeight = heightConstants - originalHeightConstants;
                    else
                        rd.ActualHeight = 0;
                }
                else
                {
                    rd.ActualHeight = double.NaN;
                    if (rd.Height.Type == GridLengthType.Star)
                        heightStars += rd.Height.Length;
                }
            }
            for (int i = 0; i < (ColumnDefinitions.Count < 1 ? 1 : ColumnDefinitions.Count); i++)
            {
                ColumnDefinition cd = GetColumn(i);
                if (cd.Width.Type == GridLengthType.Pixel)
                {
                    double originalWidthConstants = widthConstants;
                    widthConstants += cd.Width.Length;
                    if (widthConstants <= availableSize.Width)
                        cd.ActualWidth = cd.Width.Length;
                    else if (originalWidthConstants <= availableSize.Width)
                        cd.ActualWidth = widthConstants - originalWidthConstants;
                    else
                        cd.ActualWidth = 0;
                }
                else
                {
                    cd.ActualWidth = double.NaN;
                    if (cd.Width.Type == GridLengthType.Star)
                        widthStars += cd.Width.Length;
                }
            }

            // Work out height/widths for autos and treat stars the same
            double remainingHeight = availableSize.Height - heightConstants;
            double remainingWidth = availableSize.Width - widthConstants;
            for (int rowNumber = 0; rowNumber < (RowDefinitions.Count < 1 ? 1 : RowDefinitions.Count); rowNumber++)
            {
                RowDefinition rd = GetRow(rowNumber);
                for (int columnNumber = 0; columnNumber < (ColumnDefinitions.Count < 1 ? 1 : ColumnDefinitions.Count); columnNumber++)
                {
                    ColumnDefinition cd = GetColumn(columnNumber);

                    // Skip this cell if both RD/CD are constant
                    if (rd.Height.Type == GridLengthType.Pixel && cd.Width.Type == GridLengthType.Pixel)
                        continue;

                    // Skip this cell if no children
                    Control[] cellChildren = _childInformation.Where(ci => ci.Row == rowNumber && ci.Column == columnNumber).Select(ci => ci.Child).ToArray();
                    if (cellChildren.Length < 1)
                        continue;

                    // Determine available size for this cell (factoring in any pre-calculated dimensions)
                    Size cellAvailableSize = new Size(remainingWidth, remainingHeight);
                    if (cd.Width.Type == GridLengthType.Pixel)
                        cellAvailableSize.Width = cd.ActualWidth;
                    else if (!double.IsNaN(cd.ActualWidth))
                        cellAvailableSize.Width += cd.ActualWidth;
                    if (rd.Height.Type == GridLengthType.Pixel)
                        cellAvailableSize.Height = rd.ActualHeight;
                    else if (!double.IsNaN(rd.ActualHeight))
                        cellAvailableSize.Height += rd.ActualHeight;

                    // Now for this particular cell ask each child to measure and find biggest dimensions
                    Size biggestCellChildSize = new Size(0, 0);
                    foreach (Control cellChild in cellChildren)
                    {
                        Size cellChildSize = cellChild.MeasureDesiredSize(cellAvailableSize);
                        if (cellChildSize.Width > biggestCellChildSize.Width)
                            biggestCellChildSize.Width = cellChildSize.Width;
                        if (cellChildSize.Height > biggestCellChildSize.Height)
                            biggestCellChildSize.Height = cellChildSize.Height;
                    }

                    // Update the row/column definitions to have actual height/width if currently and determine offset here from already calculated and
                    // update the remaining width/height accordingly
                    if (cd.Width.Type != GridLengthType.Pixel && (double.IsNaN(cd.ActualWidth) || biggestCellChildSize.Width > cd.ActualWidth))
                    {
                        double originalWidth = double.IsNaN(cd.ActualWidth) ? 0 : cd.ActualWidth;
                        cd.ActualWidth = biggestCellChildSize.Width;
                        double widthDifference = cd.ActualWidth - originalWidth;
                        remainingWidth -= widthDifference;
                    }
                    if (rd.Height.Type != GridLengthType.Pixel && (double.IsNaN(rd.ActualHeight) || biggestCellChildSize.Height > rd.ActualHeight))
                    {
                        double originalHeight = double.IsNaN(rd.ActualHeight) ? 0 : rd.ActualHeight;
                        rd.ActualHeight = biggestCellChildSize.Height;
                        double HeightDifference = rd.ActualHeight - originalHeight;
                        remainingHeight -= HeightDifference;
                    }
                }
            }

            // Now distribute stars accordingly - currently these cells will have been fit by auto.  We should now look at all remaining space and divvy it up
            // accordingly for star cells
            double widthPerStar = remainingWidth / widthStars;
            double heightPerStar = remainingHeight / heightStars;
            for (int rowNumber = 0; rowNumber < (RowDefinitions.Count < 1 ? 1 : RowDefinitions.Count); rowNumber++)
            {
                RowDefinition rd = GetRow(rowNumber);
                if (rd.Height.Type != GridLengthType.Star)
                    continue;
                double starSize = heightPerStar * rd.Height.Length;
                if (double.IsNaN(rd.ActualHeight) || starSize > rd.ActualHeight)
                    rd.ActualHeight = starSize;
            }
            for (int columnNumber = 0; columnNumber < (ColumnDefinitions.Count < 1 ? 1 : ColumnDefinitions.Count); columnNumber++)
            {
                ColumnDefinition cd = GetColumn(columnNumber);
                if (cd.Width.Type != GridLengthType.Star)
                    continue;
                double starSize = widthPerStar * cd.Width.Length;
                if (double.IsNaN(cd.ActualWidth) || starSize > cd.ActualWidth)
                    cd.ActualWidth = starSize;
            }

            // All actual height/widths for columsn and rows are calculated so lets store our summary of these to use in render
            double offsetRow = 0;
            double offsetColumn = 0;
            for (int rowNumber = 0; rowNumber < (RowDefinitions.Count < 1 ? 1 : RowDefinitions.Count); rowNumber++)
            {
                RowDefinition rd = GetRow(rowNumber);
                rd.ActualOffset = offsetRow;
                offsetRow += rd.ActualHeight;
            }
            for (int columnNumber = 0; columnNumber < (ColumnDefinitions.Count < 1 ? 1 : ColumnDefinitions.Count); columnNumber++)
            {
                ColumnDefinition cd = GetColumn(columnNumber);
                cd.ActualOffset = offsetColumn;
                offsetColumn += cd.ActualWidth;
            }

            // For each child we can now put in widths/heights as required - do this in to their Measure() and set actual size
            for (int rowNumber = 0; rowNumber < (RowDefinitions.Count < 1 ? 1 : RowDefinitions.Count); rowNumber++)
            {
                RowDefinition rd = GetRow(rowNumber);
                for (int columnNumber = 0; columnNumber < (ColumnDefinitions.Count < 1 ? 1 : ColumnDefinitions.Count); columnNumber++)
                {
                    ColumnDefinition cd = GetColumn(columnNumber);

                    // Tell the child its actual size that we'll be using
                    Control[] cellChildren = _childInformation.Where(ci => ci.Row == rowNumber && ci.Column == columnNumber).Select(ci => ci.Child).ToArray();
                    foreach (Control child in cellChildren)
                    {
                        Size childActualSize = child.MeasureDesiredSize(new Size(cd.ActualWidth, rd.ActualHeight));
                        if (child.HorizontalAlignment == HorizontalAlignment.Stretch)
                            childActualSize.Width = cd.ActualWidth;
                        if (child.VerticalAlignment == VerticalAlignment.Stretch)
                            childActualSize.Height = rd.ActualHeight;
                        child.SetActualSize(childActualSize);
                    }
                }
            }

            // Finally we return the size we will be using
            return new Size(availableSize.Width - remainingWidth, availableSize.Height - remainingHeight);
        }

        public override void Render(GraphicsContext g)
        {
            // Cleanse our child information first
            ConsolidateChildInformation();

            // Paint our background
            PaintBackground(g, Background);

            // Clear our child rectangles out
            _childRectangles.Clear();

            // Return if we have no children
            if (Children.Count < 1)
                return;

            // Paint each child at appropriate location
            foreach (Control child in Children)
            {
                GridChildInformation gci = GetChildInformation(child);
                ColumnDefinition cd = GetColumn(gci.Column);
                RowDefinition rd = GetRow(gci.Row);

                using (ImageSurface childSurface = new ImageSurface(Format.ARGB32, (int)Math.Round(child.ActualWidth), (int)Math.Round(child.ActualHeight)))
                {
                    using (GraphicsContext childContext = new GraphicsContext(childSurface))
                    {
                        double x;
                        switch (child.HorizontalAlignment)
                        {
                            case HorizontalAlignment.Stretch:
                            case HorizontalAlignment.Left:
                                x = child.Margin.Left;
                                break;
                            case HorizontalAlignment.Right:
                                x = cd.ActualWidth - child.Margin.Right - child.ActualWidth;
                                break;
                            case HorizontalAlignment.Middle:
                                x = (cd.ActualWidth / 2) - ((child.Margin.Left + child.Margin.Right + child.ActualWidth) / 2);
                                break;
                            default:
                                throw new Exception("Unsupported horizontal alignment");
                        }
                        if (x < 0)
                            x = 0;
                        if (x > cd.ActualWidth)
                            x = cd.ActualWidth;
                        x += cd.ActualOffset;
                        double y;
                        switch (child.VerticalAlignment)
                        {
                            case VerticalAlignment.Stretch:
                            case VerticalAlignment.Top:
                                y = child.Margin.Top;
                                break;
                            case VerticalAlignment.Bottom:
                                y = rd.ActualHeight - child.Margin.Bottom - child.ActualHeight;
                                break;
                            case VerticalAlignment.Middle:
                                y = (rd.ActualHeight / 2) - ((child.Margin.Top + child.Margin.Bottom + child.ActualHeight) / 2);
                                break;
                            default:
                                throw new Exception("Unsupported vertical alignment");
                        }

                        if (y < 0)
                            y = 0;
                        if (y > rd.ActualHeight)
                            y = rd.ActualHeight;
                        y += rd.ActualOffset;
                        child.Render(childContext);
                        g.SetSourceSurface(childSurface, (int)Math.Round(x), (int)Math.Round(y));
                        g.PaintWithAlpha(child.Opacity);
                        _childRectangles.Add(new KeyValuePair<Rectangle, Control>(new Rectangle(x, y, child.ActualWidth, child.ActualHeight), child));
                    }
                }
            }
        }

        private RowDefinition GetRow(int row)
        {
            if (RowDefinitions.Count < 1)
                return new RowDefinition();
            int index = row;
            if (index < 0)
                index = 0;
            if (index >= RowDefinitions.Count)
                index = RowDefinitions.Count - 1;
            return RowDefinitions[index];
        }

        private ColumnDefinition GetColumn(int column)
        {
            if (ColumnDefinitions.Count < 1)
                return new ColumnDefinition();
            int index = column;
            if (index < 0)
                index = 0;
            if (index >= ColumnDefinitions.Count)
                index = ColumnDefinitions.Count - 1;
            return ColumnDefinitions[index];
        }

        public void SetChildRow(Control child, int row)
        {
            GetChildInformation(child).Row = row;
        }

        public void SetChildColumn(Control child, int column)
        {
            GetChildInformation(child).Column = column;
        }

        private void ConsolidateChildInformation()
        {
            GridChildInformation[] toRemove = _childInformation.Where(ci => !Children.Contains(ci.Child)).ToArray();
            foreach (GridChildInformation gci in toRemove)
                _childInformation.Remove(gci);
            foreach (Control child in Children.Where(c => !_childInformation.Any(gci => gci.Child == c)))
                _childInformation.Add(new GridChildInformation(child));
        }
        private GridChildInformation GetChildInformation(Control child)
        {
            GridChildInformation childInformation = _childInformation.FirstOrDefault(ci => ci.Child == child);
            if (childInformation == null)
            {
                childInformation = new GridChildInformation(child);
                _childInformation.Add(childInformation);
            }
            return childInformation;
        }

        // Re-implement detection of what child rectangles are here, etc.
    }
}
