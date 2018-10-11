using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingOnline.WebApplication.ViewComponents.Home
{
    [ViewComponent(Name = "BrandVC")]
    public class BrandViewComponent:ViewComponent
    {
        public BrandViewComponent()
        {
            
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // ReSharper disable once Mvc.ViewComponentViewNotResolved
            return await Task.Run(() => View());
        }
    }
}