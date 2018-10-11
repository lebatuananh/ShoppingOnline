using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingOnline.WebApplication.ViewComponents
{
    [ViewComponent(Name = "NavigationVC")]
    public class NavigationViewComponent:ViewComponent
    {
        public NavigationViewComponent()
        {
            
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.Run(() => View());
        }
    }
}