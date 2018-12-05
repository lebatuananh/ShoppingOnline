using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ShoppingOnline.Application.Systems.Shippers.Dtos;
using ShoppingOnline.Data.Entities.System;
using ShoppingOnline.Data.Enum;
using ShoppingOnline.Infrastructure.Interfaces;
using ShoppingOnline.Utilities.Dtos;

namespace ShoppingOnline.Application.Systems.Shippers
{
    public class ShipperService : IShipperService
    {
        private readonly IRepository<Shipper, int> _shipperRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ShipperService(IRepository<Shipper, int> shipperRepository, IUnitOfWork unitOfWork)
        {
            _shipperRepository = shipperRepository;
            _unitOfWork = unitOfWork;
        }

        public PagedResult<ShipperViewModel> GetAllPaging(string keyword, int page, int pageSize)
        {
            var query = _shipperRepository.FindAll(x => x.Status == Status.Active);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.Name.Contains(keyword));

            int totalRow = query.Count();

            query = query.OrderByDescending(n => n.Name).Skip((page - 1) * pageSize).Take(pageSize);

            var data = query.ProjectTo<ShipperViewModel>().ToList();

            var paginationSet = new PagedResult<ShipperViewModel>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };

            return paginationSet;
        }

        public List<ShipperViewModel> GetAll()
        {
            return _shipperRepository.FindAll().OrderBy(n => n.Name).ProjectTo<ShipperViewModel>().ToList();
        }

        public ShipperViewModel Add(ShipperViewModel viewModel)
        {
            var model = Mapper.Map<ShipperViewModel, Shipper>(viewModel);
            _shipperRepository.Add(model);
            return viewModel;
        }

        public void Update(ShipperViewModel viewModel)
        {
            var model = Mapper.Map<ShipperViewModel, Shipper>(viewModel);
            _shipperRepository.Update(model);
        }

        public void Delete(int id)
        {
            _shipperRepository.Remove(id);
        }

        public ShipperViewModel GetById(int id)
        {
            return Mapper.Map<Shipper, ShipperViewModel>(_shipperRepository.FindById(id));
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }
    }
}