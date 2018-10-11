using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingOnline.WebApplication.ViewComponents.Home
{
    [ViewComponent(Name = "ProductContentVC")]
    public class ProductContent:ViewComponent
    {
        public ProductContent()
        {
            
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // ReSharper disable once Mvc.ViewComponentViewNotResolved
            return await Task.Run(() => View());
        }
    }
}