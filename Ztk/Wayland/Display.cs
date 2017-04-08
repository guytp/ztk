using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Ztk.Wayland
{
    internal class Display : WaylandObject
    {
        [DllImport("wayland-wrapper", EntryPoint = "wlw_display_connect")]
        private static extern IntPtr DisplayConnect();

        [DllImport("wayland-wrapper", EntryPoint = "wlw_display_disconnect")]
        private static extern void DisplayDisconnect(IntPtr display);

        [DllImport("wayland-wrapper", EntryPoint = "wlw_dispatch")]
        private static extern bool Dispatch(IntPtr display);

        [DllImport("wayland-wrapper", EntryPoint = "wlw_roundtrip")]
        private static extern void Roundtrip(IntPtr display);

        protected override void ReleaseWaylandObject()
        {
            if (Handle != IntPtr.Zero)
            {
                DisplayDisconnect(Handle);
                Handle = IntPtr.Zero;
            }
        }

        public static Display Connect()
        {
            return new Display(DisplayConnect());
        }

        private Display(IntPtr handle)
            : base(handle)
        {
        }

        public bool PerformSingleDispatchLoop()
        {
            return Dispatch(Handle);
        }

        public void DispatchAndRoundtrip()
        {
            Dispatch(Handle);
            Roundtrip(Handle);
        }
    }
}