using System;
using System.Collections.Generic;
using ShoppingOnline.Application.ECommerce.Products.Dtos;
using ShoppingOnline.Data.Entities.ECommerce;
using ShoppingOnline.Infrastructure.Interfaces;

namespace ShoppingOnline.Application.ECommerce.Products
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product, int> _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public List<ProductViewModel> GetAll()
        {
            throw new System.NotImplementedException();
        }
    }
}