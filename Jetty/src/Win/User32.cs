﻿using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Jetty.Win
{

    #region Enums

    internal enum HookID : int
    {
        WH_KEYBOARD             = 0x2, // 2
        WH_KEYBOARD_LL          = 0xD, // 13
        WH_CALLWNDPROC          = 0x4, // 4
        WH_CALLWNDPROCRET       = 0xC, // 12
        WH_MOUSE                = 0x7, // 7
        WH_MOUSE_LL             = 0xE  // 14
    }

    internal enum MouseEvent : int
    {
        WM_MOUSEMOVE            = 0x0200,
        WM_MOUSEWHEEL           = 0x020A,
        WM_RBUTTONDOWN          = 0x0204,
        WM_LBUTTONDOWN          = 0x0201,
        WM_MBUTTONDOWN          = 0x0207,
        WM_XBUTTONDOWN          = 0x020B,

        WM_RBUTTONUP            = 0x0205,
        WM_LBUTTONUP            = 0x0202,
        WM_MBUTTONUP            = 0x0208,
        WM_XBUTTONUP            = 0x020C // Ref: https://msdn.microsoft.com/en-us/library/windows/desktop/ms646246(v=vs.85).aspx
    }

    internal enum KeyboardEvent : int
    {
        WM_KEYDOWN              = 0x0100,
        WM_KEYUP                = 0x0101,
        WM_SYSKEYDOWN           = 0x0104,
        WM_SYSKEYUP             = 0x0105
    }

    [Flags]
    internal enum KeyFlags : int
    {
        KF_EXTENDED             = 0x0100,
        KF_DLGMODE              = 0x0800,
        KF_MENUMODE             = 0x1000,
        KF_ALTDOWN              = 0x2000,
        KF_REPEAT               = 0x4000,
        KF_UP                   = 0x8000
    }

    /// <summary>
    /// Enumeration for virtual keys.
    /// </summary>
    internal enum VirtualKeys : ushort
    {
        LeftButton = 0x01, RightButton = 0x02, Cancel = 0x03, MiddleButton = 0x04,
        ExtraButton1 = 0x05, ExtraButton2 = 0x06, Back = 0x08, Tab = 0x09,
        Clear = 0x0C, Return = 0x0D, Shift = 0x10, Control = 0x11, Menu = 0x12,
        Pause = 0x13, CapsLock = 0x14, Kana = 0x15, Hangeul = 0x15, Hangul = 0x15,
        Junja = 0x17, Final = 0x18, Hanja = 0x19, Kanji = 0x19, Escape = 0x1B,
        Convert = 0x1C, NonConvert = 0x1D, Accept = 0x1E, ModeChange = 0x1F,
        Space = 0x20, Prior = 0x21, Next = 0x22, End = 0x23, Home = 0x24,
        Left = 0x25, Up = 0x26, Right = 0x27, Down = 0x28, Select = 0x29,
        Print = 0x2A, Execute = 0x2B, Snapshot = 0x2C, Insert = 0x2D, Delete = 0x2E,
        Help = 0x2F, N0 = 0x30, N1 = 0x31, N2 = 0x32, N3 = 0x33, N4 = 0x34,
        N5 = 0x35, N6 = 0x36, N7 = 0x37, N8 = 0x38, N9 = 0x39, A = 0x41,
        B = 0x42, C = 0x43, D = 0x44, E = 0x45, F = 0x46, G = 0x47, H = 0x48,
        I = 0x49, J = 0x4A, K = 0x4B, L = 0x4C, M = 0x4D, N = 0x4E, O = 0x4F,
        P = 0x50, Q = 0x51, R = 0x52, S = 0x53, T = 0x54, U = 0x55, V = 0x56,
        W = 0x57, X = 0x58, Y = 0x59, Z = 0x5A, LeftWindows = 0x5B, RightWindows = 0x5C,
        Application = 0x5D, Sleep = 0x5F, Numpad0 = 0x60, Numpad1 = 0x61, Numpad2 = 0x62,
        Numpad3 = 0x63, Numpad4 = 0x64, Numpad5 = 0x65, Numpad6 = 0x66, Numpad7 = 0x67,
        Numpad8 = 0x68, Numpad9 = 0x69, Multiply = 0x6A, Add = 0x6B, Separator = 0x6C,
        Subtract = 0x6D, Decimal = 0x6E, Divide = 0x6F, F1 = 0x70, F2 = 0x71,
        F3 = 0x72, F4 = 0x73, F5 = 0x74, F6 = 0x75, F7 = 0x76, F8 = 0x77, F9 = 0x78,
        F10 = 0x79, F11 = 0x7A, F12 = 0x7B, F13 = 0x7C, F14 = 0x7D, F15 = 0x7E,
        F16 = 0x7F, F17 = 0x80, F18 = 0x81, F19 = 0x82, F20 = 0x83, F21 = 0x84,
        F22 = 0x85, F23 = 0x86, F24 = 0x87, NumLock = 0x90, ScrollLock = 0x91,
        NEC_Equal = 0x92, Fujitsu_Jisho = 0x92, Fujitsu_Masshou = 0x93, Fujitsu_Touroku = 0x94,
        Fujitsu_Loya = 0x95, Fujitsu_Roya = 0x96, LeftShift = 0xA0, RightShift = 0xA1,
        LeftControl = 0xA2, RightControl = 0xA3, LeftMenu = 0xA4, RightMenu = 0xA5,
        BrowserBack = 0xA6, BrowserForward = 0xA7, BrowserRefresh = 0xA8, BrowserStop = 0xA9,
        BrowserSearch = 0xAA, BrowserFavorites = 0xAB, BrowserHome = 0xAC, VolumeMute = 0xAD,
        VolumeDown = 0xAE, VolumeUp = 0xAF, MediaNextTrack = 0xB0, MediaPrevTrack = 0xB1,
        MediaStop = 0xB2, MediaPlayPause = 0xB3, LaunchMail = 0xB4, LaunchMediaSelect = 0xB5,
        LaunchApplication1 = 0xB6, LaunchApplication2 = 0xB7, OEM1 = 0xBA, OEMPlus = 0xBB,
        OEMComma = 0xBC, OEMMinus = 0xBD, OEMPeriod = 0xBE, OEM2 = 0xBF, OEM3 = 0xC0,
        OEM4 = 0xDB, OEM5 = 0xDC, OEM6 = 0xDD, OEM7 = 0xDE, OEM8 = 0xDF, OEMAX = 0xE1,
        OEM102 = 0xE2, ICOHelp = 0xE3, ICO00 = 0xE4, ProcessKey = 0xE5, ICOClear = 0xE6,
        Packet = 0xE7, OEMReset = 0xE9, OEMJump = 0xEA, OEMPA1 = 0xEB, OEMPA2 = 0xEC,
        OEMPA3 = 0xED, OEMWSCtrl = 0xEE, OEMCUSel = 0xEF, OEMATTN = 0xF0, OEMFinish = 0xF1,
        OEMCopy = 0xF2, OEMAuto = 0xF3, OEMENLW = 0xF4, OEMBackTab = 0xF5,
        ATTN = 0xF6, CRSel = 0xF7, EXSel = 0xF8, EREOF = 0xF9, Play = 0xFA, Zoom = 0xFB,
        Noname = 0xFC, PA1 = 0xFD, OEMClear = 0xFE
    }

    #endregion

    #region Structs

    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        public int x;
        public int y;
    }

    /// <summary>
    /// Contains information about a low-level mouse input event.
    /// </summary>
    /// <see cref="https://msdn.microsoft.com/en-us/library/windows/desktop/ms644970%28v=vs.85%29.aspx"/>
    [StructLayout(LayoutKind.Sequential)]
    internal struct MSLLHOOKSTRUCT
    {
        /// <summary>
        /// The current position of the mouse.
        /// </summary>
        public POINT pt;
        /// <summary>
        /// Contains scroll offset in the case of a scroll event (multiplications of 120).
        /// Contains XMouseButton 1 or 2. ???
        /// </summary>
        public int mouseData;
        public int flags;
        public int time;
        public UIntPtr dwExtraInfo; // Touch info might be in this as uint flags. https://gist.github.com/vbfox/1339671/9f59a10f7dc15a09336bd48853541c45091ccdb8
    }

    /// <summary>
    /// Contains information about a low-level keyboard input event.
    /// </summary>
    /// <see cref="https://msdn.microsoft.com/en-us/library/windows/desktop/ms644967(v=vs.85).aspx"/>
    [StructLayout(LayoutKind.Sequential)]
    internal struct KBDLLHOOKSTRUCT
    {
        public int vkCode;
        public int scanCode;
        public int flags;
        public int time;
        public UIntPtr dwExtraInfo;
    }

    #endregion

    [SuppressUnmanagedCodeSecurity]
    internal static class User32
    {
        public delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetShellWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern ushort GetKeyState(int keyCode);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = false, CharSet = CharSet.Auto)]
        public static extern IntPtr GetMessageExtraInfo();
    }
}
