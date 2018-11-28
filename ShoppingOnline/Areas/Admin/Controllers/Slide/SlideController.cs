using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ShoppingOnline.Application.Common.Slides;
using ShoppingOnline.Application.Common.Slides.Dtos;
using ShoppingOnline.Data.Entities.System;
using ShoppingOnline.WebApplication.Areas.Admin.Controllers.Base;
using ShoppingOnline.WebApplication.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingOnline.WebApplication.Areas.Admin.Controllers.Slide
{
    public class SlideController : BaseController
    {
        private readonly ISlideService _slideService;
        private readonly IAuthorizationService _authorizationService;
        private readonly SignInManager<AppUser> _signInManager;

        public SlideController(ISlideService slideService, IAuthorizationService authorizationService, SignInManager<AppUser> signInManager)
        {
            _slideService = slideService;
            this._authorizationService = authorizationService;
            this._signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            if ((await _authorizationService.AuthorizeAsync(User, "SLIDE", Operations.Read)).Succeeded == false)
            {
                await _signInManager.SignOutAsync();
                return new RedirectResult("/Admin/Login/Index");
            }
            return View();
        }

        [HttpGet]
        public IActionResult GetById(int id)
        {
            var model = _slideService.GetById(id);

            return new OkObjectResult(model);
        }

        [HttpGet]
        public IActionResult GetAllPaging(string keyword, int page, int pageSize)
        {
            var model = _slideService.GetAllPaging(keyword, page, pageSize);
            return new OkObjectResult(model);
        }

        [HttpPost]
        public IActionResult SaveEntity(SlideViewModel pageVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            if (pageVm.Id == 0)
            {
                _slideService.Add(pageVm);
            }
            else
            {
                _slideService.Update(pageVm);
            }

            _slideService.Save();
            return new OkObjectResult(pageVm);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            _slideService.Delete(id);
            _slideService.Save();

            return new OkObjectResult(id);
        }
    }
}