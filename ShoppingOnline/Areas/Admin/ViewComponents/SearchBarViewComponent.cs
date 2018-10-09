using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ShoppingOnline.WebApplication.Areas.Admin.ViewComponents
{
    [ViewComponent(Name = "SearchBarVC")]
    public class SearchBarViewComponent : ViewComponent
    {
        public SearchBarViewComponent()
        {

        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.Run(() => View());
        }
    }
}