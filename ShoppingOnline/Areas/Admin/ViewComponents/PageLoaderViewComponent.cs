using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ShoppingOnline.WebApplication.Areas.Admin.ViewComponents
{
    [ViewComponent(Name = "PageLoaderVC")]
    public class PageLoaderViewComponent : ViewComponent
    {
        public PageLoaderViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.Run(() => View());
        }
    }
}