using System;
using System.Collections.Generic;

namespace Ztk
{
    public class KeyboardState
    {
        private readonly List<KeyboardModifier> _modifiers = new List<KeyboardModifier>();
        private readonly Dictionary<uint, KeyboardKey> _keyToCodeMap = new Dictionary<uint, KeyboardKey>();


        public KeyboardKey[] Keys
        {
            get
            {
                int i = 0;
                KeyboardKey[] keys = new KeyboardKey[_keyToCodeMap.Count];
                foreach (KeyValuePair<uint, KeyboardKey> kvp in _keyToCodeMap)
                {
                    keys[i] = kvp.Value;
                    i++;
                }
                return keys;
            }
        }
        internal uint[] KeyCodes
        {
            get
            {
                int i = 0;
                uint[] keyCodes = new uint[_keyToCodeMap.Count];
                foreach (KeyValuePair<uint, KeyboardKey> kvp in _keyToCodeMap)
                {
                    keyCodes[i] = kvp.Key;
                    i++;
                }
                return keyCodes;
            }
        }

        public KeyboardModifier[] Modifiers { get { return _modifiers.ToArray(); } }

        internal KeyboardState()
        {
        }

        internal KeyboardState(KeyboardState copyFrom)
        {
            _modifiers = new List<KeyboardModifier>(copyFrom.Modifiers);
            foreach (KeyValuePair<uint, KeyboardKey> kvp in copyFrom._keyToCodeMap)
                _keyToCodeMap.Add(kvp.Key, kvp.Value);
        }

        internal void KeyAdd(KeyboardKey key, uint keyCode)
        {
            if (!_keyToCodeMap.ContainsKey(keyCode))
                _keyToCodeMap.Add(keyCode, key);
            else
                _keyToCodeMap[keyCode] = key;
        }

        internal void KeyRemove(KeyboardKey key, uint keyCode)
        {
            if (!_keyToCodeMap.ContainsKey(keyCode))
                return;
            _keyToCodeMap.Remove(keyCode);
        }

        internal void ModifiersUpdate(IEnumerable<KeyboardModifier> modifiers)
        {
            _modifiers.Clear();
            _modifiers.AddRange(modifiers);
        }


        public override string ToString()
        {
            string keys = string.Empty;
            foreach (KeyboardKey key in Keys)
            {
                if (keys != string.Empty)
                    keys += ", ";
                keys += key.ToString();
            }
            if (keys == string.Empty)
                keys = "None";
            string mods = string.Empty;
            foreach (KeyboardModifier mod in _modifiers)
            {
                if (mods != string.Empty)
                    mods += ", ";
                mods += mod.ToString();
            }
            if (mods == string.Empty)
                mods = "None";
            string leds = string.Empty;
            return string.Format("Keys: {1}{0}Mods: {2}", Environment.NewLine, keys, mods);
        }
    }
}