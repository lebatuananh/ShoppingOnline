using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper.QueryableExtensions;
using ShoppingOnline.Application.Content.Dtos;
using ShoppingOnline.Application.ECommerce.Products.Dtos;
using ShoppingOnline.Data.Entities.ECommerce;
using ShoppingOnline.Data.Enum;
using ShoppingOnline.Infrastructure.Interfaces;
using ShoppingOnline.Utilities.Dtos;

namespace ShoppingOnline.Application.ECommerce.Products
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product, int> _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IRepository<Product, int> productRepository, IUnitOfWork unitOfWork)
        {
            this._productRepository = productRepository;
            this._unitOfWork = unitOfWork;
        }

        public ProductViewModel Add(ProductViewModel viewModel)
        {
            throw new NotImplementedException();
        }

        public void AddImage(int productId, string[] paths)
        {
            throw new NotImplementedException();
        }

        public void AddQuantity(int productId, List<ProductQuantityViewModel> productQuantities)
        {
            throw new NotImplementedException();
        }

        public void AddWholePrice(int productId, List<WholePriceViewModel> wholePrices)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public List<ProductViewModel> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public PagedResult<ProductViewModel> GetAllPaging(int? categoryId, string keyword, int page, int pageSize)
        {
            var query = _productRepository.FindAll(x => x.Status == Status.Active);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.Name.Contains(keyword) || x.ProductCategory.Name.Contains(keyword));

            if (categoryId.HasValue)
                query = query.Where(x => x.CategoryId == categoryId);

            int totalRow = query.Count();

            query = query.OrderByDescending(n => n.DateCreated).Skip((page - 1) * pageSize).Take(pageSize);

            var data = query.ProjectTo<ProductViewModel>().ToList();

            var paginationSet = new PagedResult<ProductViewModel>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };

            return paginationSet;
        }

        public PagedResult<ProductViewModel> GetAllPaging(int? categoryId, string keyword, int page, int pageSize,
            string sortBy)
        {
            throw new NotImplementedException();
        }

        public ProductViewModel GetById(int id)
        {
            throw new NotImplementedException();
        }

        public List<ColorViewModel> GetColors(int productId)
        {
            throw new NotImplementedException();
        }

        public List<ProductViewModel> GetHotProduct(int top)
        {
            throw new NotImplementedException();
        }

        public List<ProductImageViewModel> GetImages(int product)
        {
            throw new NotImplementedException();
        }

        public List<ProductViewModel> GetLastest(int top)
        {
            throw new NotImplementedException();
        }

        public List<TagViewModel> GetProductTags(int productId)
        {
            throw new NotImplementedException();
        }

        public List<ProductQuantityViewModel> GetQuantities(int productId)
        {
            throw new NotImplementedException();
        }

        public List<ProductViewModel> GetRelatedProducts(int productId, int top)
        {
            throw new NotImplementedException();
        }

        public List<SizeViewModel> GetSizes(int productId)
        {
            throw new NotImplementedException();
        }

        public List<ProductViewModel> GetUpSellProducts(int top)
        {
            throw new NotImplementedException();
        }

        public List<WholePriceViewModel> GetWholePrices(int productId)
        {
            throw new NotImplementedException();
        }

        public void ImportExcel(string filePath, int categoryId)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Update(ProductViewModel viewModel)
        {
            throw new NotImplementedException();
        }
    }
}