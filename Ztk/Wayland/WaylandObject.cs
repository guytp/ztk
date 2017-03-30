using System;
using System.Collections.Generic;
using System.Text;

namespace Ztk.Wayland
{
    public abstract class WaylandObject : IDisposable
    {
        public IntPtr Handle { get; protected set; }

        private bool _isDisposed = false;

        protected WaylandObject(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                throw new Exception("Failed to create " + GetType().Name + " Wayland object");
            Handle = handle;
        }
        ~WaylandObject()
        {
            Dispose();
        }

        protected abstract void ReleaseWaylandObject();

        public void Dispose()
        {
            if (_isDisposed)
                return;
            ReleaseWaylandObject();
            _isDisposed = true;
        }

        public override string ToString()
        {
            return Handle.ToHexString();
        }

        public override int GetHashCode()
        {
            return Handle.ToInt32();
        }
    }
}