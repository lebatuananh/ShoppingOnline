using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Application.ECommerce.ProductCategories;

namespace ShoppingOnline.WebApplication.ViewComponents
{
    public class CategoryMenuViewComponent: ViewComponent
    {
        private readonly IProductCategoryService _productCategoryService;

        public CategoryMenuViewComponent(IProductCategoryService productCategoryService)
        {
            this._productCategoryService = productCategoryService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(_productCategoryService.GetAll());
        }
    }
}