using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace HashTag.Presentation.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet("error")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("error/{code}")]
        public IActionResult FromCode(string code)
        {
            var customHttpStatuCodesPages = new[] { "400", "401", "403", "404", "500" };
            if (customHttpStatuCodesPages.Any(x => x == code))
                return View(code);

            return Redirect("Index");
        }
    }
}