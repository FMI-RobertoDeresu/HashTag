using System.Collections.Generic;
using HashTag.Presentation.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HashTag.Presentation.Controllers
{
    public abstract class BaseController : Controller
    {
        protected MessagesOptions MessagesOptions
            => HttpContext.RequestServices.GetRequiredService<IOptions<MessagesOptions>>().Value;

        protected IActionResult RedirectToDefault => Redirect("/");

        protected string GenericErrorMessage => MessagesOptions.GenericErrorMessage;

        protected string JoinWithHtmlLineBreak(IEnumerable<string> errors) => string.Join("<br/>", errors);
    }
}