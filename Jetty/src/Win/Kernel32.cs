using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Jetty.Win
{
    [SuppressUnmanagedCodeSecurity]
    internal static class Kernel32
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetProcessId(IntPtr handle);
    }
}
