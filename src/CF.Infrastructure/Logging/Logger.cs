using CF.Common.Logging;
using CF.Common.Logging.Scopes;
using Serilog.Context;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CF.Infrastructure.Logging
{
    internal class Logger : ILogger
    {
        protected static readonly Dictionary<LogLevel, LogEventLevel> _logEventLevelByLogLevel = 
            new Dictionary<LogLevel, LogEventLevel>()
            {
                { LogLevel.Critical, LogEventLevel.Fatal },
                { LogLevel.Error, LogEventLevel.Error },
                { LogLevel.Warning, LogEventLevel.Warning },
                { LogLevel.Information, LogEventLevel.Information },
                { LogLevel.Debug, LogEventLevel.Debug },
                { LogLevel.Trace, LogEventLevel.Verbose },
            };

        public IDisposable BeginScope<TValue>(IScopeProperty<TValue> scopeProperty)
        {
            return LogContext.PushProperty(scopeProperty.Name, scopeProperty.Value);
        }

        public void CloseAndFlush()
        {
            Serilog.Log.CloseAndFlush();
        }

        public void Critical(string message)
        {
            this.Log(LogLevel.Critical, message);
        }

        public void Error(string message)
        {
            this.Log(LogLevel.Error, message);
        }

        public void Warning(string message)
        {
            this.Log(LogLevel.Warning, message);
        }

        public void Information(string message)
        {
            this.Log(LogLevel.Information, message);
        }

        public void Debug(string message)
        {
            this.Log(LogLevel.Debug, message);
        }

        public void Trace(string message)
        {
            this.Log(LogLevel.Trace, message);
        }

        public void Critical(Exception exception, string message)
        {
            this.Log(LogLevel.Critical, exception, message);
        }

        public void Error(Exception exception, string message)
        {
            this.Log(LogLevel.Error, exception, message);
        }

        public void Warning(Exception exception, string message)
        {
            this.Log(LogLevel.Warning, exception, message);
        }

        public void Information(Exception exception, string message)
        {
            this.Log(LogLevel.Information, exception, message);
        }

        public void Debug(Exception exception, string message)
        {
            this.Log(LogLevel.Debug, exception, message);
        }

        public void Trace(Exception exception, string message)
        {
            this.Log(LogLevel.Trace, exception, message);
        }

        public virtual void Log(LogLevel logLevel, Exception exception, string message)
        {
            Serilog.Log.Write(_logEventLevelByLogLevel[logLevel], exception, message);
        }

        public virtual void Log(LogLevel logLevel, string message)
        {
            Serilog.Log.Write(_logEventLevelByLogLevel[logLevel], message);
        }
    }

    internal class Logger<T> : Logger, ILogger<T>
    {
        public override void Log(LogLevel logLevel, Exception exception, string message)
        {
            var property = new ContextTypeNameScopeProperty(TypeNameHelper.GetTypeDisplayName(typeof(T)));
            using (this.BeginScope(property))
            {
                Serilog.Log.Write(_logEventLevelByLogLevel[logLevel], exception.Demystify(), message);
            }
        }

        public override void Log(LogLevel logLevel, string message)
        {
            var property = new ContextTypeNameScopeProperty(TypeNameHelper.GetTypeDisplayName(typeof(T)));
            using (this.BeginScope(property))
            {
                Serilog.Log.Write(_logEventLevelByLogLevel[logLevel], message);
            }
        }
    }
}
