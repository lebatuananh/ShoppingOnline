using Microsoft.AspNetCore.Mvc.Rendering;
using ShoppingOnline.Application.ECommerce.ProductCategories.Dtos;
using ShoppingOnline.Application.ECommerce.Products.Dtos;
using ShoppingOnline.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingOnline.WebApplication.Models.ProductViewModels
{
    public class CatalogViewModel
    {
        public PagedResult<ProductViewModel> Data { get; set; }

        public string SortType { get; set; }

        public int? PageSize { get; set; }

        public ProductCategoryViewModel Category { set; get; }

        public List<SelectListItem> SortTypes { get; } = new List<SelectListItem>
        {
            new SelectListItem(){Value="lastest",Text="Lastest"},
            new SelectListItem(){Value="price",Text="Price"},
            new SelectListItem(){Value="name",Text="Name"}
        };

        public List<SelectListItem> PageSizes { get; } = new List<SelectListItem>
        {
            new SelectListItem(){Value="12",Text="12"},
            new SelectListItem(){Value="24",Text="24"},
            new SelectListItem(){Value="48",Text="48"}
        };

        public string Title { get; set; }

        public string MetaKeyword { get; set; }

        public string MetaDescription { get; set; }
    }
}
