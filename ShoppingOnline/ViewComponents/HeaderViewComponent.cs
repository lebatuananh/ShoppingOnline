using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingOnline.WebApplication.ViewComponents
{
    [ViewComponent(Name = "HeaderClientVC")]
    public class HeaderViewComponent:ViewComponent
    {
        public HeaderViewComponent()
        {
            
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.Run(() => View());
        }
    }
}