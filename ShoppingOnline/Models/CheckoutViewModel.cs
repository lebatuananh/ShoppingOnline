using System;
using System.Collections.Generic;
using System.Linq;
using ShoppingOnline.Application.Common.Enum;
using ShoppingOnline.Application.ECommerce.Bills.Dtos;
using ShoppingOnline.Data.Enum;
using ShoppingOnline.Utilities.Extensions;

namespace ShoppingOnline.WebApplication.Models
{
    public class CheckoutViewModel : BillViewModel
    {
        public List<ShoppingCartViewModel> Carts { get; set; }

        public List<EnumModel> PaymentMethods
        {
            get
            {
                return ((PaymentMethod[]) Enum.GetValues(typeof(PaymentMethod)))
                    .Select(c => new EnumModel
                    {
                        Value = (int) c,
                        Name = c.GetDescription()
                    }).ToList();
            }
        }
    }
}