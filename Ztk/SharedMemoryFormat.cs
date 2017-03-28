﻿namespace Ztk
{
    internal enum SharedMemoryFormat : uint
    {
        ARGB8888 = 0,
        XRGB8888 = 1,
        C8 = 0x20203843,
        RGB332 = 0x38424752,
        BGR233 = 0x38524742,
        XRGB4444 = 0x32315258,
        XBGR4444 = 0x32314258,
        RGBX4444 = 0x32315852,
        BGRX4444 = 0x32315842,
        ARGB4444 = 0x32315241,
        ABGR4444 = 0x32314241,
        RGBA4444 = 0x32314152,
        BGRA4444 = 0x32314142,
        XRGB1555 = 0x35315258,
        XBGR1555 = 0x35314258,
        RGBX5551 = 0x35315852,
        BGRX5551 = 0x35315842,
        ARGB1555 = 0x35315241,
        ABGR1555 = 0x35314241,
        RGBA5551 = 0x35314152,
        BGRA5551 = 0x35314142,
        RGB565 = 0x36314752,
        BGR565 = 0x36314742,
        RGB888 = 0x34324752,
        BGR888 = 0x34324742,
        XBGR8888 = 0x34324258,
        RGBX8888 = 0x34325852,
        BGRX8888 = 0x34325842,
        ABGR8888 = 0x34324241,
        RGBA8888 = 0x34324152,
        BGRA8888 = 0x34324142,
        XRGB2101010 = 0x30335258,
        XBGR2101010 = 0x30334258,
        RGBX1010102 = 0x30335852,
        BGRX1010102 = 0x30335842,
        ARGB2101010 = 0x30335241,
        ABGR2101010 = 0x30334241,
        RGBA1010102 = 0x30334152,
        BGRA1010102 = 0x30334142,
        YUYV = 0x56595559,
        YVYU = 0x55595659,
        UYVY = 0x59565955,
        VYUY = 0x59555956,
        AYUV = 0x56555941,
        NV12 = 0x3231564e,
        NV21 = 0x3132564e,
        NV16 = 0x3631564e,
        NV61 = 0x3136564e,
        YUV410 = 0x39565559,
        YVU410 = 0x39555659,
        YUV411 = 0x31315559,
        YVU411 = 0x31315659,
        YUV420 = 0x32315559,
        YVU420 = 0x32315659,
        YUV422 = 0x36315559,
        YVU422 = 0x36315659,
        YUV444 = 0x34325559,
        YVU444 = 0x34325659,
    }
}