using System;
using System.Runtime.InteropServices;
using Ztk.Drawing;

namespace Ztk.Wayland
{
    internal class Surface : WaylandObject
    {
        [DllImport("wayland-wrapper", EntryPoint = "wlw_surface_attach")]
        public static extern void SurfaceAttach(IntPtr surface, IntPtr buffer);

        [DllImport("wayland-wrapper", EntryPoint = "wlw_surface_commit")]
        public static extern void SurfaceCommit(IntPtr surface);
        [DllImport("wayland-wrapper", EntryPoint = "wlw_surface_damage")]

        public static extern void SurfaceDamage(IntPtr surface, int x, int y, int width, int height);

        [DllImport("wayland-wrapper", EntryPoint = "wlw_surface_frame_listener")]
        public static extern void SurfaceAddFrameListener(IntPtr surface, SurfaceFrameListener frameListener);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SurfaceFrameListener(IntPtr data, IntPtr callback, uint callbackData);

        private int _width;

        private int _height;

        private readonly SurfaceFrameListener _frameListener;

        private readonly SharedMemory _sharedMemory;

        private Buffer _buffer;

        private Control _renderTarget;

        private ShellSurface _shellSurface;

        public SurfaceType SurfaceType { get; private set; }

        public Control RenderTarget
        {
            get
            {
                return _renderTarget;
            }

            set
            {
                _renderTarget = value;
                if (value != null)
                {
                    Size targetSize = value.MeasureDesiredSize(new Size(value.ActualWidth, value.ActualHeight));
                    CreateBuffer((int)Math.Round(targetSize.Width), (int)Math.Round(targetSize.Height));
                    value.SetActualSize(targetSize);
                }
            }
        }

        public Surface(IntPtr handle, SharedMemory sharedMemory)
            : base(handle)
        {
            _frameListener = OnFrameListener;
            _sharedMemory = sharedMemory;
        }

        public void ConvertToShell()
        {
            if (SurfaceType != SurfaceType.Undefined)
                throw new Exception("Cannot change surface type once defined");
            _shellSurface = new ShellSurface(App.CurrentApplication.Shell.CreateShellSurface(this));
        }

        private void CreateBuffer(int width, int height)
        {
            _buffer?.Dispose();
            _buffer = _sharedMemory.CreateBuffer(width, height);
        }

        protected override void ReleaseWaylandObject()
        {
        }

        public void StartRenderingEvents()
        {
            PerformRenderingEventLoop();
        }

        private void OnFrameListener(IntPtr data, IntPtr callback, uint callbackData)
        {
        }
        private void PerformRenderingEventLoop()
        {
            SurfaceDamage(Handle, 0, 0, _width, _height);
            SurfaceAttach(Handle, _buffer.Handle);

            if (RenderTarget != null)
            {
                int pixels = _width * _height * 4;
                using (ImageSurface imageSurface = new ImageSurface(Format.ARGB32, _width, _height))
                {
                    using (GraphicsContext g = new GraphicsContext(imageSurface))
                    {
                        RenderTarget.Render(g);
                    }
                    Marshal.Copy(imageSurface.Data, 0, _buffer.SharedMemoryPointer, imageSurface.Data.Length);

                }
            }
            SurfaceAddFrameListener(Handle, _frameListener);
            SurfaceCommit(Handle);
        }

    }
}