using System.Net;
using HashTag.Infrastructure.ActionResults;
using HashTag.Presentation.Models;
using Microsoft.AspNetCore.Mvc;

namespace HashTag.Presentation.Controllers.Api
{
    public class ApiBaseController : BaseController
    {
        private static IActionResult JsonResult(JsonResponse response, HttpStatusCode code)
            => new JsonCamelCaseResult(response, code);

        protected IActionResult OkJsonResult(JsonResponse response)
            => JsonResult(response, HttpStatusCode.OK);

        protected IActionResult ReadJsonResult(JsonResponse response)
            => JsonResult(response, HttpStatusCode.OK);

        protected IActionResult CreatedJsonResult(JsonResponse response)
            => JsonResult(response, HttpStatusCode.Created);

        protected IActionResult UpdateJsonResult(JsonResponse response)
            => JsonResult(response, HttpStatusCode.OK);

        protected IActionResult DeleteJsonResult(JsonResponse response)
            => JsonResult(response, HttpStatusCode.NoContent);

        protected IActionResult BadRequestJsonResult(JsonResponse response)
            => JsonResult(response, HttpStatusCode.BadRequest);

        protected IActionResult InternalServerErrorJsonResult(JsonResponse response)
            => JsonResult(response, HttpStatusCode.InternalServerError);
    }
}