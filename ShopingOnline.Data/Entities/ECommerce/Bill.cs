using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShoppingOnline.Data.Entities.System;
using ShoppingOnline.Data.Enum;
using ShoppingOnline.Data.Interfaces;
using ShoppingOnline.Infrastructure.SharedKernel;

namespace ShoppingOnline.Data.Entities.ECommerce
{
    [Table("Bills")]
    public class Bill : DomainEntity<int>, ISwitchable, IDateTracking
    {
        public Bill()
        {
        }

        public Bill(string customerName, string customerAddress, string customerMobile, string customerMessage,
            BillStatus billStatus, PaymentMethod paymentMethod, Status status, Guid? customerId)
        {
            CustomerName = customerName;
            CustomerAddress = customerAddress;
            CustomerMobile = customerMobile;
            CustomerMessage = customerMessage;
            BillStatus = billStatus;
            PaymentMethod = paymentMethod;
            Status = status;
            CustomerId = customerId;
        }

        public Bill(int id, string customerName, string customerAddress, string customerMobile, string customerMessage,
            BillStatus billStatus, PaymentMethod paymentMethod, Status status, Guid? customerId, int? shipperId)
        {
            Id = id;
            CustomerName = customerName;
            CustomerAddress = customerAddress;
            CustomerMobile = customerMobile;
            CustomerMessage = customerMessage;
            BillStatus = billStatus;
            PaymentMethod = paymentMethod;
            Status = status;
            CustomerId = customerId;
            ShipperId = shipperId;
        }

        [Required] [MaxLength(256)] public string CustomerName { set; get; }

        [Required] [MaxLength(256)] public string CustomerAddress { set; get; }

        [Required] [MaxLength(50)] public string CustomerMobile { set; get; }

        [Required] [MaxLength(256)] public string CustomerMessage { set; get; }

        public PaymentMethod PaymentMethod { set; get; }

        public BillStatus BillStatus { set; get; }

        public DateTime DateCreated { set; get; }
        public DateTime DateModified { set; get; }

        [DefaultValue(Status.Active)] public Status Status { set; get; } = Status.Active;

        public int? ShipperId { get; set; }

        public Guid? CustomerId { set; get; }

        [ForeignKey("CustomerId")] public virtual AppUser User { set; get; }

        [ForeignKey("ShipperId")] public virtual Shipper Shipper { get; set; }

        public virtual ICollection<BillDetail> BillDetails { set; get; }
    }
}