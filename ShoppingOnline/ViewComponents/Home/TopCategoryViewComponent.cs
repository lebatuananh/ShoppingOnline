using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingOnline.WebApplication.ViewComponents.Home
{
    [ViewComponent(Name = "TopCategoryVC")]
    public class TopCategoryViewComponent:ViewComponent
    {
        public TopCategoryViewComponent()
        {
            
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // ReSharper disable once Mvc.ViewComponentViewNotResolved
            return await Task.Run(() => View());
        }
    }
}