using System;
using System.Threading.Tasks;
using HashTag.Contracts.Loggers;
using HashTag.Contracts.Services;
using HashTag.Domain.Models;
using HashTag.Infrastructure.Exceptions;
using HashTag.Infrastructure.Extensions;
using HashTag.Presentation.Models.Manage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HashTag.Presentation.Controllers
{
    [Authorize]
    public class ManageController : BaseController
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userService;
        private readonly ICurrentUserAccessor _currentUserAccessor;
        private readonly IApplicationLogger _appLogger;

        public ManageController(
            SignInManager<ApplicationUser> signInManager,
            IUserService userService,
            ICurrentUserAccessor currentUserAccessor,
            IApplicationLogger appLogger)
        {
            _signInManager = signInManager;
            _userService = userService;
            _currentUserAccessor = currentUserAccessor;
            _appLogger = appLogger;
        }

        [HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            var model = new SetPasswordModel(_currentUserAccessor.User.Id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordModel model)
        {
            if (!ModelState.IsValid)
                return View(model).WithError(JoinWithHtmlLineBreak(ModelState.GetErrorMessages()));

            try
            {
                var result = await _userService.SetPasswordAsync(model.UserId, model.Password);

                return result.Succeeded
                    ? RedirectToDefault.WithSuccess("Password was set with success.")
                    : View(model).WithError(JoinWithHtmlLineBreak(result.GetAllErrors()));
            }
            catch (ValidationException validationException)
            {
                return View(model).WithError(validationException.Message);
            }
            catch (Exception exception)
            {
                _appLogger.LogError(exception);
                return View(model).WithError(MessagesOptions.GenericErrorMessage);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditUser()
        {
            var model = new EditUserModel(_currentUserAccessor.User.Id, _currentUserAccessor.User.UserName);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserModel model)
        {
            if (!model.WithPasswordChange)
                ModelState
                    .ClearKey(nameof(model.CurrentPassword))
                    .ClearKey(nameof(model.NewPassword))
                    .ClearKey(nameof(model.NewPasswordRepeat));

            if (!ModelState.IsValid)
                return View(model).WithError(JoinWithHtmlLineBreak(ModelState.GetErrorMessages()));

            try
            {
                var result = model.WithPasswordChange
                    ? await _userService.EditAsync(model.UserId, model.UserName, model.CurrentPassword, model.NewPassword)
                    : await _userService.EditAsync(model.UserId, model.UserName);

                if (result.Succeeded)
                {
                    await _signInManager.SignOutAsync();
                    await _signInManager.SignInAsync(_currentUserAccessor.ApplicationUser, true);
                }

                return result.Succeeded 
                    ? RedirectToDefault.WithSuccess("Edited with success.")
                    : View(model).WithError(JoinWithHtmlLineBreak(result.GetAllErrors()));
            }
            catch (ValidationException validationException)
            {
                return View(model).WithError(validationException.Message);
            }
            catch (Exception exception)
            {
                _appLogger.LogError(exception);
                return View(model).WithError(MessagesOptions.GenericErrorMessage);
            }
        }
    }
}