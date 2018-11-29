using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Application.Content.Pages;

namespace ShoppingOnline.WebApplication.Controllers.Page
{
    public class PageController:Controller
    {
        private IPageService _pageService;

        public PageController(IPageService pageService)
        {
            _pageService = pageService;
        }

        [Route("page/{alias}.html", Name = "Page")]
        public IActionResult Index(string alias)
        {
            var page = _pageService.GetByAlias(alias);
            return View(page);
        }
    }
}