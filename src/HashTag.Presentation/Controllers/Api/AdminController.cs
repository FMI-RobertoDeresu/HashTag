using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AutoMapper;
using HashTag.Contracts.Loggers;
using HashTag.Contracts.Services;
using HashTag.Presentation.Models;
using HashTag.Presentation.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HashTag.Presentation.Controllers.Api
{
    [Authorize("admin")]
    [Route("api/admin")]
    public class AdminController : ApiBaseController
    {
        private readonly IPhotoService _photoService;
        private readonly IClusterService _clusterService;
        private readonly ISamplesService _samplesService;
        private readonly IResearchService _researchService;
        private readonly IChartService _chartService;
        private readonly IApplicationLogger _appLogger;

        public AdminController(
            IPhotoService photoService,
            IClusterService clusterService,
            ISamplesService samplesService,
            IResearchService researchService,
            IChartService chartService,
            IApplicationLogger appLogger)
        {
            _photoService = photoService;
            _clusterService = clusterService;
            _samplesService = samplesService;
            _researchService = researchService;
            _chartService = chartService;
            _appLogger = appLogger;
        }

        [HttpGet("buildClusters")]
        public async Task<IActionResult> BuildClusters()
        {
            return await AdminAction(async () => { await _clusterService.SaveClustersAsync(); });
        }

        [HttpGet("savePredictionClasses")]
        public async Task<IActionResult> SavePredictionClasses()
        {
            return await AdminAction(async () => { await _samplesService.SavePredictionClassesAsync(); });
        }

        [HttpGet("saveSamplesPhotosPredictions")]
        public async Task<IActionResult> SaveSamplesPhotosPredictions()
        {
            return await AdminAction(async () => { await _samplesService.SavePhotosPredictionsAsync(); });
        }

        [HttpGet("kMeansResearch")]
        public async Task<IActionResult> KMeansResearch()
        {
            return await AdminAction(async () => { await _researchService.KMeansResearchAsync(); });
        }

        [HttpGet("createTestDirectory")]
        public async Task<IActionResult> CreateTestDirectory()
        {
            return await AdminAction(async () => { await _samplesService.CreateTestDirectory(); });
        }

        [HttpGet("bindPhotosToClusters")]
        public async Task<IActionResult> BindPhotosToClusters()
        {
            return await AdminAction(async () => { await _photoService.BindPhotosToClusters(); });
        }

        [HttpGet("getClustersChartData")]
        public async Task<IActionResult> GetClustersChartData()
        {
            return await AdminAction(async () =>
            {
                var chartData = await _chartService.GetClustersChartData();
                var chartDataModel = Mapper.Map<IEnumerable<ClustersChartGroupModel>>(chartData);

                return JsonResponse.SuccessResponse(chartDataModel);
            });
        }

        [HttpGet("getKMeansResearchData")]
        public async Task<IActionResult> GetKMeansResearchData()
        {
            return await AdminAction(async () =>
            {
                var kMeansResults = await _researchService.GetKMeansResearchResultsAsync();
                var kMeansResultsModels = Mapper.Map<IEnumerable<KMeansResearchResultModel>>(kMeansResults);

                return JsonResponse.SuccessResponse(kMeansResultsModels);
            });
        }

        private async Task<IActionResult> AdminAction(Func<Task> action)
        {
            var watch = Stopwatch.StartNew();

            return await AdminAction(async () =>
            {
                await action();
                watch.Stop();

                return JsonResponse.SuccessResponse("Action completed with success!", new { watch });
            });
        }

        private async Task<IActionResult> AdminAction(Func<Task<JsonResponse>> action)
        {
            var watch = Stopwatch.StartNew();
            try
            {
                var result = await action();

                return OkJsonResult(result);
            }
            catch (Exception exception)
            {
                watch.Stop();
                _appLogger.LogError(exception);
                return InternalServerErrorJsonResult(JsonResponse.ErrorResponse(new { exception, watch }));
            }
        }
    }
}