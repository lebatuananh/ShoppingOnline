using System;
using System.ComponentModel.DataAnnotations;
using ShoppingOnline.Data.Enum;

namespace ShoppingOnline.Application.Systems.Settings.Dtos
{
    public class SystemConfigViewModel
    {
        public string Id { get; set; }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        public string Value1 { get; set; }
        
        public int? Value2 { get; set; }

        public bool? Value3 { get; set; }

        public DateTime? Value4 { get; set; }

        public decimal? Value5 { get; set; }
        public Status Status { get; set; }
    }
}