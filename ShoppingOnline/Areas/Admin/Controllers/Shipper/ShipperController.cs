using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ShoppingOnline.Application.Systems.Shippers;
using ShoppingOnline.Application.Systems.Shippers.Dtos;
using ShoppingOnline.Data.Entities.System;
using ShoppingOnline.WebApplication.Areas.Admin.Controllers.Base;
using ShoppingOnline.WebApplication.Authorization;

namespace ShoppingOnline.WebApplication.Areas.Admin.Controllers.Shipper
{
    public class ShipperController : BaseController
    {
        private readonly IShipperService _shipperService;
        private readonly IAuthorizationService _authorizationService;
        private readonly SignInManager<AppUser> _signInManager;

        public ShipperController(IShipperService shipperService, IAuthorizationService authorizationService,
            SignInManager<AppUser> signInManager)
        {
            _shipperService = shipperService;
            _authorizationService = authorizationService;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "SHIPPER", Operations.Read);
            if (result.Succeeded == false)
            {
                await _signInManager.SignOutAsync();
                return new RedirectResult("/Admin/Login/Index");
            }

            return View();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var query = _shipperService.GetAll();

            return new OkObjectResult(query);
        }

        [HttpGet]
        public IActionResult GetById(int id)
        {
            var model = _shipperService.GetById(id);

            return new OkObjectResult(model);
        }

        [HttpPost]
        public IActionResult SaveEntity(ShipperViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(n => n.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            else
            {
                if (viewModel.Id == 0)
                    _shipperService.Add(viewModel);
                else
                    _shipperService.Update(viewModel);
            }

            _shipperService.Save();

            return new OkObjectResult(viewModel);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            else
            {
                _shipperService.Delete(id);
                _shipperService.Save();
                return new OkResult();
            }
        }
        
        [HttpGet]
        public IActionResult GetAllPaging(string keyword, int page, int pageSize)
        {
            var model = _shipperService.GetAllPaging(keyword, page, pageSize);
            return new OkObjectResult(model);
        }
       
    }
}