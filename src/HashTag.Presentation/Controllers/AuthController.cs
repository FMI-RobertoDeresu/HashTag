using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using HashTag.Contracts.Loggers;
using HashTag.Contracts.Services;
using HashTag.Domain.Dtos;
using HashTag.Infrastructure.Extensions;
using HashTag.Presentation.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HashTag.Presentation.Controllers
{
    [Route("Auth")]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IApplicationLogger _appLogger;

        public AuthController(
            IAuthService authService,
            IUserService userService,
            IApplicationLogger appLogger)
        {
            _authService = authService;
            _userService = userService;
            _appLogger = appLogger;
        }

        [HttpPost("SignIn")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInModel model)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToDefault;

            if (!ModelState.IsValid)
                return RedirectToDefault.WithError(JoinWithHtmlLineBreak(ModelState.GetErrorMessages()));

            try
            {
                var signInResult = await _authService.SignInAsync(model.NameOrEmail, model.Password, false);

                if (signInResult.IsLockedOut)
                    return View("Lockout");

                return signInResult.Succeeded
                    ? RedirectToDefault
                    : RedirectToDefault.WithError(MessagesOptions.LoginFailed);
            }
            catch (Exception exception)
            {
                _appLogger.LogError(exception);
                return RedirectToDefault.WithError(exception.Message);
            }
        }

        [HttpPost("Register")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToDefault;

            if (!ModelState.IsValid)
                return RedirectToDefault.WithError(JoinWithHtmlLineBreak(ModelState.GetErrorMessages()));

            try
            {
                var createResult = await _userService.CreateAsync(Mapper.Map<UserCreateDto>(model));
                if (!createResult.Succeeded)
                    return RedirectToDefault.WithError(JoinWithHtmlLineBreak(createResult.GetAllErrors()));

                var signInResult = await _authService.SignInAsync(model.UserName, model.Password, false);
                return signInResult.Succeeded
                    ? RedirectToDefault
                    : RedirectToDefault.WithError(GenericErrorMessage);
            }
            catch (Exception exception)
            {
                _appLogger.LogError(exception);
                return RedirectToDefault.WithError(exception.Message);
            }
        }

        [HttpPost("ExternalLogin")]
        public async Task<IActionResult> ExternalLogin(string provider)
        {
            var callbackUrl = Url.Action("ExternalLoginCallback", "Auth");
            var properties = await _authService.ConfigureExternalAuthenticationPropertiesAsync(provider, callbackUrl);

            return Challenge(properties, provider);
        }


        [HttpGet("ExternalLoginCallback")]
        public async Task<IActionResult> ExternalLoginCallback(string remoteError = null)
        {
            if (!string.IsNullOrEmpty(remoteError))
                return RedirectToDefault.WithError(remoteError);

            var result = await _authService.ExternalSignInAsync();

            if (result.Succeeded)
                return RedirectToDefault;

            if (result.IsLockedOut)
                return View("Lockout");

            var externalInfo = await _authService.GetExternalLoginInfoAsync();
            var provider = externalInfo.LoginProvider;
            var email = externalInfo.Principal.FindFirstValue(ClaimTypes.Email);
            var userName = email.Split('@')[0];
            var confirmationModel = new ExternalLoginConfirmationModel
            {
                Provider = provider,
                Email = email,
                UserName = userName
            };

            return View("ExternalLoginConfirmation", confirmationModel);
        }

        [HttpPost("ExternalLoginConfirmation")]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationModel model)
        {
            if (!ModelState.IsValid)
                return View(model).WithError(JoinWithHtmlLineBreak(ModelState.GetErrorMessages()));

            var result = await _authService.ExternalLoginConfirmation(model.Email, model.UserName);

            return result.Succeeded
                ? RedirectToDefault
                : View(model).WithError(JoinWithHtmlLineBreak(result.GetAllErrors()));
        }

        [Authorize]
        [HttpGet("SignOut")]
        public async Task<IActionResult> SignOut()
        {
            await _authService.SignOutAsync();
            return RedirectToDefault;
        }
    }
}