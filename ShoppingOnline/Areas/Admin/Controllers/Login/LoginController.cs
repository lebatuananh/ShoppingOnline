using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.WebApplication.Areas.Admin.Controllers.Base;

namespace ShoppingOnline.WebApplication.Areas.Admin.Controllers.Login
{
    public class LoginController:BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}