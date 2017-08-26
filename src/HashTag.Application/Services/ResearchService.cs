using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HashTag.Contracts.Repositories;
using HashTag.Contracts.Services;
using HashTag.Domain.DependencyInjection;
using HashTag.Domain.Models;
using Microsoft.Extensions.Configuration;

namespace HashTag.Application.Services
{
    [TransientDependency(ServiceType = typeof(IResearchService))]
    public class ResearchService : IResearchService
    {
        private readonly IPhotoService _photoService;
        private readonly IClusterService _clusterService;
        private readonly ISamplesService _samplesService;
        private readonly IKMeansResearchResultRepository _kMeansResearchRepository;

        private readonly int _kmeansStages;
        private readonly int _kmeansClustersStart;
        private readonly int _kmeansClustersEnd;

        public ResearchService(
            IPhotoService photoService,
            IClusterService clusterService,
            ISamplesService samplesService,
            IKMeansResearchResultRepository kMeansResearchRepository,
            IConfiguration configuration)
        {
            _photoService = photoService;
            _clusterService = clusterService;
            _samplesService = samplesService;
            _kMeansResearchRepository = kMeansResearchRepository;

            _kmeansStages = int.Parse(configuration["research:kmeans:stages"]);
            _kmeansClustersStart = int.Parse(configuration["research:kmeans:clustersStart"]);
            _kmeansClustersEnd = int.Parse(configuration["research:kmeans:clustersEnd"]);
        }

        public async Task KMeansResearchAsync()
        {
            var stepSize = (_kmeansClustersEnd - _kmeansClustersStart) / (_kmeansStages - 1);
            var trainSamples = await _samplesService.GetTrainPhotosAsync();
            var testSamples = await _samplesService.GetTestPhotosAsync();
            var results = new List<KMeansResearchResult>();

            for (var numOfClusters = _kmeansClustersStart; numOfClusters <= _kmeansClustersEnd; numOfClusters += stepSize)
            {
                var watch = Stopwatch.StartNew();
                var clustersBuildWatch = Stopwatch.StartNew();
                var clusters = await _clusterService.BuildClustersAsync(numOfClusters, trainSamples);
                clustersBuildWatch.Stop();

                var result = 0d;
                var photoSearchWatch = Stopwatch.StartNew();
                foreach (var testPhoto in testSamples)
                {
                    var similarPhoto = await _photoService.FindMostSimilarPhotoAsync(testPhoto.Prediction, clusters);
                    result += CompareDescriptions(testPhoto.Description, similarPhoto.Description);
                }
                photoSearchWatch.Stop();

                result = result / testSamples.Count();
                watch.Stop();
                results.Add(new KMeansResearchResult(numOfClusters, result,
                    watch.ElapsedMilliseconds, clustersBuildWatch.ElapsedMilliseconds, photoSearchWatch.ElapsedMilliseconds));
            }

            await _kMeansResearchRepository.DeleteAsync(x => true);
            foreach (var kMeansResearchResult in results)
                await _kMeansResearchRepository.CreateAsync(kMeansResearchResult);
        }

        public async Task<IEnumerable<KMeansResearchResult>> GetKMeansResearchResultsAsync()
        {
            var results = await _kMeansResearchRepository.GetAllAsync();
            results = results.ToList().OrderBy(x => x.Clusters);

            return results;
        }

        //utils
        /// <summary>
        ///     Returns (number common words)/(number of words from original)
        /// </summary>
        private static double CompareDescriptions(string original, string computed)
        {
            var originalWords = original.Split(' ').Select(x => x.Trim()).Where(x => x.Length > 1).ToList();
            var computedWords = computed.Split(' ').Select(x => x.Trim()).Where(x => x.Length > 1).ToList();

            var totalWords = originalWords.Count;
            var numOfCommonWords = originalWords.Count(originalWord => computedWords
                .Any(computedWord => computedWord == originalWord));

            return (double) numOfCommonWords / totalWords;
        }
    }
}