using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShoppingOnline.Application.Systems.Announcements.Dtos;
using ShoppingOnline.Application.Systems.Permissions.Dtos;
using ShoppingOnline.Application.Systems.Roles.Dtos;
using ShoppingOnline.Data.Entities.System;
using ShoppingOnline.Infrastructure.Interfaces;
using ShoppingOnline.Utilities.Dtos;

namespace ShoppingOnline.Application.Systems.Roles
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IRepository<Function, string> _functionRepository;
        private readonly IRepository<Permission, int> _permissionRepository;
        private readonly IRepository<AnnouncementUser, int> _announcementUserRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Announcement, string> _announcementRepository;

        public RoleService(RoleManager<AppRole> roleManager, IRepository<Function, string> functionRepository,
            IRepository<Permission, int> permissionRepository,
            IRepository<AnnouncementUser, int> announcementUserRepository, IUnitOfWork unitOfWork,
            IRepository<Announcement, string> announcementRepository)
        {
            _roleManager = roleManager;
            _functionRepository = functionRepository;
            _permissionRepository = permissionRepository;
            _announcementUserRepository = announcementUserRepository;
            _unitOfWork = unitOfWork;
            _announcementRepository = announcementRepository;
        }

        public async Task<bool> AddAsync(AnnouncementViewModel announcementViewModel,
            List<AnnouncementUserViewModel> announcementUsers, AppRoleViewModel appRoleVm)
        {
            var findRole = await _roleManager.FindByNameAsync(appRoleVm.Name);

            if (findRole != null)
                return false;

            var role = new AppRole()
            {
                Name = appRoleVm.Name,
                Description = appRoleVm.Description
            };


            var result = await _roleManager.CreateAsync(role);
            var announcement = Mapper.Map<AnnouncementViewModel, Announcement>(announcementViewModel);

            foreach (var item in announcementUsers)
            {
                var user = Mapper.Map<AnnouncementUserViewModel, AnnouncementUser>(item);
                _announcementUserRepository.Add(user);
            }

            _announcementRepository.Add(announcement);
            _unitOfWork.Commit();
            return result.Succeeded;
        }

        public async Task DeleteAsync(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            await _roleManager.DeleteAsync(role);
        }

        public async Task<List<AppRoleViewModel>> GetAllAsync()
        {
            return await _roleManager.Roles.ProjectTo<AppRoleViewModel>().ToListAsync();
        }

        public PagedResult<AppRoleViewModel> GetAllPagingAsync(string keyword, int page, int pageSize)
        {
            var query = _roleManager.Roles;
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.Name.Contains(keyword)
                                         || x.Description.Contains(keyword));

            int totalRow = query.Count();
            query = query.Skip((page - 1) * pageSize)
                .Take(pageSize);

            var data = query.ProjectTo<AppRoleViewModel>().ToList();
            var paginationSet = new PagedResult<AppRoleViewModel>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };

            return paginationSet;
        }

        public async Task<AppRoleViewModel> GetById(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            return Mapper.Map<AppRole, AppRoleViewModel>(role);
        }

        public async Task<bool> UpdateAsync(AnnouncementViewModel announcementViewModel, AppRoleViewModel appRoleVm)
        {
            var announcement = Mapper.Map<AnnouncementViewModel, Announcement>(announcementViewModel);
            var role = await _roleManager.FindByIdAsync(appRoleVm.Id.ToString());

            role.Description = appRoleVm.Description;
            role.Name = appRoleVm.Name;
            await _roleManager.UpdateAsync(role);
            _announcementRepository.Add(announcement);

            _unitOfWork.Commit();
            return true;
        }

        public List<PermissionViewModel> GetListFunctionWithRole(Guid roleId)
        {
            var functions = _functionRepository.FindAll();
            var permissions = _permissionRepository.FindAll();

            var query = from f in functions
                join p in permissions on f.Id equals p.FunctionId into fp
                from p in fp.DefaultIfEmpty()
                where p != null && p.RoleId == roleId
                select new PermissionViewModel()
                {
                    RoleId = roleId,
                    FunctionId = f.Id,
                    CanCreate = p != null ? p.CanCreate : false,
                    CanDelete = p != null ? p.CanDelete : false,
                    CanRead = p != null ? p.CanRead : false,
                    CanUpdate = p != null ? p.CanUpdate : false
                };
            return query.ToList();
        }

        public void SavePermission(List<PermissionViewModel> permissionVms, Guid roleId)
        {
            var permissions = Mapper.Map<List<PermissionViewModel>, List<Permission>>(permissionVms);
            var oldPermission = _permissionRepository.FindAll().Where(x => x.RoleId == roleId).ToList();
            if (oldPermission.Count > 0)
            {
                _permissionRepository.RemoveMultiple(oldPermission);
            }

            foreach (var permission in permissions)
            {
                _permissionRepository.Add(permission);
            }

            _unitOfWork.Commit();
        }

        public Task<bool> CheckPermission(string functionId, string action, string[] roles)
        {
            var functions = _functionRepository.FindAll();
            var permissions = _permissionRepository.FindAll();
            var query = from f in functions
                join p in permissions on f.Id equals p.FunctionId
                join r in _roleManager.Roles on p.RoleId equals r.Id
                where roles.Contains(r.Name) && f.Id == functionId &&
                      (p.CanCreate && action == "Create"
                       || p.CanUpdate && action == "Update"
                       || p.CanDelete && action == "Delete"
                       || p.CanRead && action == "Read")
                select p;
            return query.AnyAsync();
        }

        public async Task<AppRoleViewModel> GetByName(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            return Mapper.Map<AppRole, AppRoleViewModel>(role);
        }

        public List<PermissionViewModel> GetListFunctionMenuWithRole(Guid roleId)
        {
            var functions = _functionRepository.FindAll();
            var permissions = _permissionRepository.FindAll();
            var query = from f in functions
                join p in permissions on f.Id equals p.FunctionId into fp
                from p in fp.DefaultIfEmpty()
                where p != null && p.RoleId == roleId &&
                      (
                          p.CanCreate == true
                          || p.CanDelete == true
                          || p.CanRead == true
                          || p.CanUpdate == true
                      )
                select new PermissionViewModel()
                {
                    RoleId = roleId,
                    FunctionId = f.Id,
                    CanCreate = p != null ? p.CanCreate : false,
                    CanDelete = p != null ? p.CanDelete : false,
                    CanRead = p != null ? p.CanRead : false,
                    CanUpdate = p != null ? p.CanUpdate : false
                };
            return query.ToList();
        }
    }
}