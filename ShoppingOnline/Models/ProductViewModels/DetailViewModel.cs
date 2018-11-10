using Microsoft.AspNetCore.Mvc.Rendering;
using ShoppingOnline.Application.Content.Dtos;
using ShoppingOnline.Application.ECommerce.ProductCategories.Dtos;
using ShoppingOnline.Application.ECommerce.Products.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingOnline.WebApplication.Models.ProductViewModels
{
    public class DetailViewModel
    {
        public ProductViewModel Product { get; set; }

        public List<ProductViewModel> RelatedProducts { get; set; }

        public ProductCategoryViewModel Category { get; set; }

        public List<ProductImageViewModel> ProductImages { get; set; }

        public List<ProductViewModel> UpsellProducts { get; set; }

        //public List<ProductViewModel> LastesProducts { get; set; }

        public List<TagViewModel> Tags { get; set; }

        public List<SelectListItem> Colors { get; set; }

        public List<SelectListItem> Sizes { get; set; }
    }
}
