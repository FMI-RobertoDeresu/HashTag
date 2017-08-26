using System.Linq;
using Microsoft.AspNetCore.Http;

namespace HashTag.Infrastructure.Logging
{
    public static class LogProperties
    {
        public static string GetIp(HttpContext httpContext)
        {
            return httpContext?.Connection?.RemoteIpAddress?.ToString();
        }

        public static string GetUsername(HttpContext httpContext)
        {
            if (httpContext?.User?.Identity?.Name != null)
                return httpContext.User.Identity.Name;
            var username = httpContext?.Items?["UserIdentity"] as string;
            return username;
        }

        public static string GetHttpMethod(HttpContext httpContext)
        {
            return httpContext?.Request?.Method;
        }

        public static string GetUrl(HttpContext httpContext)
        {
            return httpContext?.Request?.Path;
        }

        public static string GetUrlReferer(HttpContext httpContext)
        {
            if (httpContext?.Request?.Headers == null)
                return null;

            var url = httpContext.Request.Headers["Referer"];
            return !url.Any() ? null : url.ToString();
        }
    }
}