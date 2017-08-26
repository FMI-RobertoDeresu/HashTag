using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using HashTag.Contracts.Repositories;
using HashTag.Contracts.Services;
using HashTag.Domain.DependencyInjection;
using HashTag.Domain.Dtos;
using Microsoft.Extensions.Configuration;

namespace HashTag.Application.Services
{
    [TransientDependency(ServiceType = typeof(ISearchService))]
    public class SearchService : ISearchService
    {
        private readonly IClusterService _clusterService;
        private readonly IPhotoRepository _photoRepository;

        private readonly int _feedSize;

        public SearchService(
            IClusterService clusterService,
            IPhotoRepository photoRepository,
            IConfiguration configuration)
        {
            _clusterService = clusterService;
            _photoRepository = photoRepository;

            _feedSize = int.Parse(configuration["app:feedSize"]);
        }

        public async Task<IEnumerable<PhotoDto>> GetPhotosAsync(int skip)
        {
            var photos = await _photoRepository.GetPagedAsync(skip, _feedSize);
            var photosDto = Mapper.Map<IList<PhotoDto>>(photos);

            return photosDto;
        }

        public async Task<IEnumerable<PhotoDto>> GetPhotosByHashTagAsync(string hashTag, int skip)
        {
            var photos = await _photoRepository.GetPagedByHashTagAsync(hashTag, skip, _feedSize);
            var photosDto = Mapper.Map<IList<PhotoDto>>(photos);

            return photosDto;
        }

        public async Task<IEnumerable<PhotoDto>> GetPhotosByDescriptionAsync(string description, int skip)
        {
            var photos = await _photoRepository.GetPagedByDescriptionAsync(description, skip, _feedSize);
            var photosDto = Mapper.Map<IList<PhotoDto>>(photos);

            return photosDto;
        }

        public async Task<IEnumerable<PhotoDto>> GetPhotosByPredictionAsync(double[] prediction, int skip)
        {
            var nearestCluster = await _clusterService.GetNearestClusterAsync(prediction);

            var photos = await _photoRepository.GetPagedByCluster(nearestCluster, skip, _feedSize);
            var photosDto = Mapper.Map<IList<PhotoDto>>(photos);

            return photosDto;
        }
    }
}