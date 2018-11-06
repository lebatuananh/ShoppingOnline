using Microsoft.AspNetCore.Mvc;

namespace ShoppingOnline.WebApplication.Controllers.AjaxContent
{
    public class AjaxContentController:Controller
    {
        public IActionResult HeaderCart()
        {
            return ViewComponent("HeaderCart");
        }
    }
}