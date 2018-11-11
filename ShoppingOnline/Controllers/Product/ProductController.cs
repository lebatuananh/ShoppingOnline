using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using ShoppignOnline.Application.Dapper.Interfaces;
using ShoppingOnline.Application.Common;
using ShoppingOnline.Application.ECommerce.ProductCategories;
using ShoppingOnline.Application.ECommerce.Products;
using ShoppingOnline.WebApplication.Models.ProductViewModels;

namespace ShoppingOnline.WebApplication.Controllers.Product
{
    public class ProductController : Controller
    {
        private readonly IProductCategoryService _productCategoryService;
        private readonly IProductService _productService;
        private readonly IConfiguration _configuration;
        private readonly IColorDapperService _colorDapperService;
        private readonly ISizeDapperService _sizesDapperService;

        public ProductController(IProductCategoryService productCategoryService, IProductService productService, IConfiguration configuration,
             IColorDapperService colorDapperService, ISizeDapperService sizesDapperService)
        {
            _productCategoryService = productCategoryService;
            _productService = productService;
            _configuration = configuration;
            _colorDapperService = colorDapperService;
            _sizesDapperService = sizesDapperService;
        }

        [Route("products.html")]
        public IActionResult Index()
        {
            var categories = _productCategoryService.GetAll();
            return View(categories);
        }


        [Route("{alias}-c.{id}.html")]
        public IActionResult Catalog(int id, string keyword, int? pageSize, string sortBy, int page = 1)
        {
            ViewData["BodyClass"] = "shop_grid_full_width_page";
            var catalogViewModel = new CatalogViewModel();
            catalogViewModel.MetaKeyword = string.Empty;
            catalogViewModel.MetaDescription = string.Empty;

            if (pageSize == null)
                pageSize = _configuration.GetValue<int>("PageSize");

            if (string.IsNullOrEmpty(sortBy))
                sortBy = _configuration.GetValue<string>("SortType");

            catalogViewModel.PageSize = pageSize;
            catalogViewModel.SortType = sortBy;
            catalogViewModel.Data = _productService.GetAllPaging(id, string.Empty, page, pageSize.Value, sortBy);
            catalogViewModel.Category = _productCategoryService.GetById(id);
            catalogViewModel.Title = catalogViewModel.Category.Name;

            return View(catalogViewModel);
        }

        [Route("{alias}-p.{id}.html", Name = "ProductDetail")]
        public IActionResult Details(int id)
        {
            ViewData["BodyClass"] = "product-page";

            var detail = new DetailViewModel();
            detail.Product = _productService.GetById(id);
            detail.Category = _productCategoryService.GetById(detail.Product.CategoryId);
            detail.ProductImages = _productService.GetImages(id);
            detail.RelatedProducts = _productService.GetRelatedProducts(id, 5);

            detail.UpsellProducts = _productService.GetUpSellProducts(5);
            detail.Colors = _colorDapperService.GetColors(id).Select(n => new SelectListItem()
            {
                Value = n.Id.ToString(),
                Text = n.Name
            }).ToList();

            detail.Sizes = _sizesDapperService.GetSizes(id).Select(n => new SelectListItem()
            {
                Value = n.Id.ToString(),
                Text = n.Name
            }).ToList();

            detail.Tags = _productService.GetProductTags(id);

            return View(detail);
        }

        [Route("search.html")]
        public IActionResult Search(int? id, string keyword, int? pageSize, string sortBy, int page = 1)
        {

            ViewData["BodyClass"] = "shop_grid_full_width_page";
            var result = new SearchResultViewModel();

            if (pageSize == null)
                pageSize = _configuration.GetValue<int>("PageSize");

            if (string.IsNullOrEmpty(sortBy))
                sortBy = _configuration.GetValue<string>("SortType");

            result.PageSize = pageSize;
            result.SortType = sortBy;
            result.Data = _productService.GetAllPaging(id, keyword, page, pageSize.Value, sortBy);
            if (string.IsNullOrEmpty(keyword))
                keyword = "No result";
            result.Title = keyword;
            result.MetaKeyword = keyword;
            result.MetaDescription = keyword;
            result.Keyword = keyword;
            return View(result);
        }
    }
}