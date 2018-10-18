using System;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using ShoppingOnline.Application.Systems.Announcements.Dtos;
using ShoppingOnline.Data.Entities.System;
using ShoppingOnline.Infrastructure.Interfaces;
using ShoppingOnline.Utilities.Dtos;

namespace ShoppingOnline.Application.Systems.Announcements
{
    public class AnnouncementService : IAnnouncementService
    {
        private IRepository<Announcement, string> _announcementRepository;
        private IRepository<AnnouncementUser, int> _announcementUserRepository;


        private IUnitOfWork _unitOfWork;

        public AnnouncementService(IRepository<Announcement, string> announcementRepository,
            IRepository<AnnouncementUser, int> announcementUserRepository,
            IUnitOfWork unitOfWork)
        {
            _announcementRepository = announcementRepository;
            _announcementUserRepository = announcementUserRepository;
            _unitOfWork = unitOfWork;
        }

        public PagedResult<AnnouncementViewModel> GetAllUnReadPaging(Guid userId, int pageIndex, int pageSize)
        {
            var query = from x in _announcementRepository.FindAll()
                join y in _announcementUserRepository.FindAll()
                    on x.Id equals y.AnnouncementId
                    into xy
                from annonUser in xy.DefaultIfEmpty()
                where annonUser.HasRead == false && (annonUser.UserId == null || annonUser.UserId == userId)
                select x;
            int totalRow = query.Count();

            var model = query.OrderByDescending(x => x.DateCreated)
                .Skip(pageSize * (pageIndex - 1)).Take(pageSize).ProjectTo<AnnouncementViewModel>().ToList();

            var paginationSet = new PagedResult<AnnouncementViewModel>
            {
                Results = model,
                CurrentPage = pageIndex,
                RowCount = totalRow,
                PageSize = pageSize
            };

            return paginationSet;
        }

        public void MarkAsRead(Guid userId, string id)
        {
            var announ = _announcementUserRepository.FindSingle(x => x.AnnouncementId == id && x.UserId == userId);
            announ.HasRead = true;
            _announcementUserRepository.Update(announ);
        }

        public void ReadAll(Guid userId)
        {
            var announUsers = _announcementUserRepository.FindAll(x => x.HasRead == false && x.UserId == userId);

            foreach (var item in announUsers)
            {
                item.HasRead = true;
                _announcementUserRepository.Update(item);
            }
        }

        public void Delete(Guid userId, string id)
        {
            var announ = _announcementUserRepository.FindSingle(x => x.AnnouncementId == id && x.UserId == userId);
            _announcementUserRepository.Remove(announ.Id);
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }
    }
}