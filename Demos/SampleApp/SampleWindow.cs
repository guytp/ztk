using System;
using Ztk.Drawing;

namespace Ztk.Demos.SampleApp
{
    public class SampleWindow : Window
    {
        public void OldSampleWindow()
        {
            Background = new SolidColorBrush(new Color(0x2D / 255f, 0x2D / 255f, 0x30 / 255f));
            Border mainBorder = new Border
            {
                BorderThickness = 1,
                BorderBrush = Brushes.DodgerBlue,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            Child = mainBorder;

            Grid mainGrid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            mainBorder.Child = mainGrid;
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridLengthType.Star)));
            mainGrid.RowDefinitions.Add(new RowDefinition(new GridLength(3)));
            mainGrid.RowDefinitions.Add(new RowDefinition(new GridLength(25)));
            mainGrid.RowDefinitions.Add(new RowDefinition(new GridLength(3)));
            mainGrid.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridLengthType.Star)));
            Border fillerBorder = new Border { Background = Brushes.DodgerBlue, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, BorderBrush = null, BorderThickness = 0 };
            mainGrid.Children.Add(fillerBorder);
            mainGrid.SetChildRow(fillerBorder, 0);
            Border fillerBorder2 = new Border { Background = Brushes.DodgerBlue, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, BorderBrush = null, BorderThickness = 0 };
            mainGrid.Children.Add(fillerBorder2);
            mainGrid.SetChildRow(fillerBorder2, 2);
            Grid titleGrid = new Grid
            {
                Background = Brushes.DodgerBlue,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            mainGrid.Children.Add(titleGrid);
            mainGrid.SetChildRow(titleGrid, 1);
            titleGrid.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridLengthType.Star)));
            titleGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(60)));
            titleGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(398)));
            titleGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(60)));
            titleGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(60)));
            titleGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(60)));

            TextBlock windowMenu = new TextBlock
            {
                Text = "  🐱  ",
                FontSize = 22,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Middle,
                HorizontalAlignment = HorizontalAlignment.Middle
            };
            titleGrid.SetChildColumn(windowMenu, 0);
            titleGrid.Children.Add(windowMenu);

            TextBlock title = new TextBlock
            {
                Text = "Sample Application",
                FontSize = 18,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Middle,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            titleGrid.SetChildColumn(title, 1);
            titleGrid.Children.Add(title);


            TextBlock minimiseMenu = new TextBlock
            {
                Text = "  _  ",
                FontSize = 22,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Middle,
                HorizontalAlignment = HorizontalAlignment.Middle
            };
            titleGrid.SetChildColumn(minimiseMenu, 2);
            titleGrid.Children.Add(minimiseMenu);


            TextBlock maximiseMenu = new TextBlock
            {
                Text = "  □  ",
                FontSize = 22,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Middle,
                HorizontalAlignment = HorizontalAlignment.Middle
            };
            titleGrid.SetChildColumn(maximiseMenu, 3);
            titleGrid.Children.Add(maximiseMenu);

            TextBlock closeMenu = new TextBlock
            {
                Text = "  X  ",
                FontSize = 22,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Middle,
                HorizontalAlignment = HorizontalAlignment.Middle
            };
            titleGrid.SetChildColumn(closeMenu, 4);
            titleGrid.Children.Add(closeMenu);

            TextBlock mainContentText = new TextBlock
            {
                FontSize = 24,
                Text = "Main content would go here",
                Foreground = Brushes.LimeGreen,
                HorizontalAlignment = HorizontalAlignment.Middle,
                VerticalAlignment = VerticalAlignment.Middle
            };
            mainGrid.SetChildRow(mainContentText, 3);
            mainGrid.Children.Add(mainContentText);

        }
        public void GridsSampleWindow()
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

            TextBlock tbTop = new TextBlock
            {
                Background = Brushes.Red,
                Foreground = Brushes.White,
                Text = "Top  content"
            };
            mainGrid.Children.Add(tbTop);
            mainGrid.SetChildColumn(tbTop, 0);
            mainGrid.SetChildRow(tbTop, 0);
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

        public SampleWindow()
        {
            MouseLeftButtonDown += OnLeftButtonDown;
            Button btn = new Button
            {
                Content = "This is a test",
                Margin = new FourSidedNumber(15),
                HorizontalAlignment = HorizontalAlignment.Middle,
                VerticalAlignment = VerticalAlignment.Middle
            };
            btn.MouseLeftButtonDown += OnMouseLeftButtonDown;
            btn.MouseLeftButtonUp += OnMouseLeftButtonUp;
            btn.Click += OnClick;
            btn.DoubleClick += OnDoubleClick;
            Border b = new Border
            {
                BorderThickness = 2,
                BorderBrush = Brushes.Red,
                Margin = new FourSidedNumber(10),
                Background = Brushes.Gray,
                Child = btn,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            Child = b;
        }


        private void OnMouseLeftButtonDown(object sender, EventArgs e)
        {
            Console.WriteLine("Mouse down!");
        }
        private void OnMouseLeftButtonUp(object sender, EventArgs e)
        {
            Console.WriteLine("Mouse up!");
        }
        private void OnClick(object sender, EventArgs e)
        {
            Console.WriteLine("Button clicked!");
        }
        private void OnDoubleClick(object sender, EventArgs e)
        {
            Console.WriteLine("Double click!");
        }

        private void OnLeftButtonDown(object sender, EventArgs e)
        {
            DragMove();
        }
    }
}