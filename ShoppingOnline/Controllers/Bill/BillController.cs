using System;
using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Application.ECommerce.Bills;
using ShoppingOnline.Application.ECommerce.Bills.Dtos;
using ShoppingOnline.WebApplication.Extensions;
using ShoppingOnline.WebApplication.Models;

namespace ShoppingOnline.WebApplication.Controllers.Bill
{
    public class BillController:Controller
    {
        private readonly IBillService _billService;

        public BillController(IBillService billService)
        {
            _billService = billService;
        }
        
        [Route("bill.html", Name = "bill")]
        public IActionResult Index()
        {
            var model = _billService.GetAll(Guid.Parse(User.GetSpecificClaim("UserId")));
            return View(model);
        }
        
        [Route("bill.{id}.html")]
        public IActionResult Details(int id)
        {
            var model = new PayViewModel();
            model.Bill = _billService.GetDetail(id);
            model.Details = _billService.GetBillDetails(id);
            return View(model);
        }
    }
}