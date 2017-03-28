using System;
using Ztk.Drawing;

namespace Ztk.Demos.SampleApp
{
    public class SampleWindow : Window
    {
        public SampleWindow()
        {
            Background = new SolidColorBrush(new Color(0, 1, 0, 0.5));
            MouseLeftButtonDown += OnLeftButtonDown;

            Grid mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridLengthType.Star)));
            mainGrid.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridLengthType.Star)));
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridLengthType.Star)));
            mainGrid.Margin = new FourSidedNumber(5);
            mainGrid.Background = Brushes.RosyBrown;
            Child = mainGrid;

            /*
            Grid gridTop = new Grid();
            mainGrid.Children.Add(gridTop);
            mainGrid.SetChildRow(gridTop, 0);
            gridTop.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridLengthType.Star)));
            gridTop.RowDefinitions.Add(new RowDefinition(new GridLength()));
            gridTop.RowDefinitions.Add(new RowDefinition(new GridLength()));
            gridTop.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridLengthType.Star)));
            gridTop.ColumnDefinitions.Add(new ColumnDefinition(new GridLength()));
            gridTop.HorizontalAlignment = HorizontalAlignment.Stretch;
            gridTop.VerticalAlignment = VerticalAlignment.Stretch;
            gridTop.Background = Brushes.OrangeRed;
            gridTop.Margin = new FourSidedNumber(20);
            */

            TextBlock tbTop = new TextBlock
            {
                Background = Brushes.Red,
                Foreground = Brushes.White,
                Text = "Top  content"
            };
            mainGrid.Children.Add(tbTop);
            mainGrid.SetChildColumn(tbTop, 0);
            mainGrid.SetChildRow(tbTop, 0);
            /*
            Random rand = new Random();
            for (int row = 0; row < gridTop.RowDefinitions.Count; row++)
                for (int column = 0; column < gridTop.ColumnDefinitions.Count; column++)
                {
                    TextBlock textBlock = new TextBlock
                    {
                        Text = "R" + row + "C" + column,
                        FontSize = 32,
                        Margin = new FourSidedNumber(5),
                        Foreground = Brushes.DodgerBlue,
                        Background = new SolidColorBrush(new Ztk.Drawing.Color(rand.NextDouble(), rand.NextDouble(), rand.NextDouble())),
                        Padding = new FourSidedNumber(5),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch
                    };
                    gridTop.Children.Add(textBlock);
                    gridTop.SetChildRow(textBlock, row);
                    gridTop.SetChildColumn(textBlock, column);
                }
                */

            // Now add a border in bottom half of screen
            Border border = new Border
            {
                Background = Brushes.Yellow,
                CornerRadius = 10,
                BorderThickness = 1.5,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            };
            mainGrid.Children.Add(border);
            mainGrid.SetChildRow(border, 1);
            TextBlock tb = new TextBlock
            {
                Text = "TB in border in pink grid",
                FontSize = 24,
                Padding = new FourSidedNumber(5),
                Background = Brushes.LimeGreen,
                Foreground = Brushes.DodgerBlue,
                Margin = new FourSidedNumber(5),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            border.Child = tb;
        }

        private void OnLeftButtonDown(object sender, EventArgs e)
        {
            DragMove();
        }
    }
}