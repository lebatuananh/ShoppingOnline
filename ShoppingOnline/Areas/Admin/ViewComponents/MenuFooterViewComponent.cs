using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingOnline.WebApplication.Areas.Admin.ViewComponents
{
    [ViewComponent(Name = "MenuFooterVC")]
    public class MenuFooterViewComponent : ViewComponent
    {
        public MenuFooterViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.Run(() => View());
        }
    }
}