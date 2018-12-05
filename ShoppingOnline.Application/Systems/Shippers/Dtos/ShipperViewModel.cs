using System;
using ShoppingOnline.Data.Enum;

namespace ShoppingOnline.Application.Systems.Shippers.Dtos
{
    public class ShipperViewModel
    {
        public int Id { set; get; }

        public string Name { get; set; }

        public string CompanyName { get; set; }

        public string Phone { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public Status Status { get; set; }
    }
}