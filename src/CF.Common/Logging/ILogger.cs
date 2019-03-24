using System;

namespace CF.Common.Logging
{
    public interface ILogger
    {
        IDisposable BeginScope<TValue>(IScopeProperty<TValue> scopeProperty);

        void CloseAndFlush();

        void Critical(string message);

        void Error(string message);

        void Warning(string message);

        void Information(string message);

        void Debug(string message);

        void Trace(string message);

        void Critical(Exception exception, string message);

        void Error(Exception exception, string message);

        void Warning(Exception exception, string message);

        void Information(Exception exception, string message);

        void Debug(Exception exception, string message);

        void Trace(Exception exception, string message);

        void Log(LogLevel logLevel, string message);

        void Log(LogLevel logLevel, Exception exception, string message);
    }

    public interface ILogger<out T> : ILogger
    {
    }
}
