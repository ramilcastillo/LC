using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LifeCouple.WebApi.SampleTokenViews
{
    public class IndexController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View("/SampleTokenViews/Index.cshtml");
        }
    }
}
