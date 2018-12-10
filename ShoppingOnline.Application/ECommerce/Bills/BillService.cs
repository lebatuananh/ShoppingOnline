using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ShoppingOnline.Application.ECommerce.Bills.Dtos;
using ShoppingOnline.Application.ECommerce.Products.Dtos;
using ShoppingOnline.Application.Systems.Announcements.Dtos;
using ShoppingOnline.Data.Entities.ECommerce;
using ShoppingOnline.Data.Entities.System;
using ShoppingOnline.Data.Enum;
using ShoppingOnline.Infrastructure.Interfaces;
using ShoppingOnline.Utilities.Dtos;

namespace ShoppingOnline.Application.ECommerce.Bills
{
    public class BillService : IBillService
    {
        private readonly IRepository<Bill, int> _billRepository;
        private readonly IRepository<BillDetail, int> _billDetailRepository;
        private readonly IRepository<Color, int> _colorRepository;
        private readonly IRepository<Size, int> _sizeRepository;
        private readonly IRepository<Product, int> _productRepository;
        private readonly IRepository<Announcement, string> _announcementRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BillService(IRepository<Bill, int> billRepository, IRepository<BillDetail, int> billDetailRepository,
            IRepository<Color, int> colorRepository, IRepository<Size, int> sizeRepository,
            IRepository<Product, int> productRepository, IRepository<Announcement, string> announcementRepository,
            IUnitOfWork unitOfWork)
        {
            _billRepository = billRepository;
            _billDetailRepository = billDetailRepository;
            _colorRepository = colorRepository;
            _sizeRepository = sizeRepository;
            _productRepository = productRepository;
            _announcementRepository = announcementRepository;
            _unitOfWork = unitOfWork;
        }

        public void Create(BillViewModel billVm, AnnouncementViewModel announcementVm)
        {
            var order = Mapper.Map<BillViewModel, Bill>(billVm);
            var orderDetail = Mapper.Map<List<BillDetailViewModel>, List<BillDetail>>(billVm.BillDetails);

            order.BillDetails = orderDetail;

            var announcement = Mapper.Map<AnnouncementViewModel, Announcement>(announcementVm);

            _billRepository.Add(order);
             _announcementRepository.Add(announcement);
        }

        public void Update(BillViewModel billVm)
        {
            //Mapping to order domain
            var order = Mapper.Map<BillViewModel, Bill>(billVm);

            //Get order Detail
            var newDetails = order.BillDetails;

            //new details added
            var addedDetails = newDetails.Where(x => x.Id == 0).ToList();

            //get updated details
            var updatedDetails = newDetails.Where(x => x.Id != 0).ToList();

            //Existed details
            var existedDetails = _billDetailRepository.FindAll(x => x.BillId == billVm.Id);

            //Clear db
            order.BillDetails.Clear();

            foreach (var detail in updatedDetails)
            {
                var product = _productRepository.FindById(detail.ProductId);
                detail.Price = product.Price;
                _billDetailRepository.Update(detail);
            }

            foreach (var detail in addedDetails)
            {
                var product = _productRepository.FindById(detail.ProductId);
                detail.Price = product.Price;
                _billDetailRepository.Add(detail);
            }

            //_billDetailRepository.RemoveMultiple(existedDetails.Except(updatedDetails).ToList());

            _billRepository.Update(order);
        }

        public List<BillViewModel> GetAll(Guid id)
        {
            return _billRepository.FindAll(x=>x.CustomerId==id).ProjectTo<BillViewModel>().ToList();
        }

        public PagedResult<BillViewModel> GetAllPaging(string startDate, string endDate, string keyword, int pageIndex,
            int pageSize)
        {
            var query = _billRepository.FindAll();

            if (!string.IsNullOrEmpty(startDate))
            {
                DateTime fromDate = DateTime.ParseExact(startDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"));
                query = query.Where(n =>
                    n.DateCreated.Year >= fromDate.Year && n.DateCreated.Month >= fromDate.Month &&
                    n.DateCreated.Day >= fromDate.Day);
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                DateTime toDate = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"));
                query = query.Where(n =>
                    n.DateCreated.Year <= toDate.Year && n.DateCreated.Month <= toDate.Month &&
                    n.DateCreated.Day <= toDate.Day);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(n => n.CustomerName.Contains(keyword) || n.CustomerMobile.Contains(keyword));
            }

            var totalRow = query.Count();

            var data = query.OrderByDescending(n => n.DateCreated).Skip((pageIndex - 1) * pageIndex).Take(pageSize)
                .ProjectTo<BillViewModel>().ToList();

            return new PagedResult<BillViewModel>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                Results = data,
                RowCount = totalRow
            };
        }

        public BillViewModel GetDetail(int billId)
        {
            var bill = _billRepository.FindSingle(n => n.Id == billId);
            var billViewModel = Mapper.Map<Bill, BillViewModel>(bill);

            var billDetailViewModel = _billDetailRepository.FindAll(n => n.BillId == billId)
                .ProjectTo<BillDetailViewModel>().ToList();
            billViewModel.BillDetails = billDetailViewModel;

            return billViewModel;
        }

        public BillDetailViewModel CreateDetail(BillDetailViewModel billDetailVm)
        {
            var billDetail = Mapper.Map<BillDetailViewModel, BillDetail>(billDetailVm);
            _billDetailRepository.Add(billDetail);
            return billDetailVm;
        }

        public void DeleteDetail(int productId, int billId, int colorId, int sizeId)
        {
            var model = _billDetailRepository.FindSingle(n =>
                n.ProductId == productId && billId == n.BillId && n.ColorId == colorId && sizeId == n.SizeId);
            _billDetailRepository.Remove(model);
        }

        public void UpdateStatus(int orderId, BillStatus status)
        {
            var order = _billRepository.FindById(orderId);
            order.BillStatus = status;
            _billRepository.Update(order);
        }

        public List<BillDetailViewModel> GetBillDetails(int billId)
        {
            return _billDetailRepository
                .FindAll(x => x.BillId == billId, c => c.Bill, c => c.Color, c => c.Size, c => c.Product)
                .ProjectTo<BillDetailViewModel>().ToList();
        }

        public List<ColorViewModel> GetColors()
        {
            return _colorRepository.FindAll().ProjectTo<ColorViewModel>().ToList();
        }

        public List<SizeViewModel> GetSizes()
        {
            return _sizeRepository.FindAll().ProjectTo<SizeViewModel>().ToList();
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }
    }
}