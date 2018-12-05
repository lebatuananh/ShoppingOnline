using AutoMapper;
using ShoppingOnline.Application.Common.Advertisements.Dtos;
using ShoppingOnline.Application.Common.Contacts.Dtos;
using ShoppingOnline.Application.Common.Dtos;
using ShoppingOnline.Application.Common.Feedbacks.Dtos;
using ShoppingOnline.Application.Common.Slides.Dtos;
using ShoppingOnline.Application.Content.Blogs.Dtos;
using ShoppingOnline.Application.Content.Dtos;
using ShoppingOnline.Application.Content.Pages.Dtos;
using ShoppingOnline.Application.ECommerce.Bills.Dtos;
using ShoppingOnline.Application.ECommerce.ProductCategories.Dtos;
using ShoppingOnline.Application.ECommerce.Products.Dtos;
using ShoppingOnline.Application.Systems.Announcements.Dtos;
using ShoppingOnline.Application.Systems.Functions.Dtos;
using ShoppingOnline.Application.Systems.Permissions.Dtos;
using ShoppingOnline.Application.Systems.Roles.Dtos;
using ShoppingOnline.Application.Systems.Settings.Dtos;
using ShoppingOnline.Application.Systems.Shippers.Dtos;
using ShoppingOnline.Application.Systems.Users.Dtos;
using ShoppingOnline.Data.Entities.Advertisement;
using ShoppingOnline.Data.Entities.Content;
using ShoppingOnline.Data.Entities.ECommerce;
using ShoppingOnline.Data.Entities.System;
using ProductQuantity = ShoppingOnline.Data.Entities.ECommerce.ProductQuantity;

namespace ShoppingOnline.Application.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
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
            CreateMap<Blog, BlogViewModel>().MaxDepth(2);
            CreateMap<BlogTag, BlogTagViewModel>().MaxDepth(2);
            CreateMap<Footer, FooterViewModel>().MaxDepth(2);
            CreateMap<SystemConfig, SystemConfigViewModel>().MaxDepth(2);
            CreateMap<Slide, SlideViewModel>().MaxDepth(2);
            CreateMap<Page, PageViewModel>().MaxDepth(2);
            CreateMap<Advertisement, AdvertisementViewModel>().MaxDepth(2);
            CreateMap<AdvertisementPage, AdvertisementPageViewModel>().MaxDepth(2);
            CreateMap<AdvertisementPosition, AdvertisementPositionViewModel>().MaxDepth(2);


            //Bill
            CreateMap<Bill, BillViewModel>().MaxDepth(2);
            CreateMap<BillDetail, BillDetailViewModel>().MaxDepth(2);

            //ProductCategory
            CreateMap<ProductCategory, ProductCategoryViewModel>();

            //System
            CreateMap<Function, FunctionViewModel>().MaxDepth(2);
            CreateMap<AppRole, AppRoleViewModel>().MaxDepth(2);
            CreateMap<AppUser, AppUserViewModel>().MaxDepth(2);
            CreateMap<Permission, PermissionViewModel>();
            CreateMap<Announcement, AnnouncementViewModel>();
            CreateMap<AnnouncementUser, AnnouncementUserViewModel>();
            CreateMap<Feedback, FeedbackViewModel>();
            CreateMap<Contact, ContactViewModel>();
            CreateMap<Shipper, ShipperViewModel>();
        }
    }
}