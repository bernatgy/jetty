﻿using System;

namespace Jetty
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }

    public interface ILogger
    {
        void Log(LogLevel level, string message);
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);
        void LogEx(Exception exception);
    }
}
