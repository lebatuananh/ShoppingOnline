using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Utilities.Constants;

namespace ShoppingOnline.WebApplication.Areas.Admin.Controllers.Base
{
    [Area("Admin")]
    [Authorize]
    public class BaseController : Controller
    {
    }
}