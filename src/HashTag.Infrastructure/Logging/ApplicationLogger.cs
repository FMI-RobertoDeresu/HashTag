using System;
using HashTag.Contracts.Loggers;
using HashTag.Domain.DependencyInjection;
using NLog;

namespace HashTag.Infrastructure.Logging
{
    [TransientDependency(ServiceType = typeof(IApplicationLogger))]
    public class ApplicationLogger : IApplicationLogger
    {
        private readonly ILogger _logger;

        public ApplicationLogger()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void LogInfo(string info)
        {
            var infoLog = new LogEventInfo(LogLevel.Info, nameof(ApplicationLogger), string.Empty);
            infoLog.Properties["Created"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            infoLog.Properties["Type"] = $"{LogLevel.Info}";
            infoLog.Properties["Message"] = info;
            _logger.Log(infoLog);
            Flush();
        }

        public void LogError(Exception exception)
        {
            var errorLog = LogEventInfoFromException(exception, LogLevel.Error);
            _logger.Log(errorLog);
            Flush();
        }

        public void LogFatal(Exception exception)
        {
            var fatalLog = LogEventInfoFromException(exception, LogLevel.Fatal);
            _logger.Log(fatalLog);
            Flush();
        }

        private static LogEventInfo LogEventInfoFromException(Exception exception, LogLevel level)
        {
            var exceptionLog = new LogEventInfo(level, "ApplicationLogger", string.Empty);
            exceptionLog.Properties["Created"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            exceptionLog.Properties["Type"] = $"{level} - {exception.GetType().Name}";
            exceptionLog.Properties["Message"] = exception.Message;
            exceptionLog.Properties["Trace"] = exception.StackTrace;
            return exceptionLog;
        }

        private static void Flush()
        {
            LogManager.Flush();
        }
    }
}