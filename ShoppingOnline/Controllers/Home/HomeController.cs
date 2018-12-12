using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ShoppingOnline.Application.Common;
using ShoppingOnline.Application.Content.Blogs;
using ShoppingOnline.Application.ECommerce.ProductCategories;
using ShoppingOnline.Application.ECommerce.Products;
using ShoppingOnline.WebApplication.Models;

namespace ShoppingOnline.WebApplication.Controllers.Home
{
    public class HomeController : Controller
    {
        private readonly IProductCategoryService _productCategoryService;
        private readonly IProductService _productService;
        private readonly IBlogService _blogService;
        private readonly ICommonService _commonService;
        private readonly IStringLocalizer<HomeController> _localizer;


        public HomeController(IProductCategoryService productCategoryService, IProductService productService,
            IBlogService blogService, ICommonService commonService, IStringLocalizer<HomeController> localizer)
        {
            this._productCategoryService = productCategoryService;
            this._productService = productService;
            this._blogService = blogService;
            this._commonService = commonService;
            this._localizer = localizer;
        }

        [ResponseCache(CacheProfileName = "Default")]
        public IActionResult Index()
        {
            var title = _localizer["Title"];
            var culture = HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;

            ViewData["BodyClass"] = "cms-index-index cms-home-page";
            var homeViewModel = new HomeViewModel();
            homeViewModel.HomeCategories = _productCategoryService.GetHomeCategories(4);
            homeViewModel.HotProducts = _productService.GetHotProduct(5);
            homeViewModel.TopSellProducts = _productService.GetLastest(5);
            homeViewModel.LastestBlogs = _blogService.GetLastest(5);
            homeViewModel.HomeSlides = _commonService.GetSlides("top");
            homeViewModel.Advertisement = _commonService.GetAdvertistment();
            homeViewModel.Title = title;
            homeViewModel.MetaDescription = string.Empty;
            homeViewModel.MetaKeyword = string.Empty;
            return View(homeViewModel);
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions {Expires = DateTimeOffset.UtcNow.AddYears(1)}
            );

            return LocalRedirect(returnUrl);
        }

        public IActionResult SearchQuery(string q)
        {
            var result = _productService.SearchQuery(q);
            return new OkObjectResult(result);
        }
    }
}