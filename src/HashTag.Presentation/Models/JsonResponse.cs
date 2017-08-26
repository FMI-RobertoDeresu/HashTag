using System.Collections.Generic;
using HashTag.Infrastructure.Alerts;

namespace HashTag.Presentation.Models
{
    public class JsonResponse
    {
        public bool Success { get; set; }

        public IEnumerable<Alert> Alerts { get; set; }

        public dynamic Data { get; set; }

        //success
        public static JsonResponse SuccessResponse(string successMessage)
        {
            return new JsonResponse
            {
                Success = true,
                Alerts = new List<Alert> { Alert.Success(successMessage) }
            };
        }

        public static JsonResponse SuccessResponse(string successMessage, object data)
        {
            return new JsonResponse
            {
                Success = true,
                Alerts = new List<Alert> { Alert.Success(successMessage) },
                Data = data
            };
        }

        public static JsonResponse SuccessResponse(object data)
        {
            return new JsonResponse
            {
                Success = true,
                Alerts = null,
                Data = data
            };
        }

        //error
        public static JsonResponse ErrorResponse(string message)
        {
            return new JsonResponse
            {
                Success = false,
                Alerts = new[] { Alert.Error(message) },
                Data = null
            };
        }

        public static JsonResponse ErrorResponse(IEnumerable<string> messages)
        {
            return new JsonResponse
            {
                Success = false,
                Alerts = Alert.Errors(messages),
                Data = null
            };
        }

        public static JsonResponse ErrorResponse(object data)
        {
            return new JsonResponse
            {
                Success = false,
                Alerts = null,
                Data = data
            };
        }
    }
}