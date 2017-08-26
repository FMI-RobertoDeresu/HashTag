using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HashTag.Contracts.Services
{
    public interface IFileService
    {
        /// <summary>
        /// Returns file location including name.
        /// </summary>
        Task<string> SaveAsync(IFormFile file, string location);
        Task DeleteAsync(string photoPath);
    }
}