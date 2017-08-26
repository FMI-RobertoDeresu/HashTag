using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using HashTag.Contracts.Repositories;
using HashTag.Contracts.Services;
using HashTag.Domain.DependencyInjection;
using HashTag.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace HashTag.Application.Services
{
    [TransientDependency(ServiceType = typeof(IPhotoProcessingService))]
    public class PhotoProcessingService : IPhotoProcessingService
    {
        private readonly IHttpService _httpService;

        private readonly string _computePredictionPostAddress;
        private readonly int _timeoutPerPhoto;

        public PhotoProcessingService(
            IHttpService httpService,
            IConfiguration configuration)
        {
            _httpService = httpService;
            _computePredictionPostAddress = configuration["imageProcessing:address"];
            _timeoutPerPhoto = int.Parse(configuration["imageProcessing:timeoutSecondsPerPhoto"]);
        }

        public async Task<IEnumerable<double>> ComputePredictionsAsync(string photoPath)
        {
            return (await ComputePredictionsAsync(new[] { photoPath })).FirstOrDefault().ToArray();
        }

        public async Task<IEnumerable<IEnumerable<double>>> ComputePredictionsAsync(IEnumerable<string> photosPaths)
        {
            var postData = new { photosPaths = photosPaths };
            var httpTimeout = TimeSpan.FromSeconds(photosPaths.Count() * _timeoutPerPhoto);
            var response = await _httpService.Post(_computePredictionPostAddress, postData, httpTimeout);
            var predictions = ((JArray) response)
                .Select(prediction => prediction
                    .Select(predictionElement => double.Parse(predictionElement.ToString(), NumberStyles.Float))
                    .ToArray())
                .ToArray();

            return predictions;
        }

        public async Task<double> CosineSimilarityAsync(double[] first, double[] second)
        {
            if (first.Length != second.Length)
                throw new Exception("Arrays must have same length!");

            var dotProduct = 0d;
            var firstSumOfProducts = 0d;
            var secondSumOfProducts = 0d;

            for (var index = 0; index < first.Length; index++)
            {
                dotProduct += first[index] * second[index];
                firstSumOfProducts += first[index] * first[index];
                secondSumOfProducts += second[index] * second[index];
            }

            var firstEuclideanNorm = Math.Sqrt(firstSumOfProducts);
            var secondEuclideanNorm = Math.Sqrt(secondSumOfProducts);
            var similarity = dotProduct / (firstEuclideanNorm * secondEuclideanNorm);

            return similarity;
        }
    }
}