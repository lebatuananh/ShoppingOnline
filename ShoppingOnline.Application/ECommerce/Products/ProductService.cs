using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using OfficeOpenXml;
using ShoppingOnline.Application.Content.Dtos;
using ShoppingOnline.Application.ECommerce.Products.Dtos;
using ShoppingOnline.Data.Entities.Content;
using ShoppingOnline.Data.Entities.ECommerce;
using ShoppingOnline.Data.Enum;
using ShoppingOnline.Infrastructure.Interfaces;
using ShoppingOnline.Utilities.Constants;
using ShoppingOnline.Utilities.Dtos;
using ShoppingOnline.Utilities.Helpers;

namespace ShoppingOnline.Application.ECommerce.Products
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product, int> _productRepository;
        private readonly IRepository<ProductCategory, int> _productCategoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ProductTag, int> _productTagRepository;
        private readonly IRepository<Tag, string> _tagRepository;
        private readonly IRepository<ProductQuantity, int> _productQuantityRepository;
        private readonly IRepository<ProductImage, int> _productImageRepository;
        private readonly IRepository<WholePrice, int> _wholePriceRepository;
        private readonly IRepository<Color, int> _colorRepository;
        private readonly IRepository<Size, int> _sizeRepository;

        public ProductService(IRepository<Product, int> productRepository, IRepository<ProductCategory, int> productCategoryRepository, IUnitOfWork unitOfWork,
            IRepository<ProductTag, int> productTagRepository, IRepository<Tag, string> tagRepository,
            IRepository<ProductQuantity, int> productQuantityRepository,
            IRepository<ProductImage, int> productImageRepository, IRepository<WholePrice, int> wholePriceRepository,
            IRepository<Color, int> colorRepository, IRepository<Size, int> sizeRepository)
        {
            _productRepository = productRepository;
            _productCategoryRepository = productCategoryRepository;
            _unitOfWork = unitOfWork;
            _productTagRepository = productTagRepository;
            _tagRepository = tagRepository;
            _productQuantityRepository = productQuantityRepository;
            _productImageRepository = productImageRepository;
            _wholePriceRepository = wholePriceRepository;
            _colorRepository = colorRepository;
            _sizeRepository = sizeRepository;
        }

        public ProductViewModel Add(ProductViewModel viewModel)
        {
            List<ProductTag> productTags = new List<ProductTag>();
            var product = Mapper.Map<ProductViewModel, Product>(viewModel);

            if (!string.IsNullOrEmpty(viewModel.Tags))
            {
                string[] tags = viewModel.Tags.Split(',');

                foreach (var t in tags)
                {
                    var tagId = TextHelper.ToUnsignString(t);

                    if (!_tagRepository.FindAll(x => x.Id.Equals(tagId)).Any())
                    {
                        Tag tag = new Tag()
                        {
                            Id = tagId,
                            Name = t,
                            Type = CommonConstants.ProductTag
                        };

                        _tagRepository.Add(tag);
                    }

                    ProductTag productTag = new ProductTag()
                    {
                        TagId = tagId
                    };

                    productTags.Add(productTag);
                }

                foreach (var productTag in productTags)
                {
                    product.ProductTags.Add(productTag);
                }
            }

            _productRepository.Add(product);
            return viewModel;
        }

        public void AddImage(int productId, string[] paths)
        {
            _productImageRepository.RemoveMultiple(_productImageRepository.FindAll(n => n.ProductId == productId)
                .ToList());

            foreach (var item in paths)
            {
                _productImageRepository.Add(new ProductImage()
                {
                    Caption = string.Empty,
                    ProductId = productId,
                    Path = item
                });
            }
        }

        public void AddQuantity(int productId, List<ProductQuantityViewModel> productQuantities)
        {
            _productQuantityRepository.RemoveMultiple(_productQuantityRepository.FindAll(n => n.ProductId == productId)
                .ToList());

            foreach (var item in productQuantities)
            {
                _productQuantityRepository.Add(new ProductQuantity()
                {
                    ProductId = productId,
                    ColorId = item.ColorId,
                    Quantity = item.Quantity,
                    SizeId = item.SizeId
                });
            }
        }

        public void AddWholePrice(int productId, List<WholePriceViewModel> wholePrices)
        {
            _wholePriceRepository.RemoveMultiple(_wholePriceRepository.FindAll(x => x.ProductId == productId).ToList());
            foreach (var wholePrice in wholePrices)
            {
                _wholePriceRepository.Add(new WholePrice()
                {
                    ProductId = productId,
                    FromQuantity = wholePrice.FromQuantity,
                    ToQuantity = wholePrice.ToQuantity,
                    Price = wholePrice.Price
                });
            }
        }

        public void Delete(int id)
        {
            _productRepository.Remove(_productRepository.FindSingle(x => x.Id == id));
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public List<ProductViewModel> GetAll()
        {
            return _productRepository.FindAll(x => x.ProductCategory).ProjectTo<ProductViewModel>().ToList();
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
            var query = _productRepository.FindAll(x => x.Status == Status.Active);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.Name.Contains(keyword));

            if (categoryId.HasValue)
                query = query.Where(x => x.CategoryId == categoryId);

            int totalRow = query.Count();

            query = query.OrderBy(n => n.DateCreated).Skip((page - 1) * pageSize).Take(pageSize);

            var data = query.ProjectTo<ProductViewModel>().ToList();

            switch (sortBy)
            {
                case "price":
                    data = data.OrderBy(n => n.Price).ToList();
                    break;

                case "name":
                    data = data.OrderBy(n => n.Name).ToList();
                    break;
                default:
                    break;
            }

            var paginationSet = new PagedResult<ProductViewModel>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };

            return paginationSet;
        }

        public ProductViewModel GetById(int id)
        {
            var query = _productRepository.FindSingle(x => x.Id == id);
            var model = Mapper.Map<Product, ProductViewModel>(query);
            return model;
        }

        public List<ColorViewModel> GetColors(int productId)
        {
            var colors = _colorRepository.FindAll();
            var productQuantities = _productQuantityRepository.FindAll();

            var query = from c in colors
                join p in productQuantities
                    on c.Id equals p.ColorId
                where p.ProductId == productId && p.Quantity > 0
                group p by p.ColorId
                into g
                select new ColorViewModel
                {
                    Id = g.Key,
                };

            var lstColor = new List<Color>();

            foreach (var item in query)
            {
                lstColor.Add(_colorRepository.FindById(item.Id));
            }

            var model = Mapper.Map<List<Color>, List<ColorViewModel>>(lstColor);

            return model;
        }

        public List<ProductViewModel> GetHotProduct(int top)
        {
            return _productRepository.FindAll(x => x.Status == Status.Active && x.HotFlag == true)
                .OrderByDescending(x => x.DateCreated)
                .Take(top)
                .ProjectTo<ProductViewModel>()
                .ToList();
        }

        public List<ProductImageViewModel> GetImages(int productId)
        {
            return _productImageRepository.FindAll(n => n.ProductId == productId).ProjectTo<ProductImageViewModel>()
                .ToList();
        }

        public List<ProductViewModel> GetLastest(int top)
        {
            return _productRepository.FindAll(x => x.Status == Status.Active).OrderByDescending(x => x.DateCreated)
                .Take(top).ProjectTo<ProductViewModel>().ToList();
        }

        public List<TagViewModel> GetProductTags(int productId)
        {
            var tags = _tagRepository.FindAll();
            var productTags = _productTagRepository.FindAll();

            var query = from t in tags
                join pt in productTags
                    on t.Id equals pt.TagId
                where pt.ProductId == productId
                select new TagViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    Type = "PRODUCT"
                };

            return query.ToList();
        }

        public List<ProductQuantityViewModel> GetQuantities(int productId)
        {
            return _productQuantityRepository.FindAll(n => n.ProductId == productId)
                .ProjectTo<ProductQuantityViewModel>().ToList();
        }

        public List<ProductViewModel> GetRelatedProducts(int productId, int top)
        {
            var product = _productRepository.FindById(productId);
            return _productRepository.FindAll(n =>
                    n.Status == Status.Active && n.CategoryId == product.CategoryId && n.Id != productId).Take(top)
                .ProjectTo<ProductViewModel>().ToList();
        }

        public List<SizeViewModel> GetSizes(int productId)
        {
            var sizes = _sizeRepository.FindAll();
            var productQuantities = _productQuantityRepository.FindAll();

            var query = from c in sizes
                join p in productQuantities
                    on c.Id equals p.SizeId
                where p.ProductId == productId && p.Quantity > 0
                group p by p.ColorId
                into g
                select new SizeViewModel
                {
                    Id = g.Key,
                };

            var lstSize = new List<Size>();

            foreach (var item in query)
            {
                lstSize.Add(_sizeRepository.FindById(item.Id));
            }

            var model = Mapper.Map<List<Size>, List<SizeViewModel>>(lstSize);

            return model;
        }

        public List<ProductViewModel> GetUpSellProducts(int top)
        {
            return _productRepository.FindAll(n => n.PromotionPrice != null).Take(top).ProjectTo<ProductViewModel>()
                .ToList();
        }

        public List<WholePriceViewModel> GetWholePrices(int productId)
        {
            return _wholePriceRepository.FindAll(x => x.ProductId == productId).ProjectTo<WholePriceViewModel>()
                .ToList();
        }

        public void ImportExcel(string filePath, int categoryId)
        {
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                Product product;
                for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
                {
                    product = new Product();
                    product.CategoryId = categoryId;

                    product.Name = workSheet.Cells[i, 1].Value.ToString();

                    product.Description = workSheet.Cells[i, 2].Value.ToString();

                    decimal.TryParse(workSheet.Cells[i, 3].Value.ToString(), out var originalPrice);
                    product.OriginalPrice = originalPrice;

                    decimal.TryParse(workSheet.Cells[i, 4].Value.ToString(), out var price);
                    product.Price = price;
                    decimal.TryParse(workSheet.Cells[i, 5].Value.ToString(), out var promotionPrice);

                    product.PromotionPrice = promotionPrice;
                    product.Content = workSheet.Cells[i, 6].Value.ToString();
                    product.SeoKeywords = workSheet.Cells[i, 7].Value.ToString();

                    product.SeoDescription = workSheet.Cells[i, 8].Value.ToString();
                    bool.TryParse(workSheet.Cells[i, 9].Value.ToString(), out var hotFlag);

                    product.HotFlag = hotFlag;
                    bool.TryParse(workSheet.Cells[i, 10].Value.ToString(), out var homeFlag);
                    product.HomeFlag = homeFlag;

                    product.Status = Status.Active;

                    _productRepository.Add(product);
                }
            }
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void Update(ProductViewModel viewModel)
        {
            List<ProductTag> productTags = new List<ProductTag>();
            _productTagRepository.RemoveMultiple(_productTagRepository.FindAll(n => n.Id == viewModel.Id).ToList());

            if (!string.IsNullOrEmpty(viewModel.Tags))
            {
                var tags = viewModel.Tags.Split(',');

                foreach (var t in tags)
                {
                    var tagId = TextHelper.ToUnsignString(t);

                    if (!_tagRepository.FindAll(n => n.Id.Equals(tagId)).Any())
                    {
                        Tag tag = new Tag()
                        {
                            Id = tagId,
                            Name = t,
                            Type = CommonConstants.ProductTag
                        };

                        _tagRepository.Add(tag);
                    }

                    ProductTag productTag = new ProductTag()
                    {
                        TagId = tagId,
                        ProductId = viewModel.Id
                    };

                    productTags.Add(productTag);
                    _productTagRepository.Add(productTag);
                }
            }

            var product = Mapper.Map<ProductViewModel, Product>(viewModel);

            foreach (var productTag in productTags)
            {
                product.ProductTags.Add(productTag);
            }

            _productRepository.Update(product);
        }

        public List<SearchQueryViewModel> SearchQuery(string q)
        {
            var model = new List<SearchQueryViewModel>();
            _productRepository.FindAll(x => x.Name.Contains(q)).ToList().ForEach(x =>
            {
                model.Add(new SearchQueryViewModel() { Id = x.Id, Name = x.Name, Alias = x.SeoAlias, Category = _productCategoryRepository.FindById(x.CategoryId).Name, CategoryAlias = _productCategoryRepository.FindById(x.CategoryId).SeoAlias });
            });
            return model;
        }

    }
}