using System;

namespace Ztk
{
    public class MouseMoveEventArgs : EventArgs
    {
        public double X { get; private set; }

        public double Y { get; private set; }

        public MouseMoveEventArgs(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}