using System;
using System.Runtime.InteropServices;
using Ztk.Drawing;

namespace Ztk.Wayland
{
    internal class Surface : WaylandObject
    {
        [DllImport("wayland-wrapper", EntryPoint = "wlw_surface_attach")]
        private static extern void SurfaceAttach(IntPtr surface, IntPtr buffer);

        [DllImport("wayland-wrapper", EntryPoint = "wlw_surface_commit")]
        private static extern void SurfaceCommit(IntPtr surface);
        [DllImport("wayland-wrapper", EntryPoint = "wlw_surface_damage")]

        private static extern void SurfaceDamage(IntPtr surface, int x, int y, int width, int height);

        [DllImport("wayland-wrapper", EntryPoint = "wlw_surface_frame_listener")]
        private static extern void SurfaceAddFrameListener(IntPtr surface, SurfaceFrameListener frameListener);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SurfaceFrameListener(IntPtr data, IntPtr callback, uint callbackData);


        private readonly SurfaceFrameListener _frameListener;

        private readonly SharedMemory _sharedMemory;

        private Buffer _buffer;

        private Control _renderTarget;

        public SurfaceType SurfaceType { get; protected set; }

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
                    CreateBuffer((int)Math.Round(value.ActualWidth), (int)Math.Round(value.ActualHeight));
                    Size targetSize = value.MeasureDesiredSize(new Size(_buffer.Width, _buffer.Height));
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
            PerformRenderingEventLoop();
        }

        private void PerformRenderingEventLoop()
        {
            if (_buffer != null)
            {
                SurfaceDamage(Handle, 0, 0, _buffer.Width, _buffer.Height);
                SurfaceAttach(Handle, _buffer.Handle);

                if (RenderTarget != null)
                {
                    if (RenderTarget.HorizontalAlignment != HorizontalAlignment.Stretch || RenderTarget.VerticalAlignment != VerticalAlignment.Stretch)
                        throw new Exception("Main render target must have dual stretch alignments");
                    RenderTarget.MeasureDesiredSize(new Size(_buffer.Width, _buffer.Height));
                    int pixels = _buffer.Width * _buffer.Stride;
                    using (ImageSurface imageSurface = new ImageSurface(Format.ARGB32, _buffer.Width, _buffer.Height))
                    {
                        using (GraphicsContext g = new GraphicsContext(imageSurface))
                        {
                            if (RenderTarget.Opacity == 1)
                                RenderTarget.Render(g);
                            else if (RenderTarget.Opacity > 0)
                                using (ImageSurface innerImageSurface = new ImageSurface(Format.ARGB32, _buffer.Width, _buffer.Height))
                                {
                                    using (GraphicsContext innerGraphics = new GraphicsContext(innerImageSurface))
                                    {
                                        RenderTarget.Render(innerGraphics);
                                        g.SetSourceSurface(innerImageSurface, 0, 0);
                                        g.PaintWithAlpha(RenderTarget.Opacity);
                                    }
                                }
                        }
                        Marshal.Copy(imageSurface.Data, 0, _buffer.SharedMemoryPointer, imageSurface.Data.Length);
                    }
                }
            }

            SurfaceAddFrameListener(Handle, _frameListener);
            if (_buffer != null)
                SurfaceCommit(Handle);
        }
    }
}