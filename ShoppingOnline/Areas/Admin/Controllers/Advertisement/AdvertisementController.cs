using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ShoppingOnline.Application.Common.Advertisements;
using ShoppingOnline.Application.Common.Advertisements.Dtos;
using ShoppingOnline.Data.Entities.System;
using ShoppingOnline.WebApplication.Areas.Admin.Controllers.Base;
using ShoppingOnline.WebApplication.Authorization;

namespace ShoppingOnline.WebApplication.Areas.Admin.Controllers.Advertisement
{
    public class AdvertisementController : BaseController
    {
        private readonly IAdvertisementService _advertisementService;
        private readonly IAuthorizationService _authorizationService;
        private readonly SignInManager<AppUser> _signInManager;

        public AdvertisementController(IAdvertisementService advertisementService,
            IAuthorizationService authorizationService, SignInManager<AppUser> signInManager)
        {
            this._advertisementService = advertisementService;
            this._authorizationService = authorizationService;
            this._signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            if ((await _authorizationService.AuthorizeAsync(User, "ADVERTISMENT", Operations.Read)).Succeeded == false)
            {
                await _signInManager.SignOutAsync();
                return new RedirectResult("/Admin/Login/Index");
            }

            return View();
        }

        [HttpGet]
        public IActionResult GetById(int id)
        {
            var model = _advertisementService.GetById(id);

            return new OkObjectResult(model);
        }

        [HttpGet]
        public IActionResult GetAllPaging(string keyword, int page, int pageSize)
        {
            var model = _advertisementService.GetAllPaging(keyword, page, pageSize);
            return new OkObjectResult(model);
        }

        [HttpPost]
        public IActionResult SaveEntity(AdvertisementViewModel advertisementVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            if (advertisementVm.Id == 0)
            {
                _advertisementService.Add(advertisementVm);
            }
            else
            {
                _advertisementService.Update(advertisementVm);
            }

            _advertisementService.Save();
            return new OkObjectResult(advertisementVm);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            _advertisementService.Delete(id);
            _advertisementService.Save();

            return new OkObjectResult(id);
        }
    }
}