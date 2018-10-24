using System.Collections.Generic;
using System.Linq;
using HashTag.Contracts.Loggers;
using HashTag.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace HashTag.Presentation.Controllers
{
    public class PagesController : BaseController
    {

        public PagesController(
            IAuthService authService,
            IUserService userService,
            IApplicationLogger appLogger)
        {
        }

        [HttpGet("/")]
        [HttpGet("/search/{hashtag?}")]
        public IActionResult Search(string hashTag)
        {
            return View(nameof(Search), hashTag);
        }

        [HttpGet("/Profile/{userName}")]
        public IActionResult Profile(string userName)
        {
            return View(nameof(Profile), userName);
        }

        [HttpGet("/Admin")]
        [Authorize("admin")]
        public IActionResult Admin()
        {
            return View(nameof(Admin));
        }
    }
}