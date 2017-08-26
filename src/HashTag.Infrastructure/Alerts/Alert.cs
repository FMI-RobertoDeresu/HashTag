using System.Collections.Generic;
using System.Linq;

namespace HashTag.Infrastructure.Alerts
{
    public class Alert
    {
        private const string SuccessClass = "success";
        private const string InfoClass = "info";
        private const string WarningClass = "warning";
        private const string ErrorClass = "error";

        public Alert(string @class, string message)
        {
            Class = @class;
            Message = message;
        }

        public string Class { get; }

        public string Message { get; }

        public static Alert Success(string message) => new Alert(SuccessClass, message);

        public static Alert Info(string message) => new Alert(InfoClass, message);

        public static Alert Warning(string message) => new Alert(WarningClass, message);

        public static Alert Error(string message) => new Alert(ErrorClass, message);

        public static IEnumerable<Alert> Errors(IEnumerable<string> messages)
            => messages.Select(message => new Alert(ErrorClass, message));
    }
}