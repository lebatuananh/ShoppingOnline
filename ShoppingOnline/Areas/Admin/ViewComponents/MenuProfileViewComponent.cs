using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingOnline.WebApplication.Areas.Admin.ViewComponents
{
    [ViewComponent(Name = "MenuProfileVC")]
    public class MenuProfileViewComponent:ViewComponent
    {
        public MenuProfileViewComponent()
        {
            
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.Run(() => View());
        }
    }
}