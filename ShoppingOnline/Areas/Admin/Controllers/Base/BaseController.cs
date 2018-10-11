using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingOnline.WebApplication.Areas.Admin.Controllers.Base
{
    [Area("Admin")]
    [Authorize]
    public class BaseController:Controller
    {
        
    }
}