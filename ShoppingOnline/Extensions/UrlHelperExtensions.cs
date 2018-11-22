using System;
using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
using ShoppingOnline.WebApplication.Controllers.Account;

=======
using ShoppingOnline.WebApplication.Areas.Admin.Controllers.Logout;
>>>>>>> c43707ee501c0fbafc6a3f26357bc7390bd08210

namespace ShoppingOnline.WebApplication.Extensions
{
    public static class UrlHelperExtensions
    {
<<<<<<< HEAD
        public static string EmailConfirmationLink(this IUrlHelper urlHelper, Guid userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ConfirmEmail),
                controller: "Account",
                values: new {userId, code},
                protocol: scheme);
        }

        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, Guid userId, string code,
            string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ResetPassword),
                controller: "Account",
                values: new {userId, code},
                protocol: scheme);
        }
=======
//        public static string EmailConfirmationLink(this IUrlHelper urlHelper, Guid userId, string code, string scheme)
//        {
//            return urlHelper.Action(
//                action: nameof(AccountController.ConfirmEmail),
//                controller: "Account",
//                values: new { userId, code },
//                protocol: scheme);
//        }
//
//        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, Guid userId, string code, string scheme)
//        {
//            return urlHelper.Action(
//                action: nameof(AccountController.ResetPassword),
//                controller: "Account",
//                values: new { userId, code },
//                protocol: scheme);
//        }
>>>>>>> c43707ee501c0fbafc6a3f26357bc7390bd08210
    }
}