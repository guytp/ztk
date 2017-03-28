using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Ztk
{
    /// <summary>
    /// Represents a wayland seat.
    /// </summary>
    internal class Seat : WaylandObject
    {

        private Pointer _pointer;

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

        public List<SeatCapability> Capabilities { get; private set; }

        public Seat(IntPtr handle)
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
                Pointer = new Pointer(WaylandWrapperInterop.SeatPointerGet(Handle));

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
        }

        protected override void ReleaseWaylandObject()
        {
        }
    }
}