using Microsoft.AspNetCore.Identity;
using ShoppingOnline.Utilities.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingOnline.WebApplication.DataProtects.Emails
{
    public class EmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
    {
        public EmailConfirmationTokenProviderOptions()
        {
            // update the defaults
            Name = SystemConstants.TokenProvider.EmailConfirm;
            TokenLifespan = TimeSpan.FromDays(1);
        }
    }
}
