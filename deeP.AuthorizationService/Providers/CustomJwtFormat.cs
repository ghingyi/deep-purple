using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Web;
using Thinktecture.IdentityModel.Tokens;

namespace deeP.AuthorizationService.Providers
{
    public class CustomJwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private readonly string _issuer, _audience;
        private readonly byte[] _key;

        public CustomJwtFormat(string issuer, string audience, byte[] key)
        {
            _issuer = issuer;
            _audience = audience;
            _key = key;
        }

        public string Protect(AuthenticationTicket data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            var issued = data.Properties.IssuedUtc;

            var expires = data.Properties.ExpiresUtc;
            
            var signingKey = new HmacSigningCredentials(_key);
            var token = new JwtSecurityToken(_issuer, _audience, data.Identity.Claims, issued.Value.UtcDateTime, expires.Value.UtcDateTime, signingKey);

            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.WriteToken(token);

            return jwt;
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}