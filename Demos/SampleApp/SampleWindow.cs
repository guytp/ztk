using System;
using System.Linq;
using Ztk.Drawing;

namespace Ztk.Demos.SampleApp
{
    public class SampleWindow : Window
    {
        private TextBlock _trackerText;
        private TextBlock _inputText;
        private Grid _buttonsGrid;
        private decimal _count = 0;
        private string _lastAction;
        private string _lastOp = "+";

        public SampleWindow()
            : base(new Size(280, 380))
        {

            Background = Brushes.Black;
            Grid mainGrid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            Child = mainGrid;
            mainGrid.KeyPress += OnKeyPress;
            mainGrid.MouseLeftButtonDown += OnLeftButtonDown;
            mainGrid.RowDefinitions.Add(new RowDefinition(new GridLength(30)));
            mainGrid.RowDefinitions.Add(new RowDefinition(new GridLength(45)));
            mainGrid.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridLengthType.Star)));
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridLengthType.Star)));

            _trackerText = new TextBlock
            {
                Foreground = Brushes.LightGray,
                FontSize = 12,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Middle,
                Margin = new FourSidedNumber(5),
                IsFocusable = false
            };
            mainGrid.Children.Add(_trackerText);
            mainGrid.SetChildColumn(_trackerText, 0);
            mainGrid.SetChildRow(_trackerText, 0);
            _trackerText.MouseLeftButtonDown += OnLeftButtonDown;

            _inputText = new TextBlock
            {
                Text = "0",
                Foreground = Brushes.White,
                FontSize = 32,
                FontWeight = FontWeight.Bold,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new FourSidedNumber(5),
                IsFocusable = false
            };
            mainGrid.Children.Add(_inputText);
            mainGrid.SetChildColumn(_inputText, 0);
            mainGrid.SetChildRow(_inputText, 1);
            _inputText.MouseLeftButtonDown += OnLeftButtonDown;

            _buttonsGrid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = new SolidColorBrush(new Color(0x1F / 255f))
            };
            _buttonsGrid.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridLengthType.Star)));
            _buttonsGrid.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridLengthType.Star)));
            _buttonsGrid.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridLengthType.Star)));
            _buttonsGrid.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridLengthType.Star)));
            _buttonsGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridLengthType.Star)));
            _buttonsGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridLengthType.Star)));
            _buttonsGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridLengthType.Star)));
            _buttonsGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridLengthType.Star)));
            _buttonsGrid.KeyPress += OnKeyPress;
            mainGrid.Children.Add(_buttonsGrid);
            mainGrid.SetChildColumn(_buttonsGrid, 0);
            mainGrid.SetChildRow(_buttonsGrid, 2);

            string[] buttons = new[] { "7", "8", "9", "C", "4", "5", "6", "-", "1", "2", "3", "+", "±", "0", ".", "=" };
            int row = 0;
            int column = 0;
            for (int i = 0; i < buttons.Length; i++)
            {
                if (column == 4)
                {
                    row++;
                    column = 0;
                }

                if (!string.IsNullOrEmpty(buttons[i]))
                {

                    Button b = new Button
                    {
                        Content = buttons[i],
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch
                    };
                    _buttonsGrid.Children.Add(b);
                    _buttonsGrid.SetChildColumn(b, column);
                    _buttonsGrid.SetChildRow(b, row);
                    b.Click += CalculatorButtonOnClick;
                    b.KeyPress += OnKeyPress;
                }
                column++;
            }
            KeyPress += OnKeyPress;

        }


        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.Key == KeyboardKey.DUpper || e.Key == KeyboardKey.DLower) && e.KeyboardState.Modifiers.Length == 1 && e.KeyboardState.Modifiers[0] == KeyboardModifier.Control)
                App.CurrentApplication.Shutdown();

            string searchValue = e.Value;
            if (e.Key == KeyboardKey.NumPadEnter || e.Key == KeyboardKey.Return)
                searchValue = "=";
            if (string.IsNullOrEmpty(searchValue))
                return;
            Button b = _buttonsGrid.Children.Cast<Button>().FirstOrDefault(btn => ((string)(btn.Content)) == searchValue);
            if (b != null)
                CalculatorButtonOnClick(b, new EventArgs());
        }


        private void CalculatorButtonOnClick(object sender, EventArgs e)
        {
            string action = ((Button)sender).Content as string;
            decimal number = decimal.Parse(_inputText.Text);
            if (_lastAction != null && _lastAction.Length > 0 && (_lastAction == "+" || _lastAction == "-" || _lastAction == "="))
                _inputText.Text = "0";
            if (action == "C")
            {
                _count = 0;
                _trackerText.Text = string.Empty;
                _inputText.Text = "0";
                _lastOp = "+";
            }
            else if (action == "-" || action == "+")
            {
                if (_lastOp == "-")
                    _count -= number;
                else if (_lastOp == "+")
                    _count += number;
                _trackerText.Text += number + " " + action + " ";
                _inputText.Text = _count.ToString();
                _lastOp = action;
            }
            else if (action == "±")
            {
                if (_inputText.Text == "0")
                    return;
                if (_inputText.Text.Substring(0, 1) == "-")
                    _inputText.Text = _inputText.Text.Substring(1);
                else
                    _inputText.Text = "-" + _inputText.Text;
            }
            else if (action == "=")
            {
                if (_lastOp == "-")
                    _count -= number;
                else if (_lastOp == "+")
                    _count += number;
                _inputText.Text = _count.ToString();
                _trackerText.Text = string.Empty;
                _count = 0;
            }
            else
            {
                if (action == "." && _inputText.Text.Contains("."))
                    return;
                if (_inputText.Text == "0" && action != ".")
                    _inputText.Text = string.Empty;
                _inputText.Text += action;
            }
            _lastAction = action;
        }

        private void OnLeftButtonDown(object sender, EventArgs e)
        {
            DragMove();
        }
    }
}