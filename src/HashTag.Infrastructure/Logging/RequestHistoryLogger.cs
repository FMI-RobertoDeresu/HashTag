using System;
using HashTag.Contracts.Loggers;
using HashTag.Domain.DependencyInjection;
using Microsoft.AspNetCore.Http;
using NLog;

namespace HashTag.Infrastructure.Logging
{
    [TransientDependency(ServiceType = typeof(IRequestHistoryLogger))]
    public class RequestHistoryLogger : IRequestHistoryLogger
    {
        private const string LoggerName = "RequestHistoryLogger";
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ILogger _logger;

        public RequestHistoryLogger(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = LogManager.GetLogger(LoggerName);
        }

        public void LogRequest()
        {
            var logEventInfo = new LogEventInfo(LogLevel.Info, LoggerName, string.Empty);
            logEventInfo.Properties["Created"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            logEventInfo.Properties["Ip"] = LogProperties.GetIp(_httpContextAccessor.HttpContext);
            logEventInfo.Properties["Username"] = LogProperties.GetUsername(_httpContextAccessor.HttpContext);
            logEventInfo.Properties["HttpMethod"] = LogProperties.GetHttpMethod(_httpContextAccessor.HttpContext);
            logEventInfo.Properties["Url"] = LogProperties.GetUrl(_httpContextAccessor.HttpContext);
            logEventInfo.Properties["UrlReferrer"] = LogProperties.GetUrlReferer(_httpContextAccessor.HttpContext);

            _logger.Log(logEventInfo);
        }
    }
}