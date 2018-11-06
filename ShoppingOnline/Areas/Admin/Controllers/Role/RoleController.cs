using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.SignalR;
using ShoppingOnline.Application.Systems.Announcements.Dtos;
using ShoppingOnline.Application.Systems.Permissions.Dtos;
using ShoppingOnline.Application.Systems.Roles;
using ShoppingOnline.Application.Systems.Roles.Dtos;
using ShoppingOnline.Application.Systems.Users.Dtos;
using ShoppingOnline.Data.Entities.System;
using ShoppingOnline.Utilities.Dtos;
using ShoppingOnline.WebApplication.Areas.Admin.Controllers.Base;
using ShoppingOnline.WebApplication.Authorization;
using ShoppingOnline.WebApplication.Extensions;
using ShoppingOnline.WebApplication.SignalR;

namespace ShoppingOnline.WebApplication.Areas.Admin.Controllers.Role
{
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly SignInManager<AppUser> _signInManager;

        public RoleController(IRoleService roleService, IAuthorizationService authorizationService,
            IHubContext<ChatHub> hubContext, SignInManager<AppUser> signInManager,UserManager<AppUser> userManager)
        {
            _roleService = roleService;
            _authorizationService = authorizationService;
            _hubContext = hubContext;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "ROLE", Operations.Read);
            if (result.Succeeded == false)
            {
                await _signInManager.SignOutAsync();
                return new RedirectResult("/Admin/Login/Index");
            }

            return View();
        }

        public async Task<IActionResult> GetAll()
        {
            var model = await _roleService.GetAllAsync();
            return new OkObjectResult(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(Guid id)
        {
            var model = await _roleService.GetById(id);
            return new OkObjectResult(model);
        }

        [HttpGet]
        public IActionResult GetAllPaging(string keyword, int page, int pageSize)
        {
            var model = _roleService.GetAllPagingAsync(keyword, page, pageSize);
            return new OkObjectResult(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveEntity(AppRoleViewModel roleVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            if (!roleVm.Id.HasValue)
            {
                var notificationId = Guid.NewGuid().ToString();
                var model = Mapper.Map<AppUser, AppUserViewModel>(await _userManager.FindByIdAsync(User.GetUserId().ToString()));
                var announcement = new AnnouncementViewModel()
                {
                    Title = "Role created",
                    DateCreated = DateTime.Now,
                    Content = $"Role {roleVm.Name} has been created",
                    Id = notificationId,
                    UserId = User.GetUserId(),
                    Avatar = model.Avatar
                };

                var announcementUsers = new List<AnnouncementUserViewModel>()
                {
                    new AnnouncementUserViewModel()
                    {
                        AnnouncementId = notificationId,
                        HasRead = false,
                        UserId = User.GetUserId()
                    }
                };

                var result = await _roleService.AddAsync(announcement, announcementUsers, roleVm);

                if (result == false)
                {
                    return new OkObjectResult(new GenericResult(false, roleVm));
                }

                await _hubContext.Clients.All.SendAsync("ReceiveMessage", announcement);
            }
            else
            {
                var model = Mapper.Map<AppUser, AppUserViewModel>(await _userManager.FindByIdAsync(User.GetUserId().ToString()));
                var announcement = new AnnouncementViewModel()
                {
                    Title = "Role updated",
                    DateCreated = DateTime.Now,
                    Content = $"Role {roleVm.Name} has been updated",
                    Id = Guid.NewGuid().ToString(),
                    UserId = User.GetUserId(),
                    Avatar = model.Avatar
                };

                var result = await _roleService.UpdateAsync(announcement, roleVm);

                if (result == false)
                {
                    return new OkObjectResult(new GenericResult(false, roleVm));
                }

                await _hubContext.Clients.All.SendAsync("ReceiveMessage", announcement);
            }

            return new OkObjectResult(new GenericResult(true, roleVm));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            await _roleService.DeleteAsync(id);
            return new OkObjectResult(id);
        }

        [HttpPost]
        public IActionResult ListAllFunction(Guid roleId)
        {
            var functions = _roleService.GetListFunctionWithRole(roleId);
            return new OkObjectResult(functions);
        }

        [HttpPost]
        public IActionResult SavePermission(List<PermissionViewModel> listPermmission, Guid roleId)
        {
            _roleService.SavePermission(listPermmission, roleId);
            return new OkResult();
        }
    }
}