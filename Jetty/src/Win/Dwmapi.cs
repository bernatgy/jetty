using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Jetty.Win
{
    #region Enums

    [Flags]
    internal enum DWMWINDOWATTRIBUTE : uint
    {
        DWMWA_NCRENDERING_ENABLED = 1,
        DWMWA_NCRENDERING_POLICY,
        DWMWA_TRANSITIONS_FORCEDISABLED,
        DWMWA_ALLOW_NCPAINT,
        DWMWA_CAPTION_BUTTON_BOUNDS,
        DWMWA_NONCLIENT_RTL_LAYOUT,
        DWMWA_FORCE_ICONIC_REPRESENTATION,
        DWMWA_FLIP3D_POLICY,
        DWMWA_EXTENDED_FRAME_BOUNDS,
        DWMWA_HAS_ICONIC_BITMAP,
        DWMWA_DISALLOW_PEEK,
        DWMWA_EXCLUDED_FROM_PEEK,
        DWMWA_CLOAK,
        DWMWA_CLOAKED,
        DWMWA_FREEZE_REPRESENTATION,
        DWMWA_LAST
    }

    [Flags]
    internal enum DWMATTR : uint
    {
        DWM_CLOAKED_APP = 1,
        DWM_CLOAKED_SHELL,
        DWM_CLOAKED_INHERITED
    }

    #endregion

    [SuppressUnmanagedCodeSecurity]
    internal static class Dwmapi
    {
        [DllImport("dwmapi.dll", CharSet = CharSet.Auto)]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out IntPtr pvAttribute, int cbAttribute);
    }
}
