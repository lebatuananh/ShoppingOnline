using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ShoppingOnline.Application.Systems.Users;
using ShoppingOnline.Application.Systems.Users.Dtos;
using ShoppingOnline.Data.Entities.System;
using ShoppingOnline.Utilities.Dtos;
using ShoppingOnline.WebApplication.Areas.Admin.Controllers.Base;
using ShoppingOnline.WebApplication.Extensions;
using ShoppingOnline.WebApplication.Models.AccountViewModel;

namespace ShoppingOnline.WebApplication.Areas.Admin.Controllers.Logout
{
    public class AccountController : BaseController
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IAppUserService _userService;

        public AccountController(SignInManager<AppUser> signInManager, IAppUserService userService)
        {
            this._signInManager = signInManager;
            this._userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Account()
        {
            var id = User.GetSpecificClaim("UserId");
            var model = await _userService.GetByIdAsync(id);
            return new OkObjectResult(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(AppUserViewModel userVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            else
            {
                var isValid = await _userService.UpdateAccount(userVm);
                if (isValid == false)
                    return new OkObjectResult(new GenericResult(false, userVm));

                return new OkObjectResult(new GenericResult(true, userVm));
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(PasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            else
            {
                var userId = User.GetUserId();

                var isValid =
                    await _userService.ChangePassword(userId.ToString(), model.CurrentPassword, model.Password);

                if (isValid)
                {
                    await _signInManager.SignOutAsync();
                    return new OkObjectResult(new GenericResult(true));
                }
                else
                {
                    return new OkObjectResult(new GenericResult(false));
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/Admin/Login/Index");
        }
    }
}