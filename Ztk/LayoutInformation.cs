using System;
using System.Collections.Generic;
using System.Text;
using Ztk.Drawing;

namespace Ztk
{
    public class LayoutInformation
    {
        public Rectangle Rectangle { get; set; }
        
        public Control Control { get; private set; }

        public int ZIndex { get; set; }

        public LayoutInformation(Control control)
        {
            Control = control;
        }
    }
}