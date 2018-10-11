using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingOnline.WebApplication.ViewComponents.Home
{
    [ViewComponent(Name = "OurFeatureVC")]
    public class OurFeatureViewComponent:ViewComponent
    {
        public OurFeatureViewComponent()
        {
            
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // ReSharper disable once Mvc.ViewComponentViewNotResolved
            return await Task.Run(() => View());
        }
    }
}