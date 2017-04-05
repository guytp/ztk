using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Ztk.Wayland
{
    /// <summary>
    /// Represents a wayland seat.
    /// </summary>
    internal class SeatInstance : WaylandObject
    {

        [DllImport("wayland-wrapper", EntryPoint = "wlw_seat_get_pointer")]
        private static extern IntPtr SeatPointerGet(IntPtr seatHandle);

        [DllImport("wayland-wrapper", EntryPoint = "wlw_seat_get_keyboard")]
        private static extern IntPtr SeatKeyboardGet(IntPtr seatHandle);

        private Pointer _pointer;
        private Keyboard _keyboard;

        public string Name
        {
            get; set;
        }

        public Pointer Pointer
        {
            get
            {
                return _pointer;
            }
            private set
            {
                _pointer?.Dispose();
                _pointer = value;
            }
        }
        public Keyboard Keyboard
        {
            get
            {
                return _keyboard;
            }
            private set
            {
                _keyboard?.Dispose();
                _keyboard = value;
            }
        }

        public List<SeatCapability> Capabilities { get; private set; }

        public SeatInstance(IntPtr handle)
            : base(handle)
        {
            Capabilities = new List<SeatCapability>();
        }

        internal void UpdateCapabilities(SeatCapability seatCapabilities)
        {
            // Determine newly added capabilities - if pointer is added let's grab a handle to it
            SeatCapability[] allFlags = new SeatCapability[] { SeatCapability.Keyboard, SeatCapability.Pointer, SeatCapability.Touch };
            List<SeatCapability> newFlags = new List<SeatCapability>();
            foreach (SeatCapability flag in allFlags)
                if (seatCapabilities.HasFlag(flag) && !Capabilities.Contains(flag))
                {
                    Capabilities.Add(flag);
                    newFlags.Add(flag);
                }
            if (newFlags.Contains(SeatCapability.Pointer))
                Pointer = new Pointer(SeatPointerGet(Handle));
            if (newFlags.Contains(SeatCapability.Keyboard))
                Keyboard = new Keyboard(SeatKeyboardGet(Handle));

            // Determine removed capabilities - if pointer is removed we must dispose of it
            List<SeatCapability> removedFlags = new List<SeatCapability>();
            foreach (SeatCapability flag in allFlags)
                if (!seatCapabilities.HasFlag(flag) && Capabilities.Contains(flag))
                {
                    Capabilities.Remove(flag);
                    removedFlags.Add(flag);
                }
            if (removedFlags.Contains(SeatCapability.Pointer))
                Pointer = null;
            if (removedFlags.Contains(SeatCapability.Keyboard))
                Keyboard = null;
        }

        protected override void ReleaseWaylandObject()
        {
        }
    }
}