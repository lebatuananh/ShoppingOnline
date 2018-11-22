using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using ShoppignOnline.Application.Dapper.Interfaces;
using ShoppingOnline.Application.ECommerce.Bills;
using ShoppingOnline.Application.ECommerce.Bills.Dtos;
using ShoppingOnline.Application.ECommerce.Carts;
using ShoppingOnline.Application.ECommerce.Products;
using ShoppingOnline.Application.Systems.Announcements.Dtos;
using ShoppingOnline.Application.Systems.Users;
using ShoppingOnline.Data.Enum;
using ShoppingOnline.Utilities.Constants;
using ShoppingOnline.WebApplication.Extensions;
using ShoppingOnline.WebApplication.Models;
using ShoppingOnline.WebApplication.Services;
using ShoppingOnline.WebApplication.SignalR;
using IEmailSender = ShoppingOnline.WebApplication.Services.IEmailSender;

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
            IColorDapperService colorDapperService, ISizeDapperService sizeDapperService, IBillService billService,
            IViewRenderService viewRenderService, IConfiguration configuration, IEmailSender emailSender,
            IAppUserService userService, IHubContext<ChatHub> hubContext)
        {
            _productService = productService;
            _cartService = cartService;
            _colorDapperService = colorDapperService;
            _sizeDapperService = sizeDapperService;
            _billService = billService;
            _viewRenderService = viewRenderService;
            _configuration = configuration;
            _emailSender = emailSender;
            _userService = userService;
            _hubContext = hubContext;
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
                        Status = Status.Active
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
                        UserId = null,
                        Content = $"New bill has been created",
                        Id = notificationId
                    };

                    var users =  await _userService.AnnouncementUsers("BILL");

                    var announUsers = new List<AnnouncementUserViewModel>();

                    foreach (var item in users)
                    {
                        announUsers.Add(new AnnouncementUserViewModel()
                        {
                            AnnouncementId=notificationId,
                            HasRead=false,
                            UserId= (Guid)item.Id
                        });
                    }

                    announcement.AnnouncementUsers = announUsers;
                    
                    _billService.Create(billViewModel, announcement);

                    try
                    {

                        _billService.Save();

                        //var content = await _viewRenderService.RenderToStringAsync("Cart/_BillMail", billViewModel);
                        //Send mail
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
    }
}