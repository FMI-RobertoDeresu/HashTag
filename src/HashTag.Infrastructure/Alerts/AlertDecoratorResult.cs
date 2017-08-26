using System.Threading.Tasks;
using HashTag.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace HashTag.Infrastructure.Alerts
{
    public class AlertDecoratorResult : IActionResult
    {
        private readonly IActionResult _innerActionResult;
        private readonly Alert _alert;

        public AlertDecoratorResult(IActionResult innerActionResult, Alert alert)
        {
            _innerActionResult = innerActionResult;
            _alert = alert;
        }

        public Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Session.AddAlert(_alert);

            return _innerActionResult.ExecuteResultAsync(context);
        }
    }
}