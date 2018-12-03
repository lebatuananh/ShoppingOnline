using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShoppingOnline.Application.Systems.Users.Dtos;
using ShoppingOnline.Data.Entities.System;
using ShoppingOnline.Infrastructure.Interfaces;
using ShoppingOnline.Utilities.Dtos;

namespace ShoppingOnline.Application.Systems.Users
{
    public class AppUserService : IAppUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IRepository<Function, string> _functionRepository;
        private readonly IRepository<Permission, int> _permissionRepository;

        public AppUserService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager,
            IRepository<Function, string> functionRepository, IRepository<Permission, int> permissionRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _functionRepository = functionRepository;
            _permissionRepository = permissionRepository;
        }

        public async Task<bool> AddAsync(AppUserViewModel userVm)
        {
            var findByEmail = await _userManager.FindByEmailAsync(userVm.Email);
            var findByUserName = await _userManager.FindByNameAsync(userVm.UserName);
            if (findByEmail != null || findByUserName != null)
            {
                return false;
            }

            var user = new AppUser()
            {
                UserName = userVm.UserName,
                Avatar = userVm.Avatar,
                FullName = userVm.FullName,
                DateCreated = DateTime.Now,
                Status = userVm.Status,
                Email = userVm.Email,
                PhoneNumber = userVm.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, userVm.Password);
            if (result.Succeeded && userVm.Roles.Count > 0)
            {
                var appUser = await _userManager.FindByNameAsync(user.UserName);
                if (appUser != null)
                {
                    await _userManager.AddToRolesAsync(appUser, userVm.Roles);
                }
            }

            return true;
        }

        public async Task DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(user);
        }

        public async Task<List<AppUserViewModel>> GetAllAsync()
        {
            return await _userManager.Users.ProjectTo<AppUserViewModel>().ToListAsync();
        }

        public PagedResult<AppUserViewModel> GetAllPagingAsync(string keyword, int page, int pageSize)
        {
            var query = _userManager.Users;
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x =>
                    x.UserName.Contains(keyword) || x.Email.Contains(keyword) || x.FullName.Contains(keyword));

            var totalRow = query.Count();
            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var data = query.Select(n => new AppUserViewModel()
            {
                UserName = n.UserName,
                Avatar = n.Avatar,
                FullName = n.FullName,
                BirthDay = n.BirthDay.ToString(),
                Email = n.Email,
                Id = n.Id,
                PhoneNumber = n.PhoneNumber,
                Status = n.Status,
                DateCreated = n.DateCreated
            }).ToList();

            var paginationSet = new PagedResult<AppUserViewModel>
            {
                RowCount = totalRow,
                Results = data,
                PageSize = pageSize,
                CurrentPage = page
            };
            return paginationSet;
        }

        public async Task<AppUserViewModel> GetByIdAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                var userRole = await _userManager.GetRolesAsync(user);
                var model = Mapper.Map<AppUser, AppUserViewModel>(user);
                model.Roles = userRole.ToList();
                return model;
            }
            catch (Exception e)
            {
                return null;
                throw;
            }
        }

        public async Task<bool> UpdateAsync(AppUserViewModel userVm)
        {
            var user = await _userManager.FindByIdAsync(userVm.Id.ToString());
            var findByEmail = await _userManager.FindByEmailAsync(userVm.Email);

            if (!user.Email.Equals(userVm.Email) && findByEmail != null)
                return false;

            //remove current roles in db

            var currentRoles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user, userVm.Roles.Except(currentRoles).ToArray());

            if (result.Succeeded)
            {
                string[] needRemoveRoles = currentRoles.Except(userVm.Roles).ToArray();
                await _userManager.RemoveFromRolesAsync(user, needRemoveRoles);

                //Update user detail

                user.FullName = userVm.FullName;
                user.Email = userVm.Email;
                user.Status = userVm.Status;
                user.PhoneNumber = userVm.PhoneNumber;
                user.DateModified = DateTime.Now;
                await _userManager.UpdateAsync(user);
                return true;
            }

            return false;
        }

        public async Task<List<AppUserViewModel>> AnnouncementUsers(string functionId)
        {
            var functions = _functionRepository.FindAll();
            var permissions = _permissionRepository.FindAll();

            var query = from f in functions
                        join p in permissions on f.Id equals p.FunctionId
                        join r in _roleManager.Roles on p.RoleId equals r.Id
                        where f.Id == functionId && p.CanRead == true
                        select r;

            var announUsers = new List<AppUserViewModel>();

            foreach (var item in query)
            {
                var users = await _userManager.GetUsersInRoleAsync(item.Name);

                foreach (var jtem in users)
                {
                    var user = Mapper.Map<AppUser, AppUserViewModel>(jtem);
                    announUsers.Add(user);
                }
            }

            return announUsers;
        }
    }
}