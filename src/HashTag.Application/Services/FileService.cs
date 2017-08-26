using System;
using System.IO;
using System.Threading.Tasks;
using HashTag.Contracts.Services;
using HashTag.Domain.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace HashTag.Application.Services
{
    [TransientDependency(ServiceType = typeof(IFileService))]
    public class FileService : IFileService
    {
        public async Task<string> SaveAsync(IFormFile file, string location)
        {
            var uid = Guid.NewGuid();
            var path = Path.Combine(location, uid + ".jpg");

            if (!Directory.Exists(location))
                Directory.CreateDirectory(location);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
            }

            return path;
        }

        public async Task DeleteAsync(string photoPath)
        {
            if (File.Exists(photoPath))
                File.Delete(photoPath);
        }
    }
}