using System.Threading.Tasks;

namespace ShoppingOnline.WebApplication.Services
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(string viewName, object model);
    }
}