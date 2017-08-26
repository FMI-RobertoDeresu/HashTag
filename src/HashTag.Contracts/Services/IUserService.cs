using System.Collections.Generic;
using System.Threading.Tasks;
using HashTag.Domain.Dtos;
using HashTag.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace HashTag.Contracts.Services
{
    public interface IUserService
    {
        Task<IdentityResult> CreateAsync(UserCreateDto user);
        Task<IdentityResult> SetProfilePhoto(long photoId);

        Task<User> GetAsync(string userName);
        Task<User> GetWithPhotoAsync(string userName);
        Task<IEnumerable<PhotoDto>> GetPhotosAsync(string userName, int skip);

        Task<IdentityResult> SetPasswordAsync(long id, string password);
        Task<IdentityResult> EditAsync(long id, string userName);
        Task<IdentityResult> EditAsync(long id, string userName, string currentPassword, string newPassword);
    }
}