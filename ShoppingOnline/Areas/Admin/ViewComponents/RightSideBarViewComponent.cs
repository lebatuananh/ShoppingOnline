using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ShoppingOnline.WebApplication.Areas.Admin.ViewComponents
{
    [ViewComponent(Name ="RightSideBarVC")]
    public class RightSideBarViewComponent : ViewComponent
    {
        public RightSideBarViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.Run(() => View());
        }
    }
}