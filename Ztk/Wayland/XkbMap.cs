using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Ztk.Wayland
{
    /// <summary>
    /// This class is designed to wrap an XKB map.
    /// </summary>
    internal class XkbMap : IDisposable
    {
        #region Interop
        [DllImport("c", EntryPoint = "read")]
        private static extern int LibcRead(int fileDescriptor, byte[] buffer, uint length);

        [DllImport("c", EntryPoint = "lseek")]
        private static extern int LibcLSeek(int fileDescriptor, int offset, int whence);

        [DllImport("xkbcommon", EntryPoint = "xkb_context_new")]
        private static extern IntPtr XkbContextNew(uint flags);
        [DllImport("xkbcommon", EntryPoint = "xkb_context_unref")]
        private static extern void XkbContextUnref(IntPtr handle);

        [DllImport("xkbcommon", EntryPoint = "xkb_keymap_new_from_string")]
        private static extern IntPtr XKbCreateKeymap(IntPtr contextHandle, string value, uint format, uint flags);

        [DllImport("xkbcommon", EntryPoint = "xkb_keymap_unref")]
        private static extern void XkbKeymapUnref(IntPtr handle);

        [DllImport("xkbcommon", EntryPoint = "xkb_state_unref")]
        private static extern void XkbStateUnref(IntPtr handle);

        [DllImport("xkbcommon", EntryPoint = "xkb_state_new")]
        private static extern IntPtr XkbStateNew(IntPtr keymapHandle);

        [DllImport("xkbcommon", EntryPoint = "xkb_state_update_key")]
        private static extern uint XkbStateUpdateKey(IntPtr stateHandle, uint keyCode, uint direction);

        [DllImport("xkbcommon", EntryPoint = "xkb_state_key_get_utf8")]
        private static extern int XkbStateGetString(IntPtr stateHandle, uint keyCode, IntPtr buffer, uint size);

        [DllImport("xkbcommon", EntryPoint = "xkb_state_key_get_one_sym")]
        private static extern KeyboardKey XkbStateKeySym(IntPtr stateHandle, uint keyCode);

        [DllImport("xkbcommon", EntryPoint = "xkb_state_mod_name_is_active")]
        private static extern int XkbStateModNameIsActive(IntPtr stateHandle, string name, uint component);

        [DllImport("xkbcommon", EntryPoint = "xkb_state_led_name_is_active")]
        private static extern int XkbStateLedNameIsActive(IntPtr stateHandle, string name);
        #endregion

        #region Declarations
        private IntPtr _contextHandle;

        private IntPtr _keymapHandle;

        private IntPtr _stateHandle;

        private KeyboardState _keyboardState;
        #endregion


        public XkbMap(int fileDescriptor, uint size)
        {
            // First lets read in the string we've been given using the C file descriptor
            byte[] buffer = new byte[size];
            LibcLSeek(fileDescriptor, 0, 0);
            int resultCode = LibcRead(fileDescriptor, buffer, size);
            if (resultCode < 1)
                throw new Exception("Unable to read XkbMap");
            string keymap = Encoding.UTF8.GetString(buffer);

            // Now we can create our XKB context
            _contextHandle = XkbContextNew(0);
            if (_contextHandle == IntPtr.Zero)
                throw new Exception("Failed to create new XKB Context");
            _keymapHandle = XKbCreateKeymap(_contextHandle, keymap, 1, 0);
            if (_keymapHandle == IntPtr.Zero)
                throw new Exception("Failed to create XKB Keymap");

            // Reset our state
            ResetState();
        }

        public XkbKeyPressData TrackKeyUp(uint keyCode)
        {
            return TrackKey(keyCode + 8, 0);
        }

        private XkbKeyPressData TrackKey(uint keyCode, uint state)
        {
            // Determine the keysym for this before any event as it may impact if we read it afterwards
            KeyboardKey key = XkbStateKeySym(_stateHandle, keyCode);
            if (state == 0)
                _keyboardState.KeyRemove(key, keyCode);
            else
                _keyboardState.KeyAdd(key, keyCode);

            // Update our current tracking state
            uint stateModsEffective = (1 << 3);
            uint newState = XkbStateUpdateKey(_stateHandle, keyCode, state);
            if ((newState & stateModsEffective) != 0)
            {
                List<KeyboardModifier> modifiers = new List<KeyboardModifier>();
                if (XkbStateModNameIsActive(_stateHandle, "Shift", stateModsEffective) == 1)
                    modifiers.Add(KeyboardModifier.Shift);
                if (XkbStateModNameIsActive(_stateHandle, "Lock", stateModsEffective) == 1)
                    modifiers.Add(KeyboardModifier.CapsLock);
                if (XkbStateModNameIsActive(_stateHandle, "Control", stateModsEffective) == 1)
                    modifiers.Add(KeyboardModifier.Control);
                if (XkbStateModNameIsActive(_stateHandle, "Mod1", stateModsEffective) == 1)
                    modifiers.Add(KeyboardModifier.Alt);
                if (XkbStateModNameIsActive(_stateHandle, "Mod2", stateModsEffective) == 1)
                    modifiers.Add(KeyboardModifier.NumLock);
                if (XkbStateModNameIsActive(_stateHandle, "Mod4", stateModsEffective) == 1)
                    modifiers.Add(KeyboardModifier.Logo);
                _keyboardState.ModifiersUpdate(modifiers);

                // Update any key codes that have changed - i.e. A to a when shift is depressed
                uint[] keyCodes = _keyboardState.KeyCodes;
                KeyboardKey[] keys = _keyboardState.Keys;
                foreach (uint thisKeyCode in keyCodes)
                {
                    KeyboardKey thisKey = XkbStateKeySym(_stateHandle, thisKeyCode);
                    if (!keys.Contains(thisKey))
                        _keyboardState.KeyAdd(thisKey, thisKeyCode);
                }
            }

            // Return press data
            return GetPressData(key, keyCode);
        }

        public XkbKeyPressData TrackKeyDown(uint keyCode)
        {
            return TrackKey(keyCode + 8, 1);
        }

        private XkbKeyPressData GetPressData(KeyboardKey key, uint keyCode)
        {
            // Determine string for this key press and return it
            int size = XkbStateGetString(_stateHandle, keyCode, IntPtr.Zero, 0) + 1;
            if (size <= 1)
                return new XkbKeyPressData(key, null);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            XkbStateGetString(_stateHandle, keyCode, buffer, (uint)size);
            string value = Marshal.PtrToStringUTF8(buffer);
            Marshal.FreeHGlobal(buffer);
            return new XkbKeyPressData(key, value);
        }

        public void ResetState()
        {
            // Release any handles
            if (_stateHandle != IntPtr.Zero)
                XkbStateUnref(_stateHandle);

            // Now create a new state
            _stateHandle = XkbStateNew(_keymapHandle);
            if (_stateHandle == IntPtr.Zero)
                throw new Exception("Failed to create XKB State");

            // Update our corresponding C# state
            _keyboardState = new KeyboardState();
        }

        public KeyboardState GetState()
        {
            // Always return a copy so nothing else impacts this and so that it doesn't change if someone else has a handle to it
            return new KeyboardState(_keyboardState);
        }

        public void Dispose()
        {
            // Release any handles
            if (_stateHandle != IntPtr.Zero)
            {
                XkbStateUnref(_stateHandle);
                _stateHandle = IntPtr.Zero;
            }
            if (_keymapHandle != IntPtr.Zero)
            {
                XkbKeymapUnref(_keymapHandle);
                _keymapHandle = IntPtr.Zero;
            }
            if (_contextHandle != IntPtr.Zero)
            {
                XkbContextUnref(_contextHandle);
                _contextHandle = IntPtr.Zero;
            }
        }
    }
}