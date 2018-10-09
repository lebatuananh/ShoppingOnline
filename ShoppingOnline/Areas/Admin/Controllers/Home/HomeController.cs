using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Areas.Admin.Controllers.Base;

namespace ShoppingOnline.Areas.Admin.Controllers.Home
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}