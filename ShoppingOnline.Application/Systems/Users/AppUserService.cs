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
using ShoppingOnline.Data.Enum;
using ShoppingOnline.Infrastructure.Interfaces;
using ShoppingOnline.Utilities.Dtos;

namespace ShoppingOnline.Application.Systems.Users
{
    public class AppUserService : IAppUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IRepository<Function, string> _functionRepository;
        private readonly IRepository<Permission, int> _permissionRepository;

        public AppUserService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager,
            SignInManager<AppUser> signInManager, IRepository<Function, string> functionRepository,
            IRepository<Permission, int> permissionRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _functionRepository = functionRepository;
            _permissionRepository = permissionRepository;
        }

        public async Task<bool> AddAsync(AppUserViewModel viewModel)
        {
            var findByEmail = await _userManager.FindByEmailAsync(viewModel.Email);
            var findByUsername = await _userManager.FindByNameAsync(viewModel.UserName);
            var findByPhoneNumber =
                _userManager.Users.SingleOrDefault(n => n.PhoneNumber.Equals(viewModel.PhoneNumber));


            if (findByEmail != null || findByUsername != null || findByPhoneNumber != null)
            {
                return false;
            }


            var user = new AppUser()
            {
                Gender = viewModel.Gender,
                BirthDay = viewModel.BirthDay,
                Address = viewModel.Address,
                UserName = viewModel.UserName,
                Avatar = viewModel.Avatar,
                FullName = viewModel.FullName,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
                Status = viewModel.Status,
                Email = viewModel.Email,
                PhoneNumber = viewModel.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(user, viewModel.Password);

            if (result.Succeeded && viewModel.Roles.Count > 0)
            {
                var appUser = await _userManager.FindByNameAsync(user.UserName);

                if (appUser != null)
                    await _userManager.AddToRolesAsync(appUser, viewModel.Roles);
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
                BirthDay = n.BirthDay,
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
            var findByPhoneNumber = _userManager.Users.SingleOrDefault(n => n.PhoneNumber.Equals(userVm.PhoneNumber));


            if ((!user.Email.Equals(userVm.Email) && findByEmail != null))
            {
                return false;
            }


            if (string.IsNullOrEmpty(user.PhoneNumber) && findByPhoneNumber != null)
            {
                return false;
            }
            else if (findByPhoneNumber != null && !user.PhoneNumber.Equals(userVm.PhoneNumber))
            {
                return false;
            }

            //remove current roles in db
            var currentRoles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user, userVm.Roles.Except(currentRoles).ToArray());

            if (result.Succeeded)
            {
                string[] needRemoveRoles = currentRoles.Except(userVm.Roles).ToArray();
                await _userManager.RemoveFromRolesAsync(user, needRemoveRoles);

                //Update user detail
                user.Gender = userVm.Gender;
                user.Address = userVm.Address;
                user.BirthDay = userVm.BirthDay;
                user.FullName = userVm.FullName;
                user.Email = userVm.Email;
                user.Status = userVm.Status;
                user.PhoneNumber = userVm.PhoneNumber;
                user.DateModified = DateTime.Now;
                await _userManager.UpdateAsync(user);

                if (user.Status == Status.InActive)
                {
                    await _userManager.UpdateSecurityStampAsync(user);
                }

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

        public async Task<bool> UpdateAccount(AppUserViewModel userVm)
        {
            var user = await _userManager.FindByIdAsync(userVm.Id.ToString());
            var findByEmail = await _userManager.FindByEmailAsync(userVm.Email);

            if (!user.Email.Equals(userVm.Email) && findByEmail != null)
                return false;

            //Update user detail
            user.Gender = userVm.Gender;
            user.Address = userVm.Address;
            user.BirthDay = userVm.BirthDay;
            user.FullName = userVm.FullName;
            user.Email = userVm.Email;
            user.Status = userVm.Status;
            user.PhoneNumber = userVm.PhoneNumber;
            user.DateModified = DateTime.Now;
            await _userManager.UpdateAsync(user);
            return true;
        }

        public async Task<bool> ChangePassword(string userId, string oldPassword, string password)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var checkPassword = await _userManager.CheckPasswordAsync(user, oldPassword);

            if (checkPassword == false)
            {
                return false;
            }
            else
            {
                await _userManager.ChangePasswordAsync(user, oldPassword, password);

                await _userManager.UpdateSecurityStampAsync(user);

                return true;
            }
        }

        public async Task<bool> ResetPassword(string userId, string password)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null || string.IsNullOrEmpty(password))
            {
                return false;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, password);

            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckPhoneNumber(string phoneNumber)
        {
            var user = _userManager.Users.SingleOrDefault(n => n.PhoneNumber.Equals(phoneNumber));

            if (user != null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> CheckUpdatePhoneNumber(string phoneNumber, string userId)
        {
            var findByPhone = _userManager.Users.SingleOrDefault(n => n.PhoneNumber.Equals(phoneNumber));

            var user = await _userManager.FindByIdAsync(userId);

            if (string.IsNullOrEmpty(phoneNumber) && user != null)
            {
                return false;
            }

            if (user.PhoneNumber.Equals(phoneNumber) && findByPhone != null)
            {
                return true;
            }

            return false;
        }
    }
}