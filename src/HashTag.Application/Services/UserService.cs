using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HashTag.Contracts.Repositories;
using HashTag.Contracts.Services;
using HashTag.Domain.DependencyInjection;
using HashTag.Domain.Dtos;
using HashTag.Domain.Models;
using HashTag.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HashTag.Application.Services
{
    [TransientDependency(ServiceType = typeof(IUserService))]
    internal class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _usernaManager;
        private readonly IUserRepository _userRepository;
        private readonly IPhotoRepository _photoRepository;
        private readonly ICurrentUserAccessor _currentUserAccessor;

        private readonly int _feedSize;

        public UserService(
            UserManager<ApplicationUser> usernaManager,
            IUserRepository userRepository,
            IPhotoRepository photoRepository,
            ICurrentUserAccessor currentUserAccessor,
            IConfiguration configuration)
        {
            _usernaManager = usernaManager;
            _userRepository = userRepository;
            _photoRepository = photoRepository;
            _currentUserAccessor = currentUserAccessor;

            _feedSize = int.Parse(configuration["app:feedSize"]);
        }

        public async Task<IdentityResult> CreateAsync(UserCreateDto userCreate)
        {
            var applicationUser = new ApplicationUser(userCreate.Email, userCreate.UserName);
            var result = await _usernaManager.CreateAsync(applicationUser, userCreate.Password);
            return result;
        }

        public async Task<IdentityResult> SetProfilePhoto(long photoId)
        {
            var photo = await _photoRepository.GetWithUserAync(photoId);
            var user = _currentUserAccessor.User;

            if (photo.CreatedBy != user)
                throw new Exception("Photo cannot be set as profile!");

            user.ProfilePhoto = photo;
            var result = await _usernaManager.UpdateAsync(user.ApplicationUser);

            return result;
        }

        public async Task<User> GetAsync(string userName)
        {
            var user = await _usernaManager.Users.Include(x => x.User).FirstOrDefaultAsync(u => u.UserName == userName);
            if (user?.User == null)
                throw new Exception("User was not found!");

            return user.User;
        }

        public async Task<User> GetWithPhotoAsync(string userName)
        {
            var user = await _usernaManager.Users
                .Include(x => x.User).ThenInclude(x => x.ProfilePhoto)
                .FirstOrDefaultAsync(u => u.UserName == userName);

            if (user?.User == null)
                throw new Exception("User was not found!");

            return user.User;
        }

        public async Task<IEnumerable<PhotoDto>> GetPhotosAsync(string userName, int skip)
        {
            var user = await GetAsync(userName);
            var photos = await _photoRepository.GetPagedByUserAsync(user.Id, skip, _feedSize);
            var photosDto = Mapper.Map<IList<PhotoDto>>(photos);

            foreach (var photoDto in photosDto)
            {
                var photo = photos.FirstOrDefault(x => x.Id == photoDto.Id);
                photoDto.ShowActions = photo.CreatedBy == _currentUserAccessor.User;
            }

            return photosDto;
        }

        public async Task<IdentityResult> SetPasswordAsync(long id, string password)
        {
            var user = await _userRepository.GetWithAppUserAsync(id);
            var appUser = user?.ApplicationUser;
            if (appUser == null)
                throw new Exception("Application must be not null");

            if (_currentUserAccessor.User != user)
                throw new ValidationException("You are not authorized.");

            if (await _usernaManager.HasPasswordAsync(appUser))
                throw new ValidationException("You already have a password.");

            var result = await _usernaManager.AddPasswordAsync(appUser, password);

            return result;
        }

        public async Task<IdentityResult> EditAsync(long id, string userName)
        {
            var user = await _userRepository.GetWithAppUserAsync(id);
            var appUser = user?.ApplicationUser;
            if (appUser == null)
                throw new Exception("Application must be not null");

            if (_currentUserAccessor.User != user)
                throw new ValidationException("You are not authorized.");

            lock (userName)
            {
                var result = _usernaManager.SetUserNameAsync(appUser, userName).Result;
                return result;
            }
        }

        public async Task<IdentityResult> EditAsync(long id, string userName, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetWithAppUserAsync(id);
            var appUser = user?.ApplicationUser;
            if (appUser == null)
                throw new Exception("Application must be not null");

            if (_currentUserAccessor.User != user)
                throw new ValidationException("You are not authorized.");

            IdentityResult result = null;
            if (!string.IsNullOrEmpty(userName))
                result = await EditAsync(id, userName);

            if (result != null && !result.Succeeded)
                return result;

            result = await _usernaManager.ChangePasswordAsync(appUser, currentPassword, newPassword);

            return result;
        }
    }
}