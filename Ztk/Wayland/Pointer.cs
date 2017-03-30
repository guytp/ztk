using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Ztk.Wayland
{
    internal class Pointer : WaylandObject
    {
        #region Native Delegates
        private delegate void PointerOnEnterListener(IntPtr data, IntPtr pointer, uint serial, IntPtr surface, int surfaceX, int surfaceY);
        private delegate void PointerOnLeaveListener(IntPtr data, IntPtr pointer, uint serial, IntPtr surface);
        private delegate void PointerOnMotionListener(IntPtr data, IntPtr pointer, uint time, int surfaceX, int surfaceY);
        private delegate void PointerOnButtonListener(IntPtr data, IntPtr pointer, uint serial, uint time, WaylandMouseButton button, uint state);
        private delegate void PointerOnAxisListener(IntPtr data, IntPtr pointer, uint time, uint axis, int value);
        private delegate void PointerOnFrameListener(IntPtr data, IntPtr pointer);
        private delegate void PointerOnAxisSourceListener(IntPtr data, IntPtr pointer, uint axis_source);
        private delegate void PointerOnAxisStopListener(IntPtr data, IntPtr pointer, uint time, uint axis);
        private delegate void PointerOnAxisDiscreteListener(IntPtr data, IntPtr pointer, uint axis, int discrete);
        #endregion

        #region Native Methods
        [DllImport("wayland-wrapper", EntryPoint = "wlw_pointer_add_listener")]
        private static extern void PointerAddListener(IntPtr pointer, PointerOnEnterListener enterListener, PointerOnLeaveListener leaveListener, PointerOnMotionListener motionListener, PointerOnButtonListener buttonListener, PointerOnAxisListener axisListener, PointerOnFrameListener frameListener, PointerOnAxisSourceListener axisSourceListener, PointerOnAxisStopListener axisStopListener, PointerOnAxisDiscreteListener axisDiscreteListener);

        [DllImport("wayland-wrapper", EntryPoint = "wlw_pointer_destroy")]
        private static extern void PointerDestroy(IntPtr pointerHandle);
        [DllImport("wayland-wrapper", EntryPoint = "wlw_fixed_to_double")]
        private static extern double FixedToDouble(int fixedNumber);
        #endregion

        #region Declarations
        private readonly PointerOnEnterListener _enterListener;
        private readonly PointerOnLeaveListener _leaveListener;
        private readonly PointerOnMotionListener _motionListener;
        private readonly PointerOnButtonListener _buttonListener;
        private readonly PointerOnAxisListener _axisListener;
        private readonly PointerOnFrameListener _frameListener;
        private readonly PointerOnAxisSourceListener _axisSourceListener;
        private readonly PointerOnAxisStopListener _axisStopListener;
        private readonly PointerOnAxisDiscreteListener _axisDiscreteListener;
        private IntPtr _currentSurface = IntPtr.Zero;
        private double _currentSurfaceX;
        private double _currentSurfaceY;
        #endregion

        public Pointer(IntPtr handle)
            : base(handle)
        {
            // Store a handle to delegates for native use so that we do not lose a reference to them
            _enterListener = OnEnterListener;
            _leaveListener = OnLeaveListener;
            _motionListener = OnMotionListener;
            _buttonListener = OnButtonListener;
            _axisListener = OnAxisListener;
            _frameListener = OnFrameListener;
            _axisSourceListener = OnAxisSourceListener;
            _axisStopListener = OnAxisStopListener;
            _axisDiscreteListener = OnAxisDiscreteListener;

            // Tie up to our events Wayland-side
            PointerAddListener(handle, _enterListener, _leaveListener, _motionListener, _buttonListener, _axisListener, _frameListener, _axisSourceListener, _axisStopListener, _axisDiscreteListener);
        }


        #region Event Handlers
        private void OnAxisStopListener(IntPtr data, IntPtr pointer, uint time, uint axis)
        {
        }

        private void OnAxisSourceListener(IntPtr data, IntPtr pointer, uint axisSource)
        {
        }

        private void OnFrameListener(IntPtr data, IntPtr pointer)
        {
        }

        private void OnAxisListener(IntPtr data, IntPtr pointer, uint time, uint axis, int value)
        {
        }

        private void OnButtonListener(IntPtr data, IntPtr pointerHandle, uint serial, uint time, WaylandMouseButton button, uint state)
        {
            if (_currentSurface == IntPtr.Zero)
                return;
            Window window = GetWindow(_currentSurface);
            SeatInstance seat = App.CurrentApplication.Registry.Seat.Seats.First(s => s.Pointer == this);
            window.TriggerWaylandPointerButton(new WaylandPointerButtonEventArgs(this, seat, serial, button, state == 1));
        }

        private void OnMotionListener(IntPtr data, IntPtr pointer, uint time, int surfaceX, int surfaceY)
        {
            // Convert points
            double x = FixedToDouble(surfaceX);
            double y = FixedToDouble(surfaceY);

            if (_currentSurface != IntPtr.Zero)
            {
                Window window = GetWindow(_currentSurface);
                window.TriggerWaylandMouseMove(x, y);
            }

            _currentSurfaceX = x;
            _currentSurfaceY = y;
        }

        private void OnAxisDiscreteListener(IntPtr data, IntPtr pointer, uint axis, int discrete)
        {
        }

        private void OnEnterListener(IntPtr data, IntPtr pointer, uint serial, IntPtr surface, int surfaceX, int surfaceY)
        {
            if (_currentSurface != IntPtr.Zero)
            {
                Window oldWindow = GetWindow(_currentSurface);
                oldWindow.TriggerWaylandMouseLeave();
            }

            // Convert points
            double x = FixedToDouble(surfaceX);
            double y = FixedToDouble(surfaceY);

            // Notify new window we have entered
            Window newWindow = GetWindow(surface);
            newWindow.TriggerMouseEnter(x, y);

            // Store new value
            _currentSurface = surface;
            _currentSurfaceX = x;
            _currentSurfaceY = y;
        }

        private void OnLeaveListener(IntPtr data, IntPtr pointer, uint serial, IntPtr surface)
        {
            Window oldWindow = GetWindow(surface);
            oldWindow.TriggerWaylandMouseLeave();
            _currentSurface = IntPtr.Zero;
        }
        #endregion

        protected override void ReleaseWaylandObject()
        {
            if (Handle != IntPtr.Zero)
            {
                PointerDestroy(Handle);
                Handle = IntPtr.Zero;
            }
        }

        private Window GetWindow(IntPtr surfaceHandle)
        {
            return App.CurrentApplication.Registry?.Compositor?.SurfaceForHandle(surfaceHandle)?.RenderTarget as Window;
        }
    }
}