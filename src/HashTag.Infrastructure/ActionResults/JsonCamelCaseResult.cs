using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace HashTag.Infrastructure.ActionResults
{
    public class JsonCamelCaseResult : IActionResult
    {
        private readonly object _data;
        private readonly HttpStatusCode _httpStatusCode;

        public JsonCamelCaseResult(object data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            _data = data;
            _httpStatusCode = HttpStatusCode.OK;
        }

        public JsonCamelCaseResult(object data, HttpStatusCode httpStatusCode) : this(data)
        {
            _httpStatusCode = httpStatusCode;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.Converters.Add(new StringEnumConverter());
            jsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.StatusCode = (int) _httpStatusCode;
            await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(_data, jsonSerializerSettings));
        }
    }
}