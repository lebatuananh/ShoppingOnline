using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShoppingOnline.Application.Systems.Announcements.Dtos;
using ShoppingOnline.Application.Systems.Permissions.Dtos;
using ShoppingOnline.Application.Systems.Roles.Dtos;
using ShoppingOnline.Utilities.Dtos;

namespace ShoppingOnline.Application.Systems.Roles
{
    public interface IRoleService
    {
        Task<bool> AddAsync(AnnouncementViewModel announcementViewModel,
            List<AnnouncementUserViewModel> announcementUsers, AppRoleViewModel userVm);

        Task DeleteAsync(Guid id);

        Task<List<AppRoleViewModel>> GetAllAsync();

        PagedResult<AppRoleViewModel> GetAllPagingAsync(string keyword, int page, int pageSize);

        Task<AppRoleViewModel> GetById(Guid id);

        Task<bool> UpdateAsync(AnnouncementViewModel announcementViewModel, AppRoleViewModel userVm);

        List<PermissionViewModel> GetListFunctionWithRole(Guid roleId);

        void SavePermission(List<PermissionViewModel> permissions, Guid roleId);

        Task<bool> CheckPermission(string functionId, string action, string[] roles);

        Task<AppRoleViewModel> GetByName(string role);

        List<PermissionViewModel> GetListFunctionMenuWithRole(Guid roleId);
    }
}