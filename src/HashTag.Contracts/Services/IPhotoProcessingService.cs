using System.Collections.Generic;
using System.Threading.Tasks;

namespace HashTag.Contracts.Services
{
    public interface IPhotoProcessingService
    {
        Task<IEnumerable<double>> ComputePredictionsAsync(string photoPath);
        Task<IEnumerable<IEnumerable<double>>> ComputePredictionsAsync(IEnumerable<string> photosPaths);

        Task<double> CosineSimilarityAsync(double[] first, double[] second);
    }
}