namespace Ztk.Wayland
{
    internal class XkbKeyPressData
    {
        public KeyboardKey Key { get; private set; }

        public string Value { get; private set; }

        public XkbKeyPressData(KeyboardKey key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}