using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using HashTag.Contracts.Loggers;
using HashTag.Contracts.Services;
using HashTag.Infrastructure.Extensions;
using HashTag.Presentation.Models;
using HashTag.Presentation.Models.Photo;
using HashTag.Presentation.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace HashTag.Presentation.Controllers.Api
{
    [Route("api/search")]
    public class SearchController : ApiBaseController
    {
        private readonly ISearchService _searchService;
        private readonly IApplicationLogger _appLogger;

        public SearchController(ISearchService searchService, IApplicationLogger appLogger)
        {
            _searchService = searchService;
            _appLogger = appLogger;
        }

        [HttpPost]
        public async Task<IActionResult> Default([FromBody] SearchBaseModel model)
        {
            return await SearchAction(model, async input =>
            {
                var photosDtos = await _searchService.GetPhotosAsync(input.CurrentFeedSize);
                var photosModels = Mapper.Map<IEnumerable<PhotoModel>>(photosDtos);
                return photosModels;
            });
        }

        [HttpPost("byHashTag")]
        public async Task<IActionResult> ByHashTag([FromBody] SearchByHashTagModel model)
        {
            return await SearchAction(model, async input =>
            {
                var hashTag = input.HashTag.Replace("#", "").Trim();
                var photosDtos = await _searchService.GetPhotosByHashTagAsync(hashTag, input.CurrentFeedSize);
                var photosModels = Mapper.Map<IEnumerable<PhotoModel>>(photosDtos);
                return photosModels;
            });
        }

        [HttpPost("byDescription")]
        public async Task<IActionResult> ByDescription([FromBody] SearchByDescriptionModel model)
        {
            return await SearchAction(model, async input =>
            {
                var photosDtos = await _searchService.GetPhotosByDescriptionAsync(input.Description, input.CurrentFeedSize);
                var photosModels = Mapper.Map<IEnumerable<PhotoModel>>(photosDtos);
                return photosModels;
            });
        }

        [HttpPost("byPrediction")]
        public async Task<IActionResult> ByPrediction([FromBody] SearchByPredictionModel model)
        {
            return await SearchAction(model, async input =>
            {
                var photosDtos = await _searchService.GetPhotosByPredictionAsync(input.Prediction, input.CurrentFeedSize);
                var photosModels = Mapper.Map<IEnumerable<PhotoModel>>(photosDtos);
                return photosModels;
            });
        }

        private async Task<IActionResult> SearchAction<TRequest>(TRequest model,
            Func<TRequest, Task<IEnumerable<PhotoModel>>> innerSearchAction)
        {
            try
            {
                var photosModels = await innerSearchAction(model);
                photosModels.ForEach(x => x.SetAddress(Url));

                return OkJsonResult(JsonResponse.SuccessResponse(photosModels));
            }
            catch (Exception exception)
            {
                _appLogger.LogError(exception);
                return InternalServerErrorJsonResult(JsonResponse.ErrorResponse(MessagesOptions.GenericErrorMessage));
            }
        }
    }
}