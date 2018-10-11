using System;
using System.Linq;
using System.Security.Claims;

namespace ShoppingOnline.WebApplication.Extensions
{
    public static class IdentityExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            var claim = ((ClaimsIdentity) claimsPrincipal.Identity).Claims.Single(x => x.Type == "UserId");
            return Guid.Parse(claim.Value);
        }
        
        public static string GetSpecificClaim(this ClaimsPrincipal claimsPrincipal, string claimType)
        {
            var claim = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == claimType);
            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}