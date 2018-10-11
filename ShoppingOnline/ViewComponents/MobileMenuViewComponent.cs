using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingOnline.WebApplication.ViewComponents
{
    [ViewComponent(Name = "MobileMenuVC")]
    public class MobileMenuViewComponent:ViewComponent
    {
        public MobileMenuViewComponent()
        {
            
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.Run(() => View());
        }
    }
}