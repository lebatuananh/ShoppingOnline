using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingOnline.WebApplication.Areas.Admin.ViewComponents
{
    [ViewComponent(Name ="LeftSideBarVC")]
    public class LeftSideBarViewComponent:ViewComponent
    {
        public LeftSideBarViewComponent()
        {

        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.Run(() => View());
        }
    }
}
