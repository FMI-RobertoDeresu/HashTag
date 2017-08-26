using System.Collections.Generic;
using System.Threading.Tasks;
using HashTag.Domain;
using HashTag.Domain.Dtos;
using HashTag.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace HashTag.Contracts.Services
{
    public interface IPhotoService
    {
        Task<PhotoDto> GetAsync(long id);
        Task<IPhoto> FindMostSimilarPhotoAsync(double[] prediction, IEnumerable<Cluster> clusters);

        Task<string> ComputeDescriptionAsync(double[] prediction);
        Task<IEnumerable<string>> ComputeHashTagsAsync(double[] prediction);

        Task<long> SaveAsync(IFormFile file, string description, IEnumerable<string> hashTags);
        Task DeleteAsync(long id);
        Task BindPhotosToClusters();
    }
}