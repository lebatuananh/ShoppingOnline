using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.WebApplication.Areas.Admin.Controllers.Base;

namespace ShoppingOnline.WebApplication.Areas.Admin.Controllers.Home
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}