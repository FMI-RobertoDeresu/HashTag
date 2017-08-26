using HashTag.Infrastructure.Alerts;
using Microsoft.AspNetCore.Mvc;

namespace HashTag.Infrastructure.Extensions
{
    public static class IActionResultExtensions
    {
        public static IActionResult WithSuccess(this IActionResult result, string message)
        {
            return new AlertDecoratorResult(result, Alert.Success(message));
        }

        public static IActionResult WithInfo(this IActionResult result, string message)
        {
            return new AlertDecoratorResult(result, Alert.Info(message));
        }

        public static IActionResult WithWarning(this IActionResult result, string message)
        {
            return new AlertDecoratorResult(result, Alert.Warning(message));
        }

        public static IActionResult WithError(this IActionResult result, string message)
        {
            return new AlertDecoratorResult(result, Alert.Error(message));
        }
    }
}