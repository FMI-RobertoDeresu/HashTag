using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace HashTag.Contracts.Services
{
    public interface IAuthService
    {
        Task<SignInResult> SignInAsync(string nameOrEmail, string password, bool persistent);
        Task SignOutAsync();

        //external
        Task<AuthenticationProperties> ConfigureExternalAuthenticationPropertiesAsync(string provider, string callbackUrl);
        Task<ExternalLoginInfo> GetExternalLoginInfoAsync();
        Task<SignInResult> ExternalSignInAsync();
        Task<IdentityResult> ExternalLoginConfirmation(string email, string userName);
    }
}