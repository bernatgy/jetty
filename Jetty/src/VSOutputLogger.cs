using System;
using System.Diagnostics;

namespace Jetty
{
    public class VSOutputLogger : ILogger
    {
        public LogLevel Level { get; private set; }

        public VSOutputLogger(LogLevel level)
        {
            this.Level = level;
        }

        public void Log(LogLevel level, string message)
        {
            Debug.WriteLine(message, level.ToString());
        }

        public void LogError(string message)
        {
            this.Log(LogLevel.Error, message);
        }

        public void LogEx(Exception exception)
        {
            Debug.WriteLine(exception, "EXC");
        }

        public void LogInfo(string message)
        {
            this.Log(LogLevel.Info, message);
        }

        public void LogWarning(string message)
        {
            this.Log(LogLevel.Warning, message);
        }
    }
}
