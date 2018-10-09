using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ShoppingOnline.WebApplication.Areas.Admin.ViewComponents
{
    [ViewComponent(Name = "TopBarVC")]
    public class TopBarViewComponent : ViewComponent
    {
        public TopBarViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.Run(() => View());
        }
    }
}