using deeP.SPAWeb.Services;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Owin;
using System.Web.Mvc;

namespace deeP.SPAWeb
{
    public partial class Startup
    {
        public static void ConfigureOAuthTokenConsumption(IAppBuilder app)
        {
            IConfigurationService configurationService = DependencyResolver.Current.GetService<IConfigurationService>();

            // Read OAuth configuration settings
            string issuer = configurationService.GetSettingString(Constants.SettingName.Issuer) ?? "";
            string audienceString = configurationService.GetSettingString(Constants.SettingName.Audiences);
            string[] audiences = (audienceString ?? "").Split(',', ';');
            byte[] audienceSecret = TextEncodings.Base64Url.Decode(configurationService.GetSettingString(Constants.SettingName.AudienceSecret) ?? "");

            // Api controllers with an [Authorize] attribute will be authorized with JWT
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = audiences,
                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(issuer, audienceSecret)
                    }
                });
        }
    }
}