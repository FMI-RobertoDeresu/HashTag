using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HashTag.Contracts;
using HashTag.Contracts.Loggers;
using HashTag.Contracts.Repositories;
using HashTag.Contracts.Services;
using HashTag.Domain;
using HashTag.Domain.DependencyInjection;
using HashTag.Domain.Dtos;
using HashTag.Domain.Models;
using HashTag.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace HashTag.Application.Services
{
    [TransientDependency(ServiceType = typeof(IPhotoService))]
    internal class PhotoService : IPhotoService
    {
        private readonly IClusterService _clusterService;
        private readonly IPhotoProcessingService _photoProcessingService;
        private readonly IFileService _fileService;
        private readonly IApplicationLogger _appLogger;
        private readonly IPhotoRepository _photoRepository;
        private readonly IHashTagRepository _hashTagRepository;
        private readonly IPredictionClassRepository _predictionClassRepository;
        private readonly IUnitOfWork _unitOfWork;

        private readonly TaskFactory _taskFactory;
        private readonly string _photosAddress;
        private readonly double _hashTagThreshold;
        private readonly int _maxDefaultHashTags;

        public PhotoService(
            IClusterService clusterService,
            IPhotoProcessingService photoProcessingService,
            IFileService fileService,
            IApplicationLogger appLogger,
            IPhotoRepository photoRepository,
            IHashTagRepository hashTagRepository,
            IPredictionClassRepository predictionClassRepository,
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _photoRepository = photoRepository;
            _hashTagRepository = hashTagRepository;
            _predictionClassRepository = predictionClassRepository;
            _unitOfWork = unitOfWork;
            _photoProcessingService = photoProcessingService;
            _fileService = fileService;
            _clusterService = clusterService;
            _appLogger = appLogger;

            _taskFactory = new TaskFactory();
            _photosAddress = configuration["appStorage:uploadedImages"];

            _hashTagThreshold = double.Parse(configuration["app:hashTagThreshold"]);
            _maxDefaultHashTags = int.Parse(configuration["app:maxDefaultHashTags"]);
        }

        public async Task<PhotoDto> GetAsync(long id)
        {
            var photo = await _photoRepository.GetAsync(id);
            var photoDto = Mapper.Map<PhotoDto>(photo);

            return photoDto;
        }

        public async Task<IPhoto> FindMostSimilarPhotoAsync(double[] prediction, IEnumerable<Cluster> clusters)
        {
            var cluster = await _clusterService.GetNearestClusterAsync(prediction, clusters);
            return await FindMostSimilarPhotoInCluster(prediction, cluster);
        }

        public async Task<string> ComputeDescriptionAsync(double[] prediction)
        {
            try
            {
                var mostSimilarPhoto = await FindMostSimilarPhoto(prediction);
                return mostSimilarPhoto?.Description;
            }
            catch (Exception exception)
            {
                _appLogger.LogError(exception);
                return "";
            }
        }

        public async Task<IEnumerable<string>> ComputeHashTagsAsync(double[] prediction)
        {
            var topIndexes = prediction.WithIndexes()
                .OrderByDescending(x => x.Item2)
                .TakeWhile(x => x.Item2 > _hashTagThreshold)
                .Take(_maxDefaultHashTags)
                .Select(x => x.Item1)
                .ToList();

            var classes = (await _predictionClassRepository.GetManyAsync(x => topIndexes.Contains(x.Index))).ToList();
            classes.Sort((x, y) => topIndexes.IndexOf(x.Index) < topIndexes.IndexOf(y.Index) ? 1 : -1);

            var hashTags = classes.Select(x => x.Class);

            return hashTags;
        }

        public async Task<long> SaveAsync(IFormFile file, string description, IEnumerable<string> hashTagsNames)
        {
            var address = string.Empty;
            hashTagsNames = hashTagsNames.Where(x => !string.IsNullOrEmpty(x)).ToList();

            try
            {
                address = await _fileService.SaveAsync(file, _photosAddress);

                var prediction = (await _photoProcessingService.ComputePredictionsAsync(address)).ToArray();
                var nearestCluster = await _clusterService.GetNearestClusterAsync(prediction);
                var photo = new Photo(file.FileName, address, description, prediction, nearestCluster);

                foreach (var hashTagName in hashTagsNames)
                {
                    var newHashTah = new Domain.Models.HashTag(hashTagName);
                    await _hashTagRepository.CreateAsync(newHashTah);
                    photo.PhotoHashTags.Add(new PhotoHashTag(photo, newHashTah));
                }

                await _photoRepository.CreateAsync(photo);
                await _unitOfWork.CommitAsync();

                return photo.Id;
            }
            catch (Exception exception)
            {
                _appLogger.LogError(exception);
                await _fileService.DeleteAsync(address);

                return -1;
            }
        }

        public async Task DeleteAsync(long id)
        {
            var photo = await _photoRepository.GetAsync(id);

            if (photo != null)
            {
                await _photoRepository.DeleteAsync(photo);
                await _fileService.DeleteAsync(photo.Location);
            }
        }

        public async Task BindPhotosToClusters()
        {
            var photos = await _photoRepository.GetAllAsync();

            foreach (var photo in photos)
            {
                var nearestCluster = await _clusterService.GetNearestClusterAsync(photo.Prediction);
                photo.BindToCluster(nearestCluster);
                await _photoRepository.UpdateAsync(photo);
            }

            await _unitOfWork.CommitAsync();
        }

        //utils
        private async Task<IPhoto> FindMostSimilarPhoto(double[] prediction)
        {
            var cluster = await _clusterService.GetNearestClusterAsync(prediction);
            return await FindMostSimilarPhotoInCluster(prediction, cluster);
        }

        private async Task<IPhoto> FindMostSimilarPhotoInCluster(double[] prediction, Cluster cluster)
        {
            var _lock = new object();
            var bestSimilarity = double.MinValue;
            IPhoto mostSimilarPhoto = null;

            cluster.Photos.Select(clusterPhoto => _taskFactory.StartNew(async () =>
                {
                    var similarity = await _photoProcessingService.CosineSimilarityAsync(prediction,
                        clusterPhoto.SamplePhoto.Prediction);
                    lock (_lock)
                    {
                        if (!(similarity > bestSimilarity)) return;
                        bestSimilarity = similarity;
                        mostSimilarPhoto = clusterPhoto.SamplePhoto;
                    }
                }))
                .WaitAll();

            return mostSimilarPhoto;
        }
    }
}