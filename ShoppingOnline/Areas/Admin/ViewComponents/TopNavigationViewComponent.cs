using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingOnline.WebApplication.Areas.Admin.ViewComponents
{
    [ViewComponent(Name = "TopNavigationVC")]
    public class TopNavigationViewComponent:ViewComponent
    {
        public TopNavigationViewComponent()
        {
            
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.Run(() => View());
        }
    }
}