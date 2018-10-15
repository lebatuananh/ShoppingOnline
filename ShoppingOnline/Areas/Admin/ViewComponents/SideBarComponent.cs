using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Application.Systems.Functions;
using ShoppingOnline.Application.Systems.Functions.Dtos;
using ShoppingOnline.Application.Systems.Permissions.Dtos;
using ShoppingOnline.Application.Systems.Roles;
using ShoppingOnline.Application.Systems.Roles.Dtos;
using ShoppingOnline.Utilities.Constants;
using ShoppingOnline.WebApplication.Extensions;

namespace ShoppingOnline.WebApplication.Areas.Admin.ViewComponents
{
    [ViewComponent(Name = "SideBarVC")]
    public class SideBarComponent : ViewComponent
    {
        private readonly IFunctionService _functionService;
        private readonly IRoleService _roleService;

        public SideBarComponent(IFunctionService functionService, IRoleService roleService)
        {
            _functionService = functionService;
            _roleService = roleService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var roles = ((ClaimsPrincipal) User).GetSpecificClaim("Roles");
            List<FunctionViewModel> functions;
            if (roles.Split(";").Contains(CommonConstants.AdminRole))
            {
                functions = await _functionService.GetAll(string.Empty);
            }
            else
            {
                var appRoles = new List<AppRoleViewModel>();
                var permissions = new List<PermissionViewModel>();
                functions = new List<FunctionViewModel>();
                var splitRoles = roles.Split(';');
                foreach (var item in splitRoles)
                {
                    var functionByRole = await _roleService.GetByName(item);
                    appRoles.Add(functionByRole);
                }

                foreach (var item in appRoles)
                {
                    var query = _roleService.GetListFunctionMenuWithRole(item.Id.Value);
                    permissions.AddRange(query);
                }

                foreach (var item in permissions)
                {
                    var query = _functionService.GetById(item.FunctionId);
                    functions.Add(query);
                }
                
            }
            return await Task.Run(() => View(functions));
        }
    }
}