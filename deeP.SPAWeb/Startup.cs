[assembly: Microsoft.Owin.OwinStartup(typeof(deeP.SPAWeb.Startup))]

namespace deeP.SPAWeb
{
    using System.Web.Mvc;
    using NWebsec.Owin;
    using Owin;
    using System.Web.Http;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // TODO: all below, when going to production and we have an X509

            // Strict-Transport-Security - Adds the Strict-Transport-Security HTTP header to responses.
            //      This HTTP header is only relevant if you are using TLS. It ensures that content is loaded over 
            //      HTTPS and refuses to connect in case of certificate errors and warnings.
            //      Note: Including subdomains and a minimum maxage of 18 weeks is required for preloading.
            //      https://developer.mozilla.org/en-US/docs/Web/Security/HTTP_strict_transport_security
            //      http://www.troyhunt.com/2015/06/understanding-http-strict-transport.html
            // app.UseHsts(options => options.MaxAge(days: 18 * 7).IncludeSubdomains().Preload());

            // Public-Key-Pins - Adds the Public-Key-Pins HTTP header to responses.
            //      This HTTP header is only relevant if you are using TLS. It stops man-in-the-middle attacks by 
            //      telling browsers exactly which TLS certificate you expect.
            //      See https://developer.mozilla.org/en-US/docs/Web/Security/Public_Key_Pinning
            //      and https://scotthelme.co.uk/hpkp-http-public-key-pinning/
            // app.UseHpkp(options => options
            //     .Sha256Pins(
            //         "Base64 encoded SHA-256 hash of your first certificate e.g. cUPcTAZWKaASuYWhhneDttWpY3oBAkE3h2+soZS7sWs=",
            //         "Base64 encoded SHA-256 hash of your second backup certificate e.g. M8HztCzM3elUxkcjR2S5P4hhyBNf6lHkmjAHKhpGPWE=")
            //     .MaxAge(days: 18 * 7))
            //     .IncludeSubdomains();

            // Content-Security-Policy:upgrade-insecure-requests - Adds the 'upgrade-insecure-requests' directive to
            //      the Content-Security-Policy HTTP header. This is only relevant if you are using HTTPS. Any objects
            //      on the page using HTTP is automatically upgraded to HTTPS.
            //      See https://scotthelme.co.uk/migrating-from-http-to-https-ease-the-pain-with-csp-and-hsts/
            //      and http://www.w3.org/TR/upgrade-insecure-requests/
            // app.UseCsp(x => x.UpgradeInsecureRequests());

            ConfigureContainer(app);

            RegisterGlobalFilters(GlobalFilters.Filters);

            ConfigureOAuthTokenConsumption(app);
        }
    }
}
