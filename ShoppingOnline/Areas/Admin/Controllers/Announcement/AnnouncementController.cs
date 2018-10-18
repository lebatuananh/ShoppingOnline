using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Application.Systems.Announcements;
using ShoppingOnline.Data.Entities.System;
using ShoppingOnline.WebApplication.Areas.Admin.Controllers.Base;
using ShoppingOnline.WebApplication.Authorization;
using ShoppingOnline.WebApplication.Extensions;

namespace ShoppingOnline.WebApplication.Areas.Admin.Controllers.Announcement
{
    public class AnnouncementController : BaseController
    {
        private readonly IAnnouncementService _announcementService;
        private readonly IAuthorizationService _authorizationService;
        private readonly SignInManager<AppUser> _signInManager;

        public AnnouncementController(IAnnouncementService announcementService,
            IAuthorizationService authorizationService, SignInManager<AppUser> signInManager)
        {
            _announcementService = announcementService;
            _authorizationService = authorizationService;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            if ((await _authorizationService.AuthorizeAsync(User, "ANNOUNCEMENT", Operations.Read)).Succeeded == false)
            {
                await _signInManager.SignOutAsync();
                return new RedirectResult("/Admin/Login/Index");
            }

            return View();
        }

        [HttpGet]
        public IActionResult GetAllPaging(int page, int pageSize)
        {
            var model = _announcementService.GetAllUnReadPaging(User.GetUserId(), page, pageSize);
            return new OkObjectResult(model);
        }

        [HttpPost]
        public IActionResult MarkAsRead(string id)
        {
            try
            {
                _announcementService.MarkAsRead(User.GetUserId(), id);
                _announcementService.SaveChanges();
                return new OkResult();
            }
            catch (Exception e)
            {
                return new BadRequestResult();
                throw;
            }
        }

        [HttpPost]
        public IActionResult ReadAll()
        {
            try
            {
                _announcementService.ReadAll(User.GetUserId());
                _announcementService.SaveChanges();
                return new OkResult();
            }
            catch (Exception e)
            {
                return new BadRequestResult();
                throw;
            }
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            try
            {
                _announcementService.Delete(User.GetUserId(), id);
                _announcementService.SaveChanges();
                return new OkResult();
            }
            catch (Exception e)
            {
                return new BadRequestResult();
                throw;
            }
        }
    }
}