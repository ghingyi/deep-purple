using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace deeP.AuthorizationService
{
    internal static class InternalConstants
    {
        internal static class AppSettingKeys
        {
            internal const string Origins = "Origins";
            internal const string Issuer = "Issuer";
            internal const string Audiences = "Audiences";
            internal const string AudienceSecret = "AudienceSecret";
            internal const string ExpirationMinutes = "ExpirationMinutes";
        }

        internal static class RoleNames
        {
            internal const string Admin = "Admin";
            internal const string Seller = "Seller";
            internal const string Buyer = "Buyer";
        }
    }
}