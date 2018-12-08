using System;
using AutoMapper;
using ShoppingOnline.Application.Common.Advertisements.Dtos;
using ShoppingOnline.Application.Common.Feedbacks.Dtos;
using ShoppingOnline.Application.Common.Slides.Dtos;
using ShoppingOnline.Application.Content.Blogs.Dtos;
using ShoppingOnline.Application.Content.Pages.Dtos;
using ShoppingOnline.Application.ECommerce.Bills.Dtos;
using ShoppingOnline.Application.ECommerce.ProductCategories.Dtos;
using ShoppingOnline.Application.ECommerce.Products.Dtos;
using ShoppingOnline.Application.Systems.Announcements.Dtos;
using ShoppingOnline.Application.Systems.Permissions.Dtos;
using ShoppingOnline.Application.Systems.Shippers.Dtos;
using ShoppingOnline.Application.Systems.Users.Dtos;
using ShoppingOnline.Data.Entities.Advertisement;
using ShoppingOnline.Data.Entities.Content;
using ShoppingOnline.Data.Entities.ECommerce;
using ShoppingOnline.Data.Entities.System;

namespace ShoppingOnline.Application.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<ProductCategoryViewModel, ProductCategory>()
                .ConstructUsing(c => new ProductCategory(c.Name, c.Description, c.ParentId, c.HomeOrder, c.Image,
                    c.HomeFlag,
                    c.SortOrder, c.Status, c.SeoPageTitle, c.SeoAlias, c.SeoKeywords, c.SeoDescription));

            CreateMap<ProductViewModel, Product>()
                .ConstructUsing(c => new Product(c.Name, c.CategoryId, c.Image, c.Price, c.OriginalPrice,
                    c.PromotionPrice, c.Description, c.Content, c.HomeFlag, c.HotFlag, c.Tags, c.Unit, c.Status,
                    c.SeoPageTitle, c.SeoAlias, c.SeoKeywords, c.SeoDescription));

            CreateMap<BillViewModel, Bill>()
                .ConstructUsing(c => new Bill(c.Id, c.CustomerName, c.CustomerAddress,
                    c.CustomerMobile, c.CustomerMessage, c.BillStatus,
                    c.PaymentMethod, c.Status, c.CustomerId, c.ShipperId));

            CreateMap<BillDetailViewModel, BillDetail>()
                .ConstructUsing(c => new BillDetail(c.Id, c.BillId, c.ProductId,
                    c.Quantity, c.Price, c.ColorId, c.SizeId));

            CreateMap<PermissionViewModel, Permission>()
                .ConstructUsing(c =>
                    new Permission(c.RoleId, c.FunctionId, c.CanCreate, c.CanRead, c.CanUpdate, c.CanDelete));

            CreateMap<AnnouncementViewModel, Announcement>()
                .ConstructUsing(c => new Announcement(c.Title, c.Content, c.UserId, c.Status));

            CreateMap<AnnouncementViewModel, Announcement>()
                .ConstructUsing(c => new Announcement(c.Title, c.Content, c.Status));

            CreateMap<AnnouncementUserViewModel, AnnouncementUser>()
                .ConstructUsing(c => new AnnouncementUser(c.AnnouncementId, c.UserId, c.HasRead));

            CreateMap<AdvertisementViewModel, Advertisement>()
                .ConstructUsing(c => new Advertisement(c.Name, c.Description, c.Image, c.Url, c.PositionId, c.Status,
                    c.DateCreated, c.DateModified, c.SortOrder));

            CreateMap<AdvertisementPageViewModel, AdvertisementPage>()
                .ConstructUsing(c => new AdvertisementPage(c.Name));

            CreateMap<AdvertisementPositionViewModel, AdvertisementPosition>()
                .ConstructUsing(c => new AdvertisementPosition(c.Name, c.PageId));

            CreateMap<SlideViewModel, Slide>().ConstructUsing(c => new Slide());

            CreateMap<FeedbackViewModel, Feedback>().ConstructUsing(c => new Feedback());

            CreateMap<PageViewModel, Page>().ConstructUsing(c => new Page());

            CreateMap<BlogViewModel, Blog>().ConstructUsing(c => new Blog(c.Name, c.Image, c.Description, c.Content,
                c.HomeFlag, c.HotFlag,
                c.Tags, c.Status, c.SeoPageTitle, c.SeoAlias, c.SeoKeywords, c.SeoDescription));

            CreateMap<ShipperViewModel, Shipper>().ConstructUsing(c => new Shipper());

            CreateMap<AppUserViewModel, AppUser>()
                .ConstructUsing(c => new AppUser(c.Id.GetValueOrDefault(Guid.Empty), c.FullName, c.UserName,
                    c.Email, c.PhoneNumber, c.Avatar, c.Status, c.BirthDay, c.Gender));
        }
    }
}