using System;
using System.Collections.Generic;
using System.Text;

namespace Ztk
{
    internal class GridChildInformation
    {
        public Control Child { get; private set; }

        public int Row { get; set; }

        public int Column { get; set; }

        public GridChildInformation(Control child)
        {
            Child = child;
        }
    }
}