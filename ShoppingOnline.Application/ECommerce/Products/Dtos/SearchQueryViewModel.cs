using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingOnline.Application.ECommerce.Products.Dtos
{
    public class SearchQueryViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Alias { get; set; }

        public string Category { get; set; }

        public string CategoryAlias { get; set; }
    }
}
