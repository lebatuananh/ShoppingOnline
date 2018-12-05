using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Application.Common.Feedbacks;
using ShoppingOnline.Data.Entities.System;
using ShoppingOnline.WebApplication.Areas.Admin.Controllers.Base;
using ShoppingOnline.WebApplication.Authorization;

namespace ShoppingOnline.WebApplication.Areas.Admin.Controllers.Feedback
{
    public class FeedbackController : BaseController
    {
        private readonly IFeedbackService _feedbackService;
        private readonly IAuthorizationService _authorizationService;
        private readonly SignInManager<AppUser> _signInManager;

        public FeedbackController(IFeedbackService feedbackService, IAuthorizationService authorizationService,
            SignInManager<AppUser> signInManager)
        {
            _feedbackService = feedbackService;
            _authorizationService = authorizationService;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            if ((await _authorizationService.AuthorizeAsync(User, "FEEDBACK", Operations.Read)).Succeeded == false)
            {
                await _signInManager.SignOutAsync();
                return new RedirectResult("/Admin/Login/Index");
            }

            return View();
        }

        [HttpGet]
        public IActionResult GetAllPaging(string keyword, int page, int pageSize)
        {
            var model = _feedbackService.GetAllPaging(keyword, page, pageSize);
            return new OkObjectResult(model);
        }
    }
}