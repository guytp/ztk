using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Ztk.Wayland
{
    internal class Seat : WaylandObject
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SeatCapabilitiesListener(IntPtr data, IntPtr seatHandle, SeatCapability seatCapabilities);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private delegate void SeatNameListener(IntPtr data, IntPtr seatHandle, string name);



        [DllImport("wayland-wrapper", EntryPoint = "wlw_seat_add_listener")]
        private static extern void SeatAddListeners(IntPtr seat, SeatCapabilitiesListener seatCapabilitiesListener, SeatNameListener seatNameListener);


        private readonly SeatCapabilitiesListener _seatCapabilitiesListener;

        private readonly SeatNameListener _seatNameListener;

        public List<SeatInstance> Seats { get; private set; }

        public Seat(IntPtr handle)
            : base(handle)
        {
            // Setup app-wide delegates so that we do not lose references to them
            _seatCapabilitiesListener = OnSeatCapabilities;
            _seatNameListener = OnSeatName;

            Seats = new List<SeatInstance>();
            SeatAddListeners(handle, _seatCapabilitiesListener, _seatNameListener);
        }
        protected override void ReleaseWaylandObject()
        {
        }

        private void OnSeatCapabilities(IntPtr data, IntPtr seatHandle, SeatCapability seatCapabilities)
        {
            // Get a handle to, or add, this seat
            SeatInstance seat = Seats.FirstOrDefault(s => s.Handle == seatHandle);
            if (seat == null)
            {
                seat = new SeatInstance(seatHandle);
                Seats.Add(seat);
            }

            seat.UpdateCapabilities(seatCapabilities);
        }

        private void OnSeatName(IntPtr data, IntPtr seatHandle, string name)
        {
            // Get a handle to, or add, this seat
            SeatInstance seat = Seats.FirstOrDefault(s => s.Handle == seatHandle);
            if (seat == null)
            {
                seat = new SeatInstance(seatHandle);
                Seats.Add(seat);
            }

            // Update the name
            seat.Name = name;
        }

    }
}