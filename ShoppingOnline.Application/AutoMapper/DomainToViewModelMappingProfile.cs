using AutoMapper;
using ShoppingOnline.Application.Content.Dtos;
using ShoppingOnline.Application.ECommerce.Bills.Dtos;
using ShoppingOnline.Application.ECommerce.ProductCategories.Dtos;
using ShoppingOnline.Application.ECommerce.Products.Dtos;
using ShoppingOnline.Data.Entities.Content;
using ShoppingOnline.Data.Entities.ECommerce;
using ShoppingOnline.Data.Entities.System;
using ProductQuantity = ShoppingOnline.Data.Entities.ECommerce.ProductQuantity;

namespace ShoppingOnline.Application.AutoMapper
{
    public class DomainToViewModelMappingProfile:Profile
    {
        public DomainToViewModelMappingProfile()
        {
            //ECommerce
            CreateMap<Product, ProductViewModel>();
            CreateMap<ProductTag, ProductTagViewModel>();
            CreateMap<Color, ColorViewModel>().MaxDepth(2);
            CreateMap<Size, SizeViewModel>().MaxDepth(2);
            CreateMap<ProductQuantity, ProductQuantityViewModel>().MaxDepth(2);
            CreateMap<ProductImage, ProductImageViewModel>().MaxDepth(2);
            CreateMap<WholePrice, WholePriceViewModel>().MaxDepth(2);
            
            //Content
            CreateMap<Tag, TagViewModel>();
            
            //Bill
            CreateMap<Bill, BillViewModel>().MaxDepth(2);
            CreateMap<BillDetail, BillDetailViewModel>().MaxDepth(2);
            
            //ProductCategory
            CreateMap<ProductCategory, ProductCategoryViewModel>();
            



        }
    }
}