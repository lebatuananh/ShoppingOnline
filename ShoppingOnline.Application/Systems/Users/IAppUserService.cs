using System.Collections.Generic;
using System.Threading.Tasks;
using ShoppingOnline.Application.Systems.Users.Dtos;
using ShoppingOnline.Utilities.Dtos;

namespace ShoppingOnline.Application.Systems.Users
{
    public interface IAppUserService
    {
        Task<bool> AddAsync(AppUserViewModel userVm);

        Task DeleteAsync(string id);

        Task<List<AppUserViewModel>> GetAllAsync();

        PagedResult<AppUserViewModel> GetAllPagingAsync(string keyword, int page, int pageSize);

        Task<AppUserViewModel> GetByIdAsync(string id);

        Task<bool> UpdateAsync(AppUserViewModel userVm);

        Task<List<AppUserViewModel>> AnnouncementUsers(string functionId);
    }
}