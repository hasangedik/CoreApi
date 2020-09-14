using System;
using System.Security.Claims;
using System.Security.Principal;

namespace CoreApi.Common.Extensions
{
    public static class ClaimExtensions
    {
        public static int GetId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("id");
            return (claim != null) ? Convert.ToInt32(claim.Value) : default;
        }

        public static string GetFullName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("name");
            return claim?.Value;
        }
        
        public static string GetEmail(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("email");
            return claim?.Value;
        }
    }
}
