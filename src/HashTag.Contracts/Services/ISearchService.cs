using System.Collections.Generic;
using System.Threading.Tasks;
using HashTag.Domain.Dtos;

namespace HashTag.Contracts.Services
{
    public interface ISearchService
    {
        Task<IEnumerable<PhotoDto>> GetPhotosAsync(int skip);
        Task<IEnumerable<PhotoDto>> GetPhotosByHashTagAsync(string hashTag, int skip);
        Task<IEnumerable<PhotoDto>> GetPhotosByDescriptionAsync(string description, int skip);
        Task<IEnumerable<PhotoDto>> GetPhotosByPredictionAsync(double[] prediction, int skip);
    }
}