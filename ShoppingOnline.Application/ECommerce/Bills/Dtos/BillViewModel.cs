using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ShoppingOnline.Data.Enum;

namespace ShoppingOnline.Application.ECommerce.Bills.Dtos
{
    public class BillViewModel
    {
        public int Id { get; set; }

        public string CustomerName { set; get; }

        public string CustomerAddress { set; get; }

        public string CustomerMobile { set; get; }

        public string CustomerMessage { set; get; }

        public PaymentMethod PaymentMethod { set; get; }

        public BillStatus BillStatus { set; get; }

        public DateTime DateCreated { set; get; }

        public DateTime DateModified { set; get; }

        public Status Status { set; get; }

        public Guid? CustomerId { set; get; }

        public int ShipperId { set; get; }

        public List<BillDetailViewModel> BillDetails { set; get; }
    }
}