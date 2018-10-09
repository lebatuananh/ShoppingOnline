using System;
using System.Collections.Generic;
using ShoppingOnline.Application.ECommerce.Products.Dtos;

namespace ShoppingOnline.Application.ECommerce.Products
{
    public interface IProductService : IDisposable
    {
        List<ProductViewModel> GetAll();
    }
}