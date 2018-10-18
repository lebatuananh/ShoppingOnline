using System;
using ShoppingOnline.Application.Systems.Announcements.Dtos;
using ShoppingOnline.Utilities.Dtos;

namespace ShoppingOnline.Application.Systems.Announcements
{
    public interface IAnnouncementService
    {
        PagedResult<AnnouncementViewModel> GetAllUnReadPaging(Guid userId, int pageIndex, int pageSize);

        void MarkAsRead(Guid userId, string id);

        void ReadAll(Guid userId);

        void Delete(Guid userId, string id);

        void SaveChanges();
    }
}