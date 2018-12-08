using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppignOnline.Application.Dapper.Interfaces;
using ShoppingOnline.Application.ECommerce.Bills;
using ShoppingOnline.Application.ECommerce.Bills.Dtos;
using ShoppingOnline.Application.ECommerce.Carts;
using ShoppingOnline.Application.ECommerce.Products;
using ShoppingOnline.Application.Systems.Announcements.Dtos;
using ShoppingOnline.Application.Systems.Users;
using ShoppingOnline.Data.Enum;
using ShoppingOnline.Utilities.Constants;
using ShoppingOnline.WebApplication.Models;
using ShoppingOnline.WebApplication.Services;
using ShoppingOnline.WebApplication.SignalR;
using ShoppingOnline.WebApplication.Extensions;

namespace ShoppingOnline.WebApplication.Controllers.Cart
{
    public class CartController : Controller
    {
        private IProductService _productService;
        private ICartService _cartService;
        private IColorDapperService _colorDapperService;
        private ISizeDapperService _sizeDapperService;
        private IBillService _billService;
        private IViewRenderService _viewRenderService;
        private IConfiguration _configuration;
        private IEmailSender _emailSender;
        private IAppUserService _userService;
        private readonly IHubContext<ChatHub> _hubContext;

        public CartController(IProductService productService, ICartService cartService,
            IColorDapperService colorDapperService,
            ISizeDapperService sizeDapperService, IBillService billService, IViewRenderService viewRenderService,
            IConfiguration configuration,
            IEmailSender emailSender, IHubContext<ChatHub> hubContext, IAppUserService userService)
        {
            _productService = productService;
            _cartService = cartService;
            _colorDapperService = colorDapperService;
            _sizeDapperService = sizeDapperService;
            _billService = billService;
            _viewRenderService = viewRenderService;
            _configuration = configuration;
            _emailSender = emailSender;
            _hubContext = hubContext;
            _userService = userService;
        }

        [Route("cart.html", Name = "Cart")]
        public IActionResult Index()
        {
            ViewData["BodyClass"] = "shopping_cart_page";
            return View();
        }

        [Route("checkout.html", Name = "Checkout")]
        [HttpGet]
        public IActionResult Checkout()
        {
            ViewData["BodyClass"] = "shopping_cart_page";
            var model = new CheckoutViewModel();

            var session = HttpContext.Session.Get<List<ShoppingCartViewModel>>(CommonConstants.CartSession);
            if (session.Any(x => x.Color == null || x.Size == null))
            {
                return Redirect("/cart.html");
            }

            model.Carts = session;
            return View(model);
        }

        [Route("checkout.html", Name = "Checkout")]
        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var session = HttpContext.Session.Get<List<ShoppingCartViewModel>>(CommonConstants.CartSession);

            if (ModelState.IsValid)
            {
                if (session != null)
                {
                    var details = new List<BillDetailViewModel>();
                    foreach (var item in session)
                    {
                        details.Add(new BillDetailViewModel()
                        {
                            Price = item.Price,
                            ColorId = item.Color.Id,
                            SizeId = item.Size.Id,
                            Quantity = item.Quantity,
                            ProductId = item.Product.Id
                        });
                    }

                    var billViewModel = new BillViewModel()
                    {
                        CustomerMobile = model.CustomerMobile,
                        BillStatus = BillStatus.New,
                        CustomerAddress = model.CustomerAddress,
                        CustomerName = model.CustomerName,
                        CustomerMessage = model.CustomerMessage,
                        BillDetails = details,
                        Status = Status.Active,
                        ShipperId=1
                    };

                    if (User.Identity.IsAuthenticated == true)
                    {
                        billViewModel.CustomerId = Guid.Parse(User.GetSpecificClaim("UserId"));
                    }

                    var notificationId = Guid.NewGuid().ToString();


                    var announcement = new AnnouncementViewModel()
                    {
                        Title = "New bill",
                        DateCreated = DateTime.Now,
                        UserId = Guid.Parse(User.GetSpecificClaim("UserId")),
                        Content = $"New bill has been created",
                        Id = notificationId
                    };

                    var users = await _userService.AnnouncementUsers("BILL");

                    var announUsers = new List<AnnouncementUserViewModel>();

                    foreach (var item in users)
                    {
                        announUsers.Add(new AnnouncementUserViewModel()
                        {
                            AnnouncementId = notificationId,
                            HasRead = false,
                            UserId = (Guid) item.Id
                        });
                    }

                    announcement.AnnouncementUsers = announUsers;

                    _billService.Create(billViewModel, announcement);

                    try
                    {
                        _billService.Save();

                        //var content = await _viewRenderService.RenderToStringAsync("Cart/_BillMail", billViewModel);
                        ////Send mail
                        //await _emailSender.SendEmailAsync(_configuration["MailSettings:AdminMail"], "New bill from ShopMart", content);

                        ViewData["Success"] = true;

                        await _hubContext.Clients.All.SendAsync("ReceiveMessage", announcement);
                    }
                    catch (Exception ex)
                    {
                        ViewData["Success"] = false;
                        ModelState.AddModelError("", ex.Message);
                    }
                }
            }

            model.Carts = session;
            return View(model);
        }

        #region AJAX Request

        /// <summary>
        /// Get list item
        /// </summary>
        /// <returns></returns>
        public IActionResult GetCart()
        {
            var session = HttpContext.Session.Get<List<ShoppingCartViewModel>>(CommonConstants.CartSession);
            if (session == null)
                session = new List<ShoppingCartViewModel>();
            return new OkObjectResult(session);
        }

        /// <summary>
        /// Remove all products in cart
        /// </summary>
        /// <returns></returns>
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove(CommonConstants.CartSession);
            return new OkObjectResult("OK");
        }

        /// <summary>
        /// Add product to cart
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity, int colorId, int sizeId)
        {
            //Get product detail
            var product = _productService.GetById(productId);
            var color = _cartService.GetColor(colorId);
            var size = _cartService.GetSize(sizeId);
            //Get session with item list from cart
            var session = HttpContext.Session.Get<List<ShoppingCartViewModel>>(CommonConstants.CartSession);
            if (session != null)
            {
                //Convert string to list object
                bool hasChanged = false;

                //Check exist with item product id
                if (session.Any(x => x.Product.Id == productId))
                {
                    foreach (var item in session)
                    {
                        //Update quantity for product if match product id
                        if (item.Product.Id == productId)
                        {
                            item.Quantity += quantity;
                            item.Price = product.PromotionPrice ?? product.Price;
                            hasChanged = true;
                        }
                    }
                }
                else
                {
                    session.Add(new ShoppingCartViewModel()
                    {
                        Product = product,
                        Quantity = quantity,
                        Color = color,
                        Size = size,
                        Price = product.PromotionPrice ?? product.Price
                    });
                    hasChanged = true;
                }

                //Update back to cart
                if (hasChanged)
                {
                    HttpContext.Session.Set(CommonConstants.CartSession, session);
                }
            }
            else
            {
                //Add new cart
                var cart = new List<ShoppingCartViewModel>();
                cart.Add(new ShoppingCartViewModel()
                {
                    Product = product,
                    Quantity = quantity,
                    Color = color,
                    Size = size,
                    Price = product.PromotionPrice ?? product.Price
                });
                HttpContext.Session.Set(CommonConstants.CartSession, cart);
            }

            return new OkResult();
        }

        /// <summary>
        /// Remove a product
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public IActionResult RemoveFromCart(int productId)
        {
            var session = HttpContext.Session.Get<List<ShoppingCartViewModel>>(CommonConstants.CartSession);
            if (session != null)
            {
                bool hasChanged = false;
                foreach (var item in session)
                {
                    if (item.Product.Id == productId)
                    {
                        session.Remove(item);
                        hasChanged = true;
                        break;
                    }
                }

                if (hasChanged)
                {
                    HttpContext.Session.Set(CommonConstants.CartSession, session);
                }

                return new OkObjectResult(productId);
            }

            return new EmptyResult();
        }

        /// <summary>
        /// Update product quantity
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public IActionResult UpdateCart(int productId, int quantity, int colorId, int sizeId)
        {
            var session = HttpContext.Session.Get<List<ShoppingCartViewModel>>(CommonConstants.CartSession);
            if (session != null)
            {
                bool hasChanged = false;
                foreach (var item in session)
                {
                    if (item.Product.Id == productId)
                    {
                        var product = _productService.GetById(productId);
                        item.Product = product;
                        item.Size = _cartService.GetSize(sizeId);
                        item.Color = _cartService.GetColor(colorId);
                        item.Quantity = quantity;
                        item.Price = product.PromotionPrice ?? product.Price;
                        hasChanged = true;
                    }
                }

                if (hasChanged)
                {
                    HttpContext.Session.Set(CommonConstants.CartSession, session);
                }

                return new OkObjectResult(productId);
            }

            return new EmptyResult();
        }

        [HttpGet]
        public IActionResult GetColors(int productId)
        {
            var colors = _colorDapperService.GetColors(productId);
            return new OkObjectResult(colors);
        }

        [HttpGet]
        public IActionResult GetSizes(int productId)
        {
            var sizes = _sizeDapperService.GetSizes(productId);
            return new OkObjectResult(sizes);
        }

        #endregion AJAX Request
    }
}