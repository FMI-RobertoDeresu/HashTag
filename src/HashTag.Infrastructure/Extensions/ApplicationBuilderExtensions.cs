using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.Targets;
using NLog.Web;

namespace HashTag.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void ConfigureNLog(this IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            string connectionString)
        {
            var nlogConfig = env.ConfigureNLog("nlog.config");
            var dbTargets = nlogConfig.AllTargets
                .Where(x => x is DatabaseTarget)
                .Cast<DatabaseTarget>();

            dbTargets.ForEach(dbTarget => dbTarget.ConnectionString = connectionString);
            LogManager.ReconfigExistingLoggers();
            loggerFactory.AddNLog();
            app.AddNLogWeb();
        }
    }
}