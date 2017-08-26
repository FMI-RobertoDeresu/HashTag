using System;
using System.Net.Http;
using System.Threading.Tasks;
using HashTag.Contracts.Services;
using HashTag.Domain.DependencyInjection;
using HashTag.Infrastructure;
using Newtonsoft.Json;

namespace HashTag.Application.Services
{
    [TransientDependency(ServiceType = typeof(IHttpService))]
    public class HttpService : IHttpService
    {
        public async Task<object> Post(string address, object data)
        {
            return await Post(address, data, TimeSpan.FromSeconds(100));
        }

        public async Task<object> Post(string address, object data, TimeSpan timeout)
        {
            var httpRequest = new JsonContent(data);

            var httClient = new HttpClient { Timeout = timeout };
            var httpResponse = await httClient.PostAsync(address, httpRequest);

            if (httpResponse.Content == null)
                return null;

            var responseStr = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject(responseStr);

            return response;
        }
    }
}