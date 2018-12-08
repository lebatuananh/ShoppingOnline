using Microsoft.AspNetCore.Identity;
using ShoppingOnline.Utilities.Constants;
using ShoppingOnline.WebApplication.DataProtects.Emails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingOnline.WebApplication.DataProtects
{
    public static class CustomIdentityBuilderExtensions
    {
        public static IdentityBuilder AddEmailConfirmTokenProvider(this IdentityBuilder builder)
        {
            var userType = builder.UserType;
            var provider = typeof(EmailConfirmationTokenProvider<>).MakeGenericType(userType);
            return builder.AddTokenProvider(SystemConstants.TokenProvider.EmailConfirm, provider);
        }
    }
}
