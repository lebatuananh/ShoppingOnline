using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ShoppingOnline.Application.Common.Advertisements.Dtos;
using ShoppingOnline.Data.Entities.Advertisement;
using ShoppingOnline.Data.Enum;
using ShoppingOnline.Infrastructure.Interfaces;
using ShoppingOnline.Utilities.Dtos;

namespace ShoppingOnline.Application.Common.Advertisements
{
    public class AdvertisementService : IAdvertisementService
    {
        private readonly IRepository<Advertisement, int> _advertisementRepository;
        private readonly IRepository<AdvertisementPosition, string> _advertisementPositionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AdvertisementService(IRepository<AdvertisementPosition, string> advertisementPositionRepository,
            IRepository<Advertisement, int> advertisementRepository,
            IUnitOfWork unitOfWork)
        {
            this._advertisementRepository = advertisementRepository;
            this._advertisementPositionRepository = advertisementPositionRepository;
            this._unitOfWork = unitOfWork;
        }

        public AdvertisementViewModel Add(AdvertisementViewModel viewModel)
        {
            var advertisement = Mapper.Map<AdvertisementViewModel, Advertisement>(viewModel);
            _advertisementRepository.Add(advertisement);
            return viewModel;
        }

        public void Delete(int id)
        {
            _advertisementRepository.Remove(id);
        }

        public List<AdvertisementPositionViewModel> GetAllAdvertisementPosition()
        {
            return _advertisementPositionRepository.FindAll().OrderBy(x => x.Name).ProjectTo<AdvertisementPositionViewModel>().ToList();
        }

        public PagedResult<AdvertisementViewModel> GetAllPaging(string keyword, int page, int pageSize)
        {
            var query = _advertisementRepository.FindAll(x => x.Status == Status.Active);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.Name.Contains(keyword));

            int totalRow = query.Count();

            query = query.OrderByDescending(n => n.SortOrder).Skip((page - 1) * pageSize)
                .Take(pageSize);

            var data = query.ProjectTo<AdvertisementViewModel>().ToList();

            var paginationSet = new PagedResult<AdvertisementViewModel>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };

            return paginationSet;
        }

        public AdvertisementViewModel GetById(int id)
        {
            var query = _advertisementRepository.FindById(id);
            var model = Mapper.Map<Advertisement, AdvertisementViewModel>(query);
            return model;
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void Update(AdvertisementViewModel viewModel)
        {
            var advertisement = Mapper.Map<AdvertisementViewModel, Advertisement>(viewModel);
            _advertisementRepository.Update(advertisement);
        }


    }
}