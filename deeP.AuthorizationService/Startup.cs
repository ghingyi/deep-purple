using deeP.AuthorizationService.Infrastructure;
using deeP.AuthorizationService.Providers;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;

[assembly: OwinStartup(typeof(deeP.AuthorizationService.Startup))]

namespace deeP.AuthorizationService
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration httpConfig = new HttpConfiguration();

            ConfigureOAuthTokenGeneration(app);

            ConfigureOAuthTokenConsumption(app);

            ConfigureWebApi(httpConfig);

            string originString = ConfigurationManager.AppSettings[InternalConstants.AppSettingKeys.Origins];
            string[] origins = (originString ?? "*").Split(',', ';');
            app.UseCors(new Microsoft.Owin.Cors.CorsOptions() { PolicyProvider = new CustomCorsPolicyProviderAttribute(origins) });

            app.UseWebApi(httpConfig);
        }

        private void ConfigureOAuthTokenGeneration(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);

            string originString = ConfigurationManager.AppSettings[InternalConstants.AppSettingKeys.Origins];
            string[] origins = (originString ?? "*").Split(',', ';');
            string issuer = ConfigurationManager.AppSettings[InternalConstants.AppSettingKeys.Issuer] ?? "";
            string audienceString = ConfigurationManager.AppSettings[InternalConstants.AppSettingKeys.Audiences];
            string[] audiences = (audienceString ?? "").Split(',', ';');
            byte[] audienceSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings[InternalConstants.AppSettingKeys.AudienceSecret] ?? "");
            int expireMinutes = int.Parse(ConfigurationManager.AppSettings[InternalConstants.AppSettingKeys.ExpirationMinutes] ?? "60");

            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                //For Dev enviroment only (on production should be AllowInsecureHttp = false)
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/oauth/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(expireMinutes),
                Provider = new CustomOAuthProvider(origins),
                AccessTokenFormat = new CustomJwtFormat(issuer, audiences.FirstOrDefault(), audienceSecret)
            };

            // OAuth 2.0 Bearer Access Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
        }

        private void ConfigureOAuthTokenConsumption(IAppBuilder app)
        {
            string issuer = ConfigurationManager.AppSettings[InternalConstants.AppSettingKeys.Issuer] ?? "";
            string audienceString = ConfigurationManager.AppSettings[InternalConstants.AppSettingKeys.Audiences];
            string[] audiences = (audienceString ?? "").Split(',', ';');
            byte[] audienceSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings[InternalConstants.AppSettingKeys.AudienceSecret] ?? "");

            // Api controllers with an [Authorize] attribute will be validated with JWT
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

        private void ConfigureWebApi(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}