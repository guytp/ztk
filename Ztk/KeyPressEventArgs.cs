using System;
using System.Collections.Generic;
using System.Text;

namespace Ztk
{
    public class KeyPressEventArgs
    {
        public KeyboardState KeyboardState { get; private set; }
        
        public KeyboardKey Key { get; private set; }

        public string Value { get; private set; }

        public KeyPressEventArgs(KeyboardState keyboardState, KeyboardKey key, string value)
        {
            KeyboardState = keyboardState;
            Key = key;
            Value = value;
        }
    }
}