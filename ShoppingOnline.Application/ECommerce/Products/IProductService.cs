using System;
using System.Collections.Generic;
using ShoppingOnline.Application.Content.Dtos;
using ShoppingOnline.Application.ECommerce.Products.Dtos;
using ShoppingOnline.Utilities.Dtos;

namespace ShoppingOnline.Application.ECommerce.Products
{
    public interface IProductService : IDisposable
    {
        List<ProductViewModel> GetAll();

        PagedResult<ProductViewModel> GetAllPaging(int? categoryId, string keyword, int page, int pageSize);

        ProductViewModel Add(ProductViewModel viewModel);

        void Update(ProductViewModel viewModel);

        void Delete(int id);

        ProductViewModel GetById(int id);

        void Save();

        void ImportExcel(string filePath, int categoryId);

        void AddQuantity(int productId, List<ProductQuantityViewModel> productQuantities);

        List<ProductQuantityViewModel> GetQuantities(int productId);

        List<ProductImageViewModel> GetImages(int product);

        void AddImage(int productId, string[] paths);

        void AddWholePrice(int productId, List<WholePriceViewModel> wholePrices);

        List<WholePriceViewModel> GetWholePrices(int productId);

        List<ProductViewModel> GetLastest(int top);

        List<ProductViewModel> GetHotProduct(int top);

        PagedResult<ProductViewModel> GetAllPaging(int? categoryId, string keyword, int page, int pageSize,
            string sortBy);

        List<ProductViewModel> GetRelatedProducts(int productId, int top);

        List<ColorViewModel> GetColors(int productId);

        List<SizeViewModel> GetSizes(int productId);

        List<ProductViewModel> GetUpSellProducts(int top);

        List<TagViewModel> GetProductTags(int productId);

        List<SearchQueryViewModel> SearchQuery(string q);

    }
}