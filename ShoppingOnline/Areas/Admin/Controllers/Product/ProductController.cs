using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Application.ECommerce.Products;
using ShoppingOnline.Data.Entities.System;
using ShoppingOnline.WebApplication.Areas.Admin.Controllers.Base;
using ShoppingOnline.WebApplication.Authorization;

namespace ShoppingOnline.WebApplication.Areas.Admin.Controllers.Product
{
    public class ProductController : BaseController
    {
        private IProductService _productService;
        private readonly IAuthorizationService _authorizationService;
        private readonly SignInManager<AppUser> _signInManager;

        public ProductController(IProductService productService, IAuthorizationService authorizationService,
            SignInManager<AppUser> signInManager)
        {
            _productService = productService;
            _authorizationService = authorizationService;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "USER", Operations.Read);

            if (result.Succeeded == false)
            {
                await _signInManager.SignOutAsync();
                return new RedirectResult("/Admin/Login/Index");
            }

            return View();
        }

        [HttpGet]
        public IActionResult GetAllPaging(int? categoryId, string keyword, int page, int pageSize)
        {
            var model = _productService.GetAllPaging(categoryId, keyword, page, pageSize);
            return new OkObjectResult(model);
        }
    }
}