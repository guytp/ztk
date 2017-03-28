using System;
using System.Collections.Generic;
using System.Text;

namespace Ztk
{
    public abstract class WaylandObject : IDisposable
    {
        public IntPtr Handle { get; protected set; }

        private bool _isDisposed = false;

        protected WaylandObject(IntPtr handle)
        {
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
    }
}