using System;
using System.Collections.Generic;
using System.Text;

namespace Ztk
{
    public struct FourSidedNumber
    {
        public double Left { get; set; }
        public double Top { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }

        public FourSidedNumber(double uniform)
            : this(uniform, uniform, uniform, uniform)
        {
        }

        public FourSidedNumber(double left, double top, double right, double bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }
}
