using System.Collections.Generic;
using ShoppingOnline.Application.ECommerce.Bills.Dtos;

namespace ShoppingOnline.WebApplication.Models
{
    public class PayViewModel
    {
        public BillViewModel Bill { get; set; }
        public List<BillDetailViewModel> Details { get; set; }
    }
}