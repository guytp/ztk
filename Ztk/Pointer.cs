using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Ztk
{
    internal class Pointer : WaylandObject
    {
        #region Native Delegates
        private delegate void PointerOnEnterListener(IntPtr data, IntPtr pointer, uint serial, IntPtr surface, int surfaceX, int surfaceY);
        private delegate void PointerOnLeaveListener(IntPtr data, IntPtr pointer, uint serial, IntPtr surface);
        private delegate void PointerOnMotionListener(IntPtr data, IntPtr pointer, uint time, int surfaceX, int surfaceY);
        private delegate void PointerOnButtonListener(IntPtr data, IntPtr pointer, uint serial, uint time, MouseButton button, uint state);
        private delegate void PointerOnAxisListener(IntPtr data, IntPtr pointer, uint time, uint axis, int value);
        private delegate void PointerOnFrameListener(IntPtr data, IntPtr pointer);
        private delegate void PointerOnAxisSourceListener(IntPtr data, IntPtr pointer, uint axis_source);
        private delegate void PointerOnAxisStopListener(IntPtr data, IntPtr pointer, uint time, uint axis);
        private delegate void PointerOnAxisDiscreteListener(IntPtr data, IntPtr pointer, uint axis, int discrete);
        #endregion

        #region Native Methods
        [DllImport("wayland-wrapper", EntryPoint = "wlw_pointer_add_listener")]
        private static extern void PointerAddListener(IntPtr pointer, PointerOnEnterListener enterListener, PointerOnLeaveListener leaveListener, PointerOnMotionListener motionListener, PointerOnButtonListener buttonListener, PointerOnAxisListener axisListener, PointerOnFrameListener frameListener, PointerOnAxisSourceListener axisSourceListener, PointerOnAxisStopListener axisStopListener, PointerOnAxisDiscreteListener axisDiscreteListener);
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

        #region Properties
        public uint Serial { get; private set; }
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

        private void OnButtonListener(IntPtr data, IntPtr pointer, uint serial, uint time, MouseButton button, uint state)
        {
            if (_currentSurface == IntPtr.Zero)
                return;
            Serial = serial;
            Window window = App.CurrentApplication.GetWindow(_currentSurface);
            window.TriggerMouseButton(button, state == 1, this, App.CurrentApplication.Seats.First(s => s.Pointer == this));
        }

        private void OnMotionListener(IntPtr data, IntPtr pointer, uint time, int surfaceX, int surfaceY)
        {
            // Convert points
            double x = WaylandWrapperInterop.FixedToDouble(surfaceX);
            double y = WaylandWrapperInterop.FixedToDouble(surfaceY);

            if (_currentSurface != IntPtr.Zero)
            {
                Window window = App.CurrentApplication.GetWindow(_currentSurface);
                window.TriggerMouseMove(x, y);
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
                Window oldWindow = App.CurrentApplication.GetWindow(_currentSurface);
                oldWindow.TriggerMouseLeave();
            }

            // Convert points
            double x = WaylandWrapperInterop.FixedToDouble(surfaceX);
            double y = WaylandWrapperInterop.FixedToDouble(surfaceY);

            // Notify new window we have entered
            Window newWindow = App.CurrentApplication.GetWindow(surface);
            newWindow.TriggerMouseEnter(x, y);

            // Store new value
            _currentSurface = surface;
            _currentSurfaceX = x;
            _currentSurfaceY = y;
            Serial = serial;
        }

        private void OnLeaveListener(IntPtr data, IntPtr pointer, uint serial, IntPtr surface)
        {
            Window oldWindow = App.CurrentApplication.GetWindow(surface);
            oldWindow.TriggerMouseLeave();
            _currentSurface = IntPtr.Zero;
            Serial = serial;
        }
        #endregion

        protected override void ReleaseWaylandObject()
        {
            if (Handle != IntPtr.Zero)
            {
                WaylandWrapperInterop.PointerDestroy(Handle);
                Handle = IntPtr.Zero;
            }
        }
    }
}