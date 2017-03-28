using System;

namespace Ztk
{
    public class MouseButtonEventArgs : EventArgs
    {
        public MouseButton MouseButton { get; private set; }

        public MouseButtonEventArgs(MouseButton mouseButton)
        {
            MouseButton = mouseButton;
        }
    }
}