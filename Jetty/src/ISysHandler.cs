using Jetty.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Jetty
{
    internal interface ISysHandler : IDisposable
    {
        bool IsJetHandled(Process pr);
        IEnumerable<ShellWnd> GetOpenShellWnds();
        int AttachMouseDownHook(Action<SystemInputEvent> callback);
        int AttachMouseUpHook(Action<SystemInputEvent> callback);
        int AttachMouseScrollHook(Action<SystemInputEvent> callback);
        int AttachKeyDown(Action<SystemInputEvent> callback);
        int AttachKeyUp(Action<SystemInputEvent> callback);
        bool DetachHook(int hookId);
    }
}
