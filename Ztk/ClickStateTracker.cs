using System;

namespace Ztk
{
    public class ClickStateTracker
    {
        private const double DoubleClickTime = 1.5;

        private DateTime? _lastClick;

        private bool _isEntered;

        private bool _isDown;

        public event EventHandler Click;

        public event EventHandler DoubleClick;

        public ClickStateTracker(Control control)
        {
            control.MouseLeftButtonDown += OnControlLeftDown;
            control.MouseLeftButtonUp += OnControlLeftUp;
            control.MouseLeave += OnControlLeave;
            control.MouseEnter += OnControlEnter;
        }

        private void OnControlEnter(object sender, EventArgs e)
        {
            // Reset state when we come in
            _isEntered = true;
            _lastClick = null;
            _isDown = false;
        }

        private void OnControlLeave(object sender, EventArgs e)
        {
            _isEntered = false;
        }

        private void OnControlLeftUp(object sender, EventArgs e)
        {
            // Do nothing if state isn't valid
            if (!_isEntered || !_isDown)
                return;

            // We're in here and down so this is a click or a double click depending on duration
            bool isDoubleClick = _lastClick != null && DateTime.UtcNow.Subtract(_lastClick.Value).TotalSeconds < DoubleClickTime;
            if (isDoubleClick)
                DoubleClick?.Invoke(this, new EventArgs());
            else
                Click?.Invoke(this, new EventArgs());

            // Mark as no longer down
            _isDown = false;
            _lastClick = isDoubleClick ? null : (DateTime?)DateTime.UtcNow;
        }

        private void OnControlLeftDown(object sender, EventArgs e)
        {
            // Do nothing if state isn't valid
            if (!_isEntered || _isDown)
                return;

            // Record this as a down
            _isDown = true;
        }
    }
}