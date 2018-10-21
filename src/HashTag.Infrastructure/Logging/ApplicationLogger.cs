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
            _logger = LogManager.GetLogger("ApplicationLogger");
        }

        public void LogInfo(string message)
        {
            var logEventInfo = new LogEventInfo();
            logEventInfo.Level = LogLevel.Info;
            logEventInfo.Properties["Created"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            logEventInfo.Properties["Type"] = $"{LogLevel.Info}";
            logEventInfo.Properties["Message"] = message;

            _logger.Log(logEventInfo);
        }

        public void LogError(Exception exception)
        {
            var errorLog = LogEventInfoFromException(exception, LogLevel.Error);
            _logger.Log(errorLog);
        }

        public void LogFatal(Exception exception)
        {
            var fatalLog = LogEventInfoFromException(exception, LogLevel.Fatal);
            _logger.Log(fatalLog);
        }

        private static LogEventInfo LogEventInfoFromException(Exception exception, LogLevel level)
        {
            var message = exception.Message;
            var innerException = exception.InnerException;

            while (innerException != null)
            {
                message = $"{message}{Environment.NewLine}{Environment.NewLine}" +
                          $"Inner exception ({innerException.GetType().Name}): {innerException.Message}";
                innerException = innerException.InnerException;
            }

            var logEventInfo = new LogEventInfo();
            logEventInfo.Level = level;
            logEventInfo.Properties["Created"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            logEventInfo.Properties["Type"] = $"{level} - {exception.GetType().Name}";
            logEventInfo.Properties["Message"] = message;
            logEventInfo.Properties["Trace"] = exception.StackTrace;

            return logEventInfo;
        }
    }
}