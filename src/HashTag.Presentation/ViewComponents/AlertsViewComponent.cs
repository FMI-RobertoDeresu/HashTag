using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HashTag.Presentation.ViewComponents
{
    public class Alerts : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}