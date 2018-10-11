using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingOnline.WebApplication.ViewComponents.Home
{
    [ViewComponent(Name = "ProductViewedVC")]
    public class ProductViewed:ViewComponent
    {
        public ProductViewed()
        {
            
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // ReSharper disable once Mvc.ViewComponentViewNotResolved
            return await Task.Run(() => View());
        }
    }
}