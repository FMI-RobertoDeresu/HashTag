using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HashTag.Contracts.Loggers;
using HashTag.Contracts.Services;
using HashTag.Domain.Dtos;
using HashTag.Infrastructure.Extensions;
using HashTag.Presentation.Models;
using HashTag.Presentation.Models.Photo;
using HashTag.Presentation.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace HashTag.Presentation.Controllers.Api
{
    [Route("api/users/")]
    public class UsersController : ApiBaseController
    {
        private readonly ICurrentUserAccessor _currentUserAccessor;
        private readonly IUserService _userService;
        private readonly IApplicationLogger _appLogger;

        public UsersController(
            ICurrentUserAccessor currentUserAccessor,
            IUserService userService,
            IApplicationLogger appLogger)
        {
            _currentUserAccessor = currentUserAccessor;
            _userService = userService;
            _appLogger = appLogger;
        }

        [HttpGet("get/{userName}")]
        public async Task<IActionResult> Get(string userName)
        {
            try
            {
                var user = await _userService.GetWithPhotoAsync(userName);
                var profilePhotoModel = Mapper.Map<PhotoModel>(Mapper.Map<PhotoDto>(user.ProfilePhoto));
                profilePhotoModel?.SetAddress(Url);
                var result = new
                {
                    FullName = user.UserName,
                    UserName = user.UserName,
                    ProfilePhoto = profilePhotoModel,
                    CurrentUserName = _currentUserAccessor.UserName
                };

                return ReadJsonResult(JsonResponse.SuccessResponse(result));
            }
            catch (Exception exception)
            {
                _appLogger.LogError(exception);
                return InternalServerErrorJsonResult(JsonResponse.ErrorResponse(exception));
            }
        }

        [HttpGet("getProfilePhotos/{userName}/{currentFeedSize}")]
        public async Task<IActionResult> GetProfilePhotos(string userName, int currentFeedSize)
        {
            try
            {
                var photosDtos = await _userService.GetPhotosAsync(userName, currentFeedSize);
                var photosModels = Mapper.Map<IEnumerable<PhotoModel>>(photosDtos);
                photosModels.ForEach(x => x.SetAddress(Url));

                return OkJsonResult(JsonResponse.SuccessResponse(photosModels));
            }
            catch (Exception exception)
            {
                _appLogger.LogError(exception);
                return InternalServerErrorJsonResult(JsonResponse.ErrorResponse(exception));
            }
        }

        [HttpPost("setProfilePhoto")]
        public async Task<IActionResult> SetProfilePhoto([FromBody] SetProfilePhotoModel model)
        {
            if (!model.Id.HasValue)
                return BadRequestJsonResult(JsonResponse.ErrorResponse("Photo id not specified!"));

            try
            {
                var result = await _userService.SetProfilePhoto(model.Id.Value);
                if (!result.Succeeded)
                    return BadRequestJsonResult(JsonResponse.ErrorResponse(result.Errors.Select(x => x.Description)));

                return OkJsonResult(JsonResponse.SuccessResponse("Profile photo was changed."));
            }
            catch (Exception exception)
            {
                _appLogger.LogError(exception);
                return BadRequest(JsonResponse.ErrorResponse(MessagesOptions.GenericErrorMessage));
            }
        }
    }
}