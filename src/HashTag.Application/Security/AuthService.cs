using System;
using System.Security.Claims;
using System.Threading.Tasks;
using HashTag.Contracts.Services;
using HashTag.Domain.DependencyInjection;
using HashTag.Domain.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace HashTag.Application.Security
{
    [TransientDependency(ServiceType = typeof(IAuthService))]
    internal class AuthService : IAuthService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<SignInResult> SignInAsync(string nameOrEmail, string password, bool persistent)
        {
            var user = await _userManager.FindByNameAsync(nameOrEmail) ?? await _userManager.FindByEmailAsync(nameOrEmail);
            if (user == null)
                throw new Exception("Incorrect username or password.");

            var result = await _signInManager.PasswordSignInAsync(user, password, persistent, true);

            return result;
        }

        public Task<AuthenticationProperties> ConfigureExternalAuthenticationPropertiesAsync(string provider, string callBackUrl)
        {
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, callBackUrl);
            return Task.FromResult(properties);
        }

        public async Task<SignInResult> ExternalSignInAsync()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                throw new Exception("External login info cannot be null.");

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);

            return result;
        }

        public async Task<ExternalLoginInfo> GetExternalLoginInfoAsync()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();

            return info;
        }

        public async Task<IdentityResult> ExternalLoginConfirmation(string email, string userName)
        {
            var info = await GetExternalLoginInfoAsync();
            if (info == null)
                throw new Exception("External login info cannot be null.");

            if (info.Principal.FindFirstValue(ClaimTypes.Email) != email)
                throw new Exception("Email has been changed.");

            var applicationUser = new ApplicationUser(email, userName);
            var result = await _userManager.CreateAsync(applicationUser);
            if (!result.Succeeded)
                return result;

            result = await _userManager.AddLoginAsync(applicationUser, info);
            if (!result.Succeeded)
                return result;

            await _signInManager.SignInAsync(applicationUser, false);

            return result;
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}