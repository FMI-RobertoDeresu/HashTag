using System;
using System.Threading.Tasks;

namespace HashTag.Contracts.Services
{
    public interface IHttpService
    {
        Task<object> Post(string address, object data);
        Task<object> Post(string address, object data, TimeSpan timeout);
    }
}