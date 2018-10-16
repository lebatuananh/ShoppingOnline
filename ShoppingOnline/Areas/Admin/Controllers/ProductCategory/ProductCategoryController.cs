using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ShoppingOnline.Application.ECommerce.ProductCategories;
using ShoppingOnline.Application.ECommerce.ProductCategories.Dtos;
using ShoppingOnline.Data.Entities.System;
using ShoppingOnline.Utilities.Helpers;
using ShoppingOnline.WebApplication.Areas.Admin.Controllers.Base;
using ShoppingOnline.WebApplication.Authorization;

namespace ShoppingOnline.WebApplication.Areas.Admin.Controllers.ProductCategory
{
    public class ProductCategoryController : BaseController
    {
        private readonly IProductCategoryService _productCategoryService;
        private readonly IAuthorizationService _authorizationService;
        private readonly SignInManager<AppUser> _signInManager;

        public ProductCategoryController(IProductCategoryService productCategoryService,
            SignInManager<AppUser> signInManager, IAuthorizationService authorizationService)
        {
            _productCategoryService = productCategoryService;
            _signInManager = signInManager;
            _authorizationService = authorizationService;
        }
        
        public async Task<IActionResult> Index()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "PRODUCT_CATEGORY", Operations.Read);
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
            var query = _productCategoryService.GetAll();

            return new OkObjectResult(query);
        }
        
        public IActionResult UpdateParentId(int sourceId, int targetId, Dictionary<int, int> items)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            else
            {
                if (sourceId == targetId)
                {
                    return new BadRequestResult();
                }
                else
                {
                    _productCategoryService.UpdateParentId(sourceId, targetId, items);
                    _productCategoryService.Save();
                    return new OkResult();
                }
            }
        }
        
        [HttpPost]
        public IActionResult ReOrder(int sourceId, int targetId)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            else
            {
                if (sourceId == targetId)
                {
                    return new BadRequestResult();
                }
                else
                {
                    _productCategoryService.ReOrder(sourceId, targetId);
                    _productCategoryService.Save();
                    return new OkResult();
                }
            }
        }
        
        [HttpGet]
        public IActionResult GetById(int id)
        {
            var model = _productCategoryService.GetById(id);

            return new OkObjectResult(model);
        }
        
        [HttpPost]
        public IActionResult SaveEntity(ProductCategoryViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(n => n.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            else
            {
                viewModel.SeoAlias = TextHelper.ToUnsignString(viewModel.Name);

                if (viewModel.Id == 0)
                    _productCategoryService.Add(viewModel);
                else
                    _productCategoryService.Update(viewModel);
            }

            _productCategoryService.Save();

            return new OkObjectResult(viewModel);
        }
        
        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return new BadRequestResult();
            }
            else
            {
                _productCategoryService.Delete(id);
                _productCategoryService.Save();
                return new OkResult();
            }
        }
        
        
    }
}