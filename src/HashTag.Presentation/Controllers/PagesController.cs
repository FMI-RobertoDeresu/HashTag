using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HashTag.Presentation.Controllers
{
    public class PagesController : BaseController
    {
        [HttpGet]
        public IActionResult Search(string hashTag)
        {
            return View(nameof(Search), hashTag);
        }

        [HttpGet]
        public IActionResult Profile(string userName)
        {
            return View(nameof(Profile), userName);
        }

        [HttpGet]
        [Authorize("admin")]
        public IActionResult Admin()
        {
            return View(nameof(Admin));
        }
    }
}