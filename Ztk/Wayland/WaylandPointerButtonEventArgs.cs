﻿namespace Ztk.Wayland
{
    internal class WaylandPointerButtonEventArgs
    {
        public Pointer Pointer { get; private set; }

        public SeatInstance Seat { get; private set; }

        public uint Serial { get; private set; }

        public WaylandMouseButton MouseButton { get; private set; }

        public bool IsButtonDown { get; private set; }

        public WaylandPointerButtonEventArgs(Pointer pointer, SeatInstance seat, uint serial, WaylandMouseButton mouseButton, bool isButtonDown)
        {
            Pointer = pointer;
            Seat = seat;
            Serial = serial;
            MouseButton = mouseButton;
            IsButtonDown = isButtonDown;
        }
    }
}