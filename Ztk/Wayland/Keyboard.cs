using System;
using System.Runtime.InteropServices;

namespace Ztk.Wayland
{
    internal class Keyboard : WaylandObject
    {
        #region Native Delegates
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void KeyboardOnKeymapListener(IntPtr data, IntPtr handle, KeyboardMapFormat format, int fileDescriptor, uint size);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void KeyboardOnEnterListener(IntPtr data, IntPtr handle, uint serial, IntPtr surfaceHandle, IntPtr keysPointer);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void KeyboardOnLeaveListener(IntPtr data, IntPtr handle, uint serial, IntPtr surfaceHandle);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void KeyboardOnKeyListener(IntPtr data, IntPtr handle, uint serial, uint time, uint key, KeyState state);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void KeyboardOnModifiersListener(IntPtr data, IntPtr handle, uint serial, uint modsDepressed, uint modsLatched, uint modsLocked, uint group);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void KeyboardOnRepeatInfoListener(IntPtr data, IntPtr handle, int rate, int delay);
        #endregion

        #region Native Methods
        [DllImport("wayland-wrapper", EntryPoint = "wlw_keyboard_add_listener")]
        private static extern void KeyboardAddListener(IntPtr pointer, KeyboardOnKeymapListener keymapListener, KeyboardOnEnterListener enterListener, KeyboardOnLeaveListener leaveListener, KeyboardOnKeyListener keyListener, KeyboardOnModifiersListener modifiersListener, KeyboardOnRepeatInfoListener repeatInfoListener);

        [DllImport("wayland-wrapper", EntryPoint = "wlw_keyboard_destroy")]
        private static extern void KeyboardDestroy(IntPtr pointerHandle);
        #endregion

        #region Declarations
        private readonly KeyboardOnKeymapListener _keymapListener;
        private readonly KeyboardOnEnterListener _enterListener;
        private readonly KeyboardOnLeaveListener _leaveListener;
        private readonly KeyboardOnKeyListener _keyListener;
        private readonly KeyboardOnModifiersListener _modifiersListener;
        private readonly KeyboardOnRepeatInfoListener _repeatInfoListener;
        private XkbMap _xkbMap;
        #endregion

        public Keyboard(IntPtr handle)
            : base(handle)
        {
            // Store a handle to delegates for native use so that we do not lose a reference to them
            _keymapListener = OnKeymapListener;
            _enterListener = OnEnterListener;
            _leaveListener = OnLeaveListener;
            _keyListener = OnKeyListener;
            _modifiersListener = OnModifiersListener;
            _repeatInfoListener = OnRepeatInfoListener;

            // Tie up to our events Wayland-side
            KeyboardAddListener(handle, _keymapListener, _enterListener, _leaveListener, _keyListener, _modifiersListener, _repeatInfoListener);
        }

        #region Event Handlers
        private void OnRepeatInfoListener(IntPtr data, IntPtr handle, int rate, int delay)
        {
            Console.WriteLine("Keyboard:: Repeat rate " + rate + " with delay " + delay);
        }

        private void OnModifiersListener(IntPtr data, IntPtr handle, uint serial, uint modsDepressed, uint modsLatched, uint modsLocked, uint group)
        {
            // Track these separately withe down/up events
        }

        private void OnKeyListener(IntPtr data, IntPtr handle, uint serial, uint time, uint key, KeyState state)
        {
            // First, update tracking
            XkbKeyPressData keyData;
            if (state == KeyState.Pressed)
                keyData = _xkbMap.TrackKeyDown(key);
            else
                keyData = _xkbMap.TrackKeyUp(key);
            Console.WriteLine("Key " + state + ": " + keyData.Key + " = " + keyData.Value);

            // Now get a copy of the current state
            KeyboardState keyboardState = _xkbMap.GetState();
            Console.WriteLine(keyboardState.ToString() + Environment.NewLine + "------------------------------------------------------------------------------");

            // TODO: Propogate this as KeyDown(State, Pressed, CharacterPressed) or KeyUp(State, Released, CharacterReleased)
        }

        private void OnLeaveListener(IntPtr data, IntPtr handle, uint serial, IntPtr surfaceHandle)
        {
            // TODO: Propogate this as a loose focus
        }

        private void OnEnterListener(IntPtr data, IntPtr handle, uint serial, IntPtr surfaceHandle, IntPtr keysPointer)
        {
            // Reset our state to be fresh
            _xkbMap?.ResetState();

            // TODO: Propogate this as a focus
        }

        private void OnKeymapListener(IntPtr data, IntPtr handle, KeyboardMapFormat format, int fileDescriptor, uint size)
        {
            if (format != KeyboardMapFormat.LibXkbV1)
                throw new Exception("Unsupported keymap format, keyboard input will not work.");
            if (size < 1)
                throw new Exception("Keymap has 0 length, keyboard input will not work.");
            _xkbMap?.Dispose();
            _xkbMap = new XkbMap(fileDescriptor, size);
        }
        #endregion

        protected override void ReleaseWaylandObject()
        {
            if (Handle != IntPtr.Zero)
            {
                KeyboardDestroy(Handle);
                Handle = IntPtr.Zero;
            }
            if (_xkbMap != null)
            {
                _xkbMap.Dispose();
                _xkbMap = null;
            }
        }
    }
}