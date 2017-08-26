using HashTag.Contracts.Services;
using HashTag.Domain.DependencyInjection;
using HashTag.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HashTag.Application.Security
{
    [ScopedDependency(ServiceType = typeof(ICurrentUserAccessor))]
    public class CurrentUserAccessor : ICurrentUserAccessor
    {
        private readonly UserManager<ApplicationUser> _usernaManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private User _user;

        public CurrentUserAccessor(UserManager<ApplicationUser> usernaManager, IHttpContextAccessor httpContextAccessor)
        {
            _usernaManager = usernaManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsAuthenticated => _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;

        public string UserName => IsAuthenticated ? _httpContextAccessor.HttpContext.User.Identity.Name : null;

        public User User => _user ?? (_user = GetApplicationUser(UserName)?.User);

        public ApplicationUser ApplicationUser => User?.ApplicationUser;

        public ApplicationUser GetApplicationUser(string userName)
        {
            var applicationUser = _usernaManager.Users
                .Include(x => x.User)
                .FirstOrDefaultAsync(u => u.UserName == UserName)
                .Result;

            return applicationUser;
        }
    }
}