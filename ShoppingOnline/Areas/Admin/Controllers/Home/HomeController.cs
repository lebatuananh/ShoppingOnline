using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShoppignOnline.Application.Dapper.Interfaces;
using ShoppingOnline.WebApplication.Areas.Admin.Controllers.Base;

namespace ShoppingOnline.WebApplication.Areas.Admin.Controllers.Home
{
    public class HomeController : BaseController
    {
        private readonly IReportService _reportService;
        public HomeController(IReportService reportService)
        {
            _reportService = reportService;
        }
        
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetRevenue(string fromDate, string toDate)
        {
            return new OkObjectResult(await _reportService.GetReportAsync(fromDate, toDate));
        }
    }
}