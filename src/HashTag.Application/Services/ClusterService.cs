using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HashTag.Contracts.Loggers;
using HashTag.Contracts.Repositories;
using HashTag.Contracts.Services;
using HashTag.Domain.DependencyInjection;
using HashTag.Domain.Models;
using HashTag.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;

namespace HashTag.Application.Services
{
    [TransientDependency(ServiceType = typeof(IClusterService))]
    internal class ClusterService : IClusterService
    {
        private readonly ISamplesService _samplesService;
        private readonly IPhotoProcessingService _photoProcessingService;
        private readonly IApplicationLogger _logger;
        private readonly IClusterRepository _clusterRepository;
        private readonly IClusterSamplePhotoRepository _clusterSamplePhotoRepository;
        private readonly int _numberOfClusters;
        private readonly int _predictionLength;

        private readonly TaskFactory _taskFactory;

        public ClusterService(
            ISamplesService samplesService,
            IPhotoProcessingService photoProcessingService,
            IApplicationLogger logger,
            IClusterRepository clusterRepository,
            IClusterSamplePhotoRepository clusterSamplePhotoRepository,
            IConfiguration configuration)
        {
            _samplesService = samplesService;
            _photoProcessingService = photoProcessingService;
            _logger = logger;
            _clusterRepository = clusterRepository;
            _clusterSamplePhotoRepository = clusterSamplePhotoRepository;

            _taskFactory = new TaskFactory();
            _numberOfClusters = int.Parse(configuration["app:clusters"]);
            _predictionLength = int.Parse(configuration["imageProcessing:predictionLength"]);
        }

        public async Task<Cluster> GetNearestClusterAsync(double[] prediction)
        {
            var clusters = await _clusterRepository.GetAllAsync();
            var nearest = await GetNearestClusterCoreAsync(prediction, clusters);
            var nearestWithPhotos = await _clusterRepository.GetWithPhotosAsync(nearest.Id);
            return nearestWithPhotos;
        }

        public async Task<Cluster> GetNearestClusterAsync(double[] prediction, IEnumerable<Cluster> clusters)
        {
            return await GetNearestClusterCoreAsync(prediction, clusters);
        }

        public async Task SaveClustersAsync()
        {
            var trainSamples = await _samplesService.GetTrainPhotosAsync();
            var clusters = await BuildClustersAsync(_numberOfClusters, trainSamples);

            await _clusterSamplePhotoRepository.DeleteAsync(x => true);
            await _clusterRepository.DeleteAsync(x => true);
            foreach (var cluster in clusters)
                await _clusterRepository.CreateAsync(cluster);

            _logger.LogInfo("Old clusters removed and new clusters added.");
        }

        public async Task<IEnumerable<Cluster>> BuildClustersAsync(int numOfClusters, IEnumerable<SamplePhoto> trainSamples)
        {
            var clusters = new Cluster[numOfClusters];
            var clustersPhotos = trainSamples
                .Select(samplePhoto => new ClusterSamplePhoto(samplePhoto))
                .ToArray();

            if (numOfClusters > clustersPhotos.Length)
                throw new Exception("More clusters than sample photos!!");

            //generate clusters
            for (var clusterNumber = 0; clusterNumber < numOfClusters; clusterNumber++)
            {
                var usedPhotos = clusters.Take(clusterNumber).SelectMany(cluster => cluster.Photos);
                var randomUnusedPhoto = clustersPhotos.Except(usedPhotos).Shuffle().First();
                var newCluster = new Cluster(randomUnusedPhoto.SamplePhoto.Prediction);
                newCluster.Photos.Add(randomUnusedPhoto);
                randomUnusedPhoto.Cluster = newCluster;
                clusters[clusterNumber] = newCluster;
            }

            var optimizationRound = 0;
            var clusterPredictionIndexes = Enumerable.Range(0, _predictionLength).ToArray();
            while (true)
            {
                optimizationRound++;
                var clusterHasChanges = new bool[clusters.Length];

                //repartition photos into clusters
                clustersPhotos.Select(delegate(ClusterSamplePhoto clusterPhoto)
                    {
                        return _taskFactory.StartNew(async () =>
                        {
                            var currentCluster = clusterPhoto.Cluster;
                            var nearestCluster = await GetNearestClusterCoreAsync(clusterPhoto.SamplePhoto.Prediction,
                                clusters);

                            if (ReferenceEquals(currentCluster, nearestCluster)) return;
                            lock (nearestCluster)
                            {
                                clusterHasChanges[Array.IndexOf(clusters, nearestCluster)] = true;
                                nearestCluster.Photos.Add(clusterPhoto);
                                clusterPhoto.Cluster = nearestCluster;
                            }

                            if (currentCluster == null) return;
                            lock (currentCluster)
                            {
                                clusterHasChanges[Array.IndexOf(clusters, currentCluster)] = true;
                                currentCluster.Photos.Remove(clusterPhoto);
                            }
                        });
                    })
                    .WaitAll();

                if (clusterHasChanges.All(x => !x)) break;

                //recenter clusters
                clusters.Where(cluster => clusterHasChanges[Array.IndexOf(clusters, cluster)])
                    .Select(delegate(Cluster cluster)
                    {
                        return _taskFactory.StartNew(() =>
                        {
                            var corePrediction = new double[_predictionLength];
                            var clusterPhotosPredictions = cluster.Photos.Select(
                                photo => photo.SamplePhoto.Prediction.ToArray()).ToArray();
                            clusterPredictionIndexes.Select(delegate(int index)
                                {
                                    return _taskFactory.StartNew(() =>
                                    {
                                        clusterPhotosPredictions.ForEach(
                                            prediction => corePrediction[index] += prediction[index]);
                                    });
                                })
                                .WaitAll();

                            cluster.CorePrediction = corePrediction;
                        });
                    })
                    .WaitAll();
            }

            _logger.LogInfo($"Clusters optimized after {optimizationRound} rounds.");
            return clusters;
        }

        //
        private async Task<Cluster> GetNearestClusterCoreAsync(double[] prediction, IEnumerable<Cluster> clusters)
        {
            var _lock = new object();
            var bestSimilarity = double.MinValue;
            Cluster nearestCluster = null;

            clusters.Select(cluster => _taskFactory.StartNew(async () =>
                {
                    var similarity = await _photoProcessingService.CosineSimilarityAsync(prediction, cluster.CorePrediction);
                    lock (_lock)
                    {
                        if (!(similarity > bestSimilarity)) return;
                        bestSimilarity = similarity;
                        nearestCluster = cluster;
                    }
                }))
                .WaitAll();

            return nearestCluster;
        }
    }
}