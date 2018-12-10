using System;
using System.Collections.Generic;
using ShoppingOnline.Application.ECommerce.Bills.Dtos;
using ShoppingOnline.Application.ECommerce.Products.Dtos;
using ShoppingOnline.Application.Systems.Announcements.Dtos;
using ShoppingOnline.Data.Enum;
using ShoppingOnline.Utilities.Dtos;

namespace ShoppingOnline.Application.ECommerce.Bills
{
    public interface IBillService
    {
        void Create(BillViewModel billVm, AnnouncementViewModel announcementVm);

        void Update(BillViewModel billVm);

        List<BillViewModel> GetAll(Guid id);

        PagedResult<BillViewModel> GetAllPaging(string startDate, string endDate, string keyword,
            int pageIndex, int pageSize);

        BillViewModel GetDetail(int billId);

        BillDetailViewModel CreateDetail(BillDetailViewModel billDetailVm);

        void DeleteDetail(int productId, int billId, int colorId, int sizeId);

        void UpdateStatus(int billId, BillStatus status);

        List<BillDetailViewModel> GetBillDetails(int billId);

        List<ColorViewModel> GetColors();

        List<SizeViewModel> GetSizes();

        void Save();
    }
}