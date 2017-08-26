using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HashTag.Contracts.Repositories;
using HashTag.Contracts.Services;
using HashTag.Domain.DependencyInjection;
using HashTag.Domain.Models;
using HashTag.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HashTag.Application.Services
{
    [TransientDependency(ServiceType = typeof(ISamplesService))]
    internal class SamplesService : ISamplesService
    {
        private readonly IPhotoProcessingService _photoProcessingService;
        private readonly IPredictionClassRepository _predictionClassRepository;
        private readonly ISamplesRepository _samplePhotoRepository;

        private readonly string _photosLocation;
        private readonly string _photosTokensLocation;
        private readonly string _testPhotosFileLocation;
        private readonly string _trainPhotosFileLocation;
        private readonly string _predictionClassesLocation;
        private readonly string _testPhotosDirectory;

        public SamplesService(
            IPhotoProcessingService photoProcessingService,
            IPredictionClassRepository predictionClassRepository,
            ISamplesRepository samplePhotoRepository,
            IConfiguration configuration)
        {
            _photoProcessingService = photoProcessingService;
            _predictionClassRepository = predictionClassRepository;
            _samplePhotoRepository = samplePhotoRepository;

            _photosLocation = configuration["appStorage:samples:photosLocation"];
            _photosTokensLocation = configuration["appStorage:samples:photosTokensLocation"];
            _testPhotosFileLocation = configuration["appStorage:samples:testPhotosFileLocation"];
            _trainPhotosFileLocation = configuration["appStorage:samples:trainPhotosFileLocation"];
            _predictionClassesLocation = configuration["appStorage:samples:predictionClassesFileLocation"];
            _testPhotosDirectory = configuration["appStorage:samples:testPhotosDirectory"];
        }

        public async Task<IEnumerable<SamplePhoto>> GetTrainPhotosAsync()
        {
            return await GetPhotosByFileAsync(_trainPhotosFileLocation);
        }

        public async Task<IEnumerable<SamplePhoto>> GetTestPhotosAsync()
        {
            return await GetPhotosByFileAsync(_testPhotosFileLocation);
        }

        public async Task SavePredictionClassesAsync()
        {
            var classesFileText = File.ReadAllText(_predictionClassesLocation);
            var classes = await Task.Run(() => JsonConvert.DeserializeObject(classesFileText));

            var predictionClasses = new List<PredictionClass>();
            foreach (var classItem in (JObject) classes)
                predictionClasses.Add(new PredictionClass(int.Parse(classItem.Key), classItem.Value[1].Value<string>()));

            await _predictionClassRepository.DeleteAsync(x => true);
            foreach (var predictionClass in predictionClasses)
                await _predictionClassRepository.CreateAsync(predictionClass);
        }

        public async Task SavePhotosPredictionsAsync()
        {
            var trainPhotosPaths = (await GetPhotosPathsByFileAsync(_trainPhotosFileLocation)).ToList();
            var testPhotosPaths = (await GetPhotosPathsByFileAsync(_testPhotosFileLocation)).ToList();

            var photos = trainPhotosPaths.Union(testPhotosPaths).ToList();

            if (photos.Count != photos.Distinct().Count())
                throw new Exception("Duplicates photos found!");

            var samplePhotos = new List<SamplePhoto>();
            var predictions = await _photoProcessingService.ComputePredictionsAsync(photos);

            foreach (var location in photos)
            {
                var name = GetPhotoName(location);
                var description = await GetPhotoDescriptionAsync(name);
                var prediction = predictions.ElementAt(photos.IndexOf(location)).ToArray();
                var samplePhoto = new SamplePhoto(name, location, description, prediction);
                samplePhotos.Add(samplePhoto);
            }

            await _samplePhotoRepository.DeleteAsync(samplePhoto => true);
            foreach (var samplePhoto in samplePhotos)
                await _samplePhotoRepository.CreateAsync(samplePhoto);
        }

        public async Task CreateTestDirectory()
        {
            var testPhotosPaths = await GetPhotosPathsByFileAsync(_testPhotosFileLocation);

            if (!Directory.Exists(_testPhotosDirectory))
                Directory.CreateDirectory(_testPhotosDirectory);

            Directory.EnumerateFiles(_testPhotosDirectory).ForEach(File.Delete);

            foreach (var testPhotoPath in testPhotosPaths)
            {
                var location = Path.Combine(_testPhotosDirectory, GetPhotoName(testPhotoPath));
                var bytes = File.ReadAllBytes(testPhotoPath);
                File.WriteAllBytes(location, bytes);
            }
        }

        //
        private static string GetPhotoName(string path)
        {
            return path.Split('/', '\\').Last(x => !string.IsNullOrEmpty(x));
        }

        private async Task<string> GetPhotoDescriptionAsync(string photoName)
        {
            var descriptions = File.ReadAllLines(_photosTokensLocation);
            var photoDescriptions = descriptions
                .Where(x => x.StartsWith(photoName))
                .OrderBy(x => x.Length)
                .ToList();
            var description = photoDescriptions.FirstOrDefault()?.Split('@')[1];

            return description;
        }

        private async Task<IEnumerable<string>> GetPhotosPathsByFileAsync(string photosFilePath)
        {
            var samplesNames = File.ReadAllLines(photosFilePath);
            var photosPaths = Directory
                .GetFiles(_photosLocation)
                .Where(photoPath => samplesNames.Any(photoPath.EndsWith))
                .ToList();

            return photosPaths;
        }

        private async Task<IEnumerable<SamplePhoto>> GetPhotosByFileAsync(string fileLocation)
        {
            var photosNames = File.ReadAllLines(fileLocation);
            var photosSamples = await _samplePhotoRepository.GetManyAsync(photoSample => photosNames.Contains(photoSample.Name));

            return photosSamples;
        }
    }
}