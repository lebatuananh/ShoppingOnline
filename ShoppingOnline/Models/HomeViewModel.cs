using System.Collections.Generic;
using ShoppingOnline.Application.Common.Advertisements.Dtos;
using ShoppingOnline.Application.Common.Dtos;
using ShoppingOnline.Application.Common.Slides.Dtos;
using ShoppingOnline.Application.Content.Blogs.Dtos;
using ShoppingOnline.Application.ECommerce.ProductCategories.Dtos;
using ShoppingOnline.Application.ECommerce.Products.Dtos;

namespace ShoppingOnline.WebApplication.Models
{
    public class HomeViewModel
    {
        public List<BlogViewModel> LastestBlogs { get; set; }
        public List<SlideViewModel> HomeSlides { get; set; }
        public List<ProductViewModel> HotProducts { get; set; }
        public List<ProductViewModel> TopSellProducts { get; set; }
        public List<ProductCategoryViewModel> HomeCategories { get; set; }
        public AdvertisementViewModel Advertisement { get; set; }
        public string Title { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
    }
}