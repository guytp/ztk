using System;

namespace Ztk.Wayland
{
    internal class Shell : WaylandObject
    { 
        public Shell(IntPtr handle)
            : base(handle)
        {
        }
        

        protected override void ReleaseWaylandObject()
        {
        }
    }
}