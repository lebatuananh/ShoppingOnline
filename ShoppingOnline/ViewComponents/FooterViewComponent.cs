using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingOnline.WebApplication.ViewComponents
{
    [ViewComponent(Name = "FooterClientVC")]
    public class FooterViewComponent:ViewComponent
    {
        public FooterViewComponent()
        {
            
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.Run(() => View());
        }
    }
}