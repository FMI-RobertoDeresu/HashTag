using System.Collections.Generic;
using System.Threading.Tasks;
using HashTag.Domain.Models;

namespace HashTag.Contracts.Services
{
    public interface ISamplesService
    {
        Task<IEnumerable<SamplePhoto>> GetTrainPhotosAsync();
        Task<IEnumerable<SamplePhoto>> GetTestPhotosAsync();

        Task SavePredictionClassesAsync();
        Task SavePhotosPredictionsAsync();
        Task CreateTestDirectory();
    }
}