using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace Utils.Tool
{
    public class ExtendIdentity
    {
        public static string GetOrganizationId(IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("OrganizationId");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetUserId(IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("UserId");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetClaim(IIdentity identity,string iclaim)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst(iclaim); 
            return (claim != null) ? claim.Value : string.Empty;
        }


        
    }
}