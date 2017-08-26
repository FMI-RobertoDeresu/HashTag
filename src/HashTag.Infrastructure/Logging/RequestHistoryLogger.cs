using System;
using HashTag.Contracts;
using HashTag.Contracts.Loggers;
using HashTag.Domain.DependencyInjection;
using Microsoft.AspNetCore.Http;
using NLog;

namespace HashTag.Infrastructure.Logging
{
    [TransientDependency(ServiceType = typeof(IRequestHistoryLogger))]
    public class RequestHistoryLogger : IRequestHistoryLogger
    {
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestHistoryLogger(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void LogRequest()
        {
            var eventLog = new LogEventInfo(LogLevel.Info, "RequestHistoryLogger", string.Empty);
            eventLog.Properties["Created"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            eventLog.Properties["Ip"] = LogProperties.GetIp(_httpContextAccessor.HttpContext);
            eventLog.Properties["Username"] = LogProperties.GetUsername(_httpContextAccessor.HttpContext);
            eventLog.Properties["HttpMethod"] = LogProperties.GetHttpMethod(_httpContextAccessor.HttpContext);
            eventLog.Properties["Url"] = LogProperties.GetUrl(_httpContextAccessor.HttpContext);
            eventLog.Properties["UrlReferrer"] = LogProperties.GetUrlReferer(_httpContextAccessor.HttpContext);

            _logger.Log(eventLog);
        }
    }
}