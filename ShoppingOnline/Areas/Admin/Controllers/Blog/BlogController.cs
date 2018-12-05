using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using ShoppingOnline.Application.Content.Blogs;
using ShoppingOnline.Application.Content.Blogs.Dtos;
using ShoppingOnline.Data.Entities.System;
using ShoppingOnline.Utilities.Helpers;
using ShoppingOnline.WebApplication.Areas.Admin.Controllers.Base;
using ShoppingOnline.WebApplication.Authorization;

namespace ShoppingOnline.WebApplication.Areas.Admin.Controllers.Blog
{
    public class BlogController : BaseController
    {
        private readonly IBlogService _blogService;
        private readonly IAuthorizationService _authorizationService;
        private readonly SignInManager<AppUser> _signInManager;

        public BlogController(IBlogService blogService, IAuthorizationService authorizationService,
            SignInManager<AppUser> signInManager)
        {
            _blogService = blogService;
            _authorizationService = authorizationService;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "BLOG", Operations.Read);

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
            var model = _blogService.GetAll();
            return new OkObjectResult(model);
        }

        [HttpGet]
        public IActionResult GetTags(string text)
        {
            var model = _blogService.GetListTag(text);
            return new OkObjectResult(model);
        }

        [HttpGet]
        public IActionResult GetById(int id)
        {
            var model = _blogService.GetById(id);
            return new OkObjectResult(model);
        }

        [HttpGet]
        public IActionResult GetAllPaging(string keyword, int page, int pageSize)
        {
            var model = _blogService.GetAllPaging(keyword, pageSize, page);
            return new OkObjectResult(model);
        }

        [HttpPost]
        public IActionResult SaveEntity(BlogViewModel blogViewModel)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            else
            {
                blogViewModel.SeoAlias = TextHelper.ToUnsignString(blogViewModel.Name);
                if (blogViewModel.Id == 0)
                {
                    _blogService.Add(blogViewModel);
                }
                else
                {
                    _blogService.Update(blogViewModel);
                }

                _blogService.Save();
                return new OkObjectResult(blogViewModel);
            }
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
                _blogService.Delete(id);
                _blogService.Save();

                return new OkObjectResult(id);
            }
        }

        [HttpDelete]
        public IActionResult DeleteMulti(string checkedProducts)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            else
            {
                var listProductCategory = JsonConvert.DeserializeObject<List<int>>(checkedProducts);
                foreach (var item in listProductCategory)
                {
                    _blogService.Delete(item);
                }

                _blogService.Save();

                return new OkResult();
            }
        }
    }
}