using Jetty.Events;
using Jetty.Win;
using SHDocVw;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using System.Runtime.InteropServices;
using static Jetty.Win.User32;

namespace Jetty
{
    public class Win32SysHandler : ISysHandler
    {
        // Send mouse input example: https://gist.github.com/Harmek/1263705

        private const int BaseWheelDelta = 120;
        private static uint CurrentHookId; // TODO: This might not be a good thing to do...
        private static uint NextHookId { get { return ++CurrentHookId; } }

        private readonly ILogger _logger;
        private readonly Dictionary<HookID, SysHook> _currentHooks = new Dictionary<HookID, SysHook>();

        private bool disposed = false;

        public Win32SysHandler(ILogger logger)
        {
            this._logger = logger;
#if(DEBUG)
            this.DebugLog();
#endif
        }

        public IEnumerable<ShellWnd> GetOpenShellWnds()
        {
            throw new NotImplementedException(); // TODO: DebugLog() <--
        }

        public bool IsJetHandled(Process pr)
        {
            // TODO: IsWindowVisible might be the same as MainWindowHandle != IntPtr.Zero
            return pr.MainWindowHandle != IntPtr.Zero &&
                !string.IsNullOrEmpty(pr.MainWindowTitle) &&
                IsWindowVisible(pr.MainWindowHandle) &&
                !this.IsCloakedApplication(pr.MainWindowHandle);
        }

        private void DebugLog()
        {
            if (this._logger == null) return;

            // Print out found Shell Windows and their info.
            var shellWindows = new ShellWindows();

            foreach (IWebBrowser2 wnd in shellWindows)
                this._logger.Log(LogLevel.Debug, $"Found Shell Window: {wnd.Name} (Name)\n\tLocationName: {wnd.LocationName}\n\tFullName: {wnd.FullName}");
        }

        #region Disposing

        public bool DetachHook(int hookId)
        {
            KeyValuePair<HookID, SysHook>? sysHook = this._currentHooks.FirstOrDefault(sh => sh.Value.Callbacks.ContainsKey((uint)hookId));
            if (sysHook.HasValue)
            {
                sysHook.Value.Value.Callbacks.Remove((uint)hookId);
                if (sysHook.Value.Value.Callbacks.Count == 0)
                {
                    UnhookWindowsHookEx(sysHook.Value.Value.HookHandle.ToInt32());
                    this._currentHooks.Remove(sysHook.Value.Key);
                }

                return true;
            }

            return false;
        }

        public void Dispose()
        {
            if (this.disposed) return;
            foreach (var sysHook in this._currentHooks)
                UnhookWindowsHookEx(sysHook.Value.HookHandle.ToInt32());

            this.disposed = true;
            GC.SuppressFinalize(this);
        }

        ~Win32SysHandler()
        {
            this.Dispose();
        }

        #endregion

        #region Mouse Events

        public int AttachMouseDownHook(Action<SystemInputEvent> callback)
        {
            // Creating the delegate to call for the system.
            this.RegisterProc(HookID.WH_MOUSE_LL, new HookProc(OnMouseProc));
            var cbs = this._currentHooks[HookID.WH_MOUSE_LL].Callbacks;

            cbs.Add(NextHookId, new KeyValuePair<HookType, Action<SystemInputEvent>>(HookType.MouseDown, callback));
            return (int)CurrentHookId;
        }

        public int AttachMouseUpHook(Action<SystemInputEvent> callback)
        {
            // Creating the delegate to call for the system.
            this.RegisterProc(HookID.WH_MOUSE_LL, new HookProc(OnMouseProc));
            var cbs = this._currentHooks[HookID.WH_MOUSE_LL].Callbacks;

            cbs.Add(NextHookId, new KeyValuePair<HookType, Action<SystemInputEvent>>(HookType.MouseUp, callback));
            return (int)CurrentHookId;
        }

        public int AttachMouseScrollHook(Action<SystemInputEvent> callback)
        {
            // Creating the delegate to call for the system.
            this.RegisterProc(HookID.WH_MOUSE_LL, new HookProc(OnMouseProc));
            var cbs = this._currentHooks[HookID.WH_MOUSE_LL].Callbacks;

            cbs.Add(NextHookId, new KeyValuePair<HookType, Action<SystemInputEvent>>(HookType.MouseScroll, callback));
            return (int)CurrentHookId;
        }

        #endregion

        #region Keyboard Events

        public int AttachKeyDown(Action<SystemInputEvent> callback)
        {
            // Creating the delegate to call for the system.
            this.RegisterProc(HookID.WH_KEYBOARD_LL, new HookProc(OnKeyboardProc));
            var cbs = this._currentHooks[HookID.WH_KEYBOARD_LL].Callbacks;

            cbs.Add(NextHookId, new KeyValuePair<HookType, Action<SystemInputEvent>>(HookType.KButtonDown, callback));
            return (int)CurrentHookId;
        }

        public int AttachKeyUp(Action<SystemInputEvent> callback)
        {
            // Creating the delegate to call for the system.
            this.RegisterProc(HookID.WH_KEYBOARD_LL, new HookProc(OnKeyboardProc));
            var cbs = this._currentHooks[HookID.WH_KEYBOARD_LL].Callbacks;

            cbs.Add(NextHookId, new KeyValuePair<HookType, Action<SystemInputEvent>>(HookType.KButttonUp, callback));
            return (int)CurrentHookId;
        }

        #endregion

        // Ref: https://msdn.microsoft.com/en-us/library/windows/desktop/ms644985(v=vs.85).aspx
        private int OnKeyboardProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code < 0) // System is asking not to process the event.
                return CallNextHookEx(this._currentHooks[HookID.WH_KEYBOARD_LL].HookHandle, code, wParam, lParam);

            // Calling WINAPI in a try block so we can handle the event after returning.
            try
            {
                // Calling the next hook in the chain. According to the docs this should only be called if the application didn't process the message.
                // We "should" return a non-zero value if we processed the message but then others in the chain wouldn't get it. (Might need IntPtr)
                return CallNextHookEx(this._currentHooks[HookID.WH_KEYBOARD_LL].HookHandle, code, wParam, lParam);
            }
            finally
            {
                // Marshall the data from the callback.
                var hookData = this.MarshalKeyboardData(lParam);
                var key = KeyInterop.KeyFromVirtualKey(hookData.vkCode);
                var e = (KeyboardEvent)wParam.ToInt32();
                var args = new SystemKeyboardEvent()
                {
                    Key = key,
                    Mods = Keyboard.Modifiers
                };
                //var flags = (KeyFlags)hookData.flags; // Not useful?

                // Filtering out modifiers.
                if (key != Key.LeftShift &&
                        key != Key.RightShift &&
                        key != Key.LeftCtrl &&
                        key != Key.RightCtrl &&
                        //key != Key.LWin &&
                        //key != Key.RWin &&
                        key != Key.LeftAlt &&
                        key != Key.RightAlt) {

                    foreach (var cb in this._currentHooks[HookID.WH_KEYBOARD_LL].Callbacks)
                    {
                        if (cb.Value.Key == HookType.KButtonDown && (e == KeyboardEvent.WM_KEYDOWN || e == KeyboardEvent.WM_SYSKEYDOWN))
                        {
                            args.State = KeyStates.Down;
                            cb.Value.Value.Invoke(args);
                        }
                        else if (cb.Value.Key == HookType.KButttonUp && (e == KeyboardEvent.WM_KEYUP || e == KeyboardEvent.WM_SYSKEYUP))
                        {
                            args.State = KeyStates.None;
                            cb.Value.Value.Invoke(args);
                        }
                    }
                }
            }
        }

        // Ref: https://msdn.microsoft.com/en-us/library/windows/desktop/ms644986(v=vs.85).aspx
        private int OnMouseProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code < 0) // System is asking not to process the event.
                return CallNextHookEx(this._currentHooks[HookID.WH_MOUSE_LL].HookHandle, code, wParam, lParam);

            // Calling WINAPI in a try block so we can handle the event after returning.
            try
            {
                // Calling the next hook in the chain. According to the docs this should only be called if the application didn't process the message.
                // We "should" return a non-zero value if we processed the message but then others in the chain wouldn't get it. (Might need IntPtr)
                return CallNextHookEx(this._currentHooks[HookID.WH_MOUSE_LL].HookHandle, code, wParam, lParam);
            }
            finally
            {
                // Marshall the data from the callback.
                var hookData = this.MarshalMouseData(lParam);
                var mData = new DW_INT() { Number = hookData.mouseData };
                var e = (MouseEvent)wParam.ToInt32();
                var args = new SystemMouseEvent()
                {
                    Position = new System.Windows.Point(hookData.pt.x, hookData.pt.y),
                    Mods = Keyboard.Modifiers
                };

                foreach (var cb in this._currentHooks[HookID.WH_MOUSE_LL].Callbacks)
                {
                    MouseButton? btn = null;

                    switch (cb.Value.Key)
                    {
                        case HookType.MouseDown:
                            switch (e)
                            {
                                case MouseEvent.WM_LBUTTONDOWN: btn = MouseButton.Left; break;
                                case MouseEvent.WM_RBUTTONDOWN: btn = MouseButton.Right; break;
                                case MouseEvent.WM_MBUTTONDOWN: btn = MouseButton.Middle; break;
                                case MouseEvent.WM_XBUTTONDOWN:
                                    btn = mData.High == 1 ? MouseButton.XButton1 : MouseButton.XButton2;
                                    break;
                            }

                            if (btn.HasValue)
                            {
                                args.Button = btn.Value;
                                args.State = MouseButtonState.Pressed;
                                cb.Value.Value.Invoke(args);
                            }
                            break;
                        case HookType.MouseUp:
                            switch (e)
                            {
                                case MouseEvent.WM_LBUTTONUP: btn = MouseButton.Left; break;
                                case MouseEvent.WM_RBUTTONUP: btn = MouseButton.Right; break;
                                case MouseEvent.WM_MBUTTONUP: btn = MouseButton.Middle; break;
                                case MouseEvent.WM_XBUTTONUP:
                                    btn = mData.High == 1 ? MouseButton.XButton1 : MouseButton.XButton2;
                                    break;
                            }

                            if (btn.HasValue)
                            {
                                args.Button = btn.Value;
                                args.State = MouseButtonState.Released;
                                cb.Value.Value.Invoke(args);
                            }
                            break;
                        case HookType.MouseScroll:
                            if (e == MouseEvent.WM_MOUSEWHEEL)
                            {
                                args.WheelDelta = mData.High / BaseWheelDelta;
                                cb.Value.Value.Invoke(args);
                            }
                            break;
                    }
                }
            }
        }

        private MSLLHOOKSTRUCT MarshalMouseData(IntPtr lParam)
        { return (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)); }

        private KBDLLHOOKSTRUCT MarshalKeyboardData(IntPtr lParam)
        { return (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT)); }

        private bool IsCloakedApplication(IntPtr hWnd)
        {
            var mainResult = IntPtr.Zero;
            var hResult = Dwmapi.DwmGetWindowAttribute(hWnd, (int)DWMWINDOWATTRIBUTE.DWMWA_CLOAKED, out mainResult, Marshal.SizeOf(typeof(uint)));
            return hResult > 0 || (DWMATTR)mainResult != 0;
        }

        private void RegisterProc(HookID hId, HookProc proc)
        {
            if (!this._currentHooks.ContainsKey(hId))
            {
                // Hooking the delegate to the system.
                // Hooks might be removed by timeout. Ref: https://msdn.microsoft.com/en-us/library/windows/desktop/ms644986(v=vs.85).aspx
                var hHook = SetWindowsHookEx((int)hId, proc, IntPtr.Zero, 0); // TODO: Module might be needed...
                // Storing the callback.
                this._currentHooks.Add(hId, new SysHook()
                {
                    HookHandle = hHook,
                    Procedure = proc,
                    Callbacks = new Dictionary<uint, KeyValuePair<HookType, Action<SystemInputEvent>>>()
                });
            }
        }

        /// <summary>
        /// All of the currently implemented Hook Types. Only used as a key to identify registered hooks.
        /// </summary>
        private enum HookType
        {
            MouseDown,
            MouseUp,
            MouseScroll,
            KButtonDown,
            KButttonUp
        }

        /// <summary>
        /// A registered System Hook and it's callbacks.
        /// </summary>
        private class SysHook
        {
            public IntPtr HookHandle;
            public HookProc Procedure;
            public Dictionary<uint, KeyValuePair<HookType, Action<SystemInputEvent>>> Callbacks;
        }

        #region Unmanaged Helpers

        /// <summary>
        /// Provides an easy access to a DWORD's High and Low order word.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct DW_UINT
        {
            [FieldOffset(0)]
            public uint Number;

            [FieldOffset(0)]
            public ushort Low;

            [FieldOffset(2)]
            public ushort High;
        }

        /// <summary>
        /// Provides an easy access to a DWORD's High and Low order word.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct DW_INT
        {
            [FieldOffset(0)]
            public int Number;

            [FieldOffset(0)]
            public short Low;

            [FieldOffset(2)]
            public short High;
        }

        #endregion
    }
}
