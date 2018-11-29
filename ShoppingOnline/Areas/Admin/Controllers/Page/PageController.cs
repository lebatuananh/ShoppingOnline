using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ShoppingOnline.Application.Content.Pages;
using ShoppingOnline.Application.Content.Pages.Dtos;
using ShoppingOnline.Data.Entities.System;
using ShoppingOnline.WebApplication.Areas.Admin.Controllers.Base;
using ShoppingOnline.WebApplication.Authorization;

namespace ShoppingOnline.WebApplication.Areas.Admin.Controllers.Page
{
    public class PageController : BaseController
    {
        private readonly IPageService _pageService;
        private readonly IAuthorizationService _authorizationService;
        private readonly SignInManager<AppUser> _signInManager;

        public PageController(IPageService pageService, IAuthorizationService authorizationService,
            SignInManager<AppUser> signInManager)
        {
            _pageService = pageService;
            this._authorizationService = authorizationService;
            this._signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            if ((await _authorizationService.AuthorizeAsync(User, "PAGE", Operations.Read)).Succeeded == false)
            {
                await _signInManager.SignOutAsync();
                return new RedirectResult("/Admin/Login/Index");
            }

            return View();
        }

        public IActionResult GetAll()
        {
            var model = _pageService.GetAll();

            return new OkObjectResult(model);
        }

        [HttpGet]
        public IActionResult GetById(int id)
        {
            var model = _pageService.GetById(id);

            return new OkObjectResult(model);
        }

        [HttpGet]
        public IActionResult GetAllPaging(string keyword, int page, int pageSize)
        {
            var model = _pageService.GetAllPaging(keyword, page, pageSize);
            return new OkObjectResult(model);
        }

        [HttpPost]
        public IActionResult SaveEntity(PageViewModel pageVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            if (pageVm.Id == 0)
            {
                _pageService.Add(pageVm);
            }
            else
            {
                _pageService.Update(pageVm);
            }

            _pageService.SaveChanges();
            return new OkObjectResult(pageVm);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            _pageService.Delete(id);
            _pageService.SaveChanges();

            return new OkObjectResult(id);
        }
    }
}