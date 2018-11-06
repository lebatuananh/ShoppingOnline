using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Application.ECommerce.ProductCategories;

namespace ShoppingOnline.WebApplication.ViewComponents
{
    public class MobileMenuViewComponent:ViewComponent
    {
        private IProductCategoryService _productCategoryService;

        public MobileMenuViewComponent(IProductCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(_productCategoryService.GetAll());
        }
    }
}