using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HashTag.Contracts.Loggers;
using HashTag.Contracts.Services;
using HashTag.Presentation.Models;
using HashTag.Presentation.Models.Photo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace HashTag.Presentation.Controllers.Api
{
    [Route("api/photos")]
    public class PhotosController : ApiBaseController
    {
        private readonly IPhotoService _photoService;
        private readonly IPhotoProcessingService _photoProcessingService;
        private readonly IFileService _fileService;
        private readonly IApplicationLogger _appLogger;

        private readonly string _tempPhotosAddress;

        public PhotosController(
            IPhotoService photoService,
            IPhotoProcessingService photoProcessingService,
            IFileService fileService,
            IApplicationLogger appLogger,
            IConfiguration configuration)
        {
            _photoService = photoService;
            _photoProcessingService = photoProcessingService;
            _fileService = fileService;
            _appLogger = appLogger;

            _tempPhotosAddress = configuration["appStorage:tempUploadedImages"];
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var photo = await _photoService.GetAsync(id);
                if (photo == null)
                    return NotFound();

                return PhysicalFile(photo.Location, "image/jpg");
            }
            catch (Exception exception)
            {
                _appLogger.LogError(exception);
                return InternalServerErrorJsonResult(JsonResponse.ErrorResponse(exception));
            }
        }

        [HttpPost("prediction")]
        public async Task<IActionResult> Prediction(IFormFile file)
        {
            var tempPhotoPath = string.Empty;

            try
            {
                tempPhotoPath = await _fileService.SaveAsync(file, _tempPhotosAddress);

                var prediction = await _photoProcessingService.ComputePredictionsAsync(tempPhotoPath);
                var response = new { prediction };

                return OkJsonResult(JsonResponse.SuccessResponse(response));
            }
            catch (Exception exception)
            {
                _appLogger.LogError(exception);
                return InternalServerErrorJsonResult(JsonResponse.ErrorResponse(exception));
            }
            finally
            {
                await _fileService.DeleteAsync(tempPhotoPath);
            }
        }

        [HttpPost("descriptionAndHashTags")]
        public async Task<IActionResult> DescriptionAndHashTags(IFormFile file)
        {
            var tempPhotoPath = string.Empty;

            if (!file.ContentType.StartsWith("image/"))
                return BadRequestJsonResult(JsonResponse.ErrorResponse("Uploaded file must be an image."));

            try
            {
                tempPhotoPath = await _fileService.SaveAsync(file, _tempPhotosAddress);

                var prediction = (await _photoProcessingService.ComputePredictionsAsync(tempPhotoPath)).ToArray();
                var description = await _photoService.ComputeDescriptionAsync(prediction);
                var hashTags = await _photoService.ComputeHashTagsAsync(prediction);
                var response = new { description, hashTags = $"#{string.Join("#", hashTags)}" };

                return OkJsonResult(JsonResponse.SuccessResponse(response));
            }
            catch (Exception exception)
            {
                _appLogger.LogError(exception);
                return InternalServerErrorJsonResult(JsonResponse.ErrorResponse(GenericErrorMessage));
            }
            finally
            {
                await _fileService.DeleteAsync(tempPhotoPath);
            }
        }

        [HttpPost("post")]
        public async Task<IActionResult> Post(IFormFile file, string description, string hashTags)
        {
            try
            {
                var id = await _photoService.SaveAsync(file, description, hashTags?.Split('#') ?? new string[0]);
                if (id <= 0)
                    return InternalServerErrorJsonResult(JsonResponse.ErrorResponse("Photo was not added!"));

                var photoDto = await _photoService.GetAsync(id);
                var photoModel = Mapper.Map<PhotoModel>(photoDto);
                photoModel.SetAddress(Url);
                photoModel.ShowActions = true;

                return CreatedJsonResult(JsonResponse.SuccessResponse("Photo was added!", photoModel));
            }
            catch (Exception exception)
            {
                _appLogger.LogError(exception);
                return InternalServerErrorJsonResult(JsonResponse.ErrorResponse(exception));
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody]DeletePhotoModel model)
        {
            if (!model.Id.HasValue)
                return BadRequestJsonResult(JsonResponse.ErrorResponse("Photo id not specified!"));

            try
            {
                await _photoService.DeleteAsync(model.Id.Value);

                return OkJsonResult(JsonResponse.SuccessResponse("Photo was delete with success!"));
            }
            catch (Exception exception)
            {
                _appLogger.LogError(exception);
                return BadRequest(JsonResponse.ErrorResponse(exception));
            }
        }
    }
}