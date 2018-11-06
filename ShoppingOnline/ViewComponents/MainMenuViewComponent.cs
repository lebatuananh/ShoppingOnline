using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ShoppingOnline.Application.ECommerce.ProductCategories;
using ShoppingOnline.Infrastructure.Enum;

namespace ShoppingOnline.WebApplication.ViewComponents
{
    public class MainMenuViewComponent : ViewComponent
    {
        private IProductCategoryService _productCategoryService;
        private readonly IMemoryCache _memoryCache;

        public MainMenuViewComponent(IProductCategoryService productCategoryService, IMemoryCache memoryCache)
        {
            _productCategoryService = productCategoryService;
            _memoryCache = memoryCache;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = _memoryCache.GetOrCreate(CacheKey.ProductCategories, entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromHours(2);
                return _productCategoryService.GetAll();
            });

            return View(_productCategoryService.GetAll());
        }
    }
}