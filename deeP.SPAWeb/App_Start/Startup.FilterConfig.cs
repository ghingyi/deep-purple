namespace deeP.SPAWeb
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using Boilerplate.Web.Mvc.Filters;
    using deeP.SPAWeb.Constants;
    using NWebsec.Mvc.HttpHeaders;
    using NWebsec.Mvc.HttpHeaders.Csp;
    using deeP.SPAWeb.Filters;
    using deeP.SPAWeb.Services;

    public partial class Startup
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            AddDefaultFilters(filters);

            AddSearchEngineOptimizationFilters(filters);
            AddSecurityFilters(filters);
            AddContentSecurityPolicyFilters(filters);
        }

        private static void AddDefaultFilters(GlobalFilterCollection filters)
        {
            // Add DefaultsFilter which initializes BaseUri and Configuration on ViewBag for use in Views
            filters.Add(new DefaultsFilter(DependencyResolver.Current.GetService<IConfigurationService>()));
        }

        /// <summary>
        /// Adds filters which help improve search engine optimization (SEO).
        /// </summary>
        private static void AddSearchEngineOptimizationFilters(GlobalFilterCollection filters)
        {
            filters.Add(new RedirectToCanonicalUrlAttribute(
                RouteTable.Routes.AppendTrailingSlash,
                RouteTable.Routes.LowercaseUrls));
        }

        /// <summary>
        /// Several NWebsec Security Filters are added here. 
        /// (See <see cref="http://rehansaeed.com/nwebsec-asp-net-mvc-security-through-http-headers/"/>, 
        /// <see cref="http://www.dotnetnoob.com/2012/09/security-through-http-response-headers.html"/> and 
        /// <see cref="https://github.com/NWebsec/NWebsec/wiki"/> for more information).
        /// </summary>
        private static void AddSecurityFilters(GlobalFilterCollection filters)
        {
            // Require HTTPS to be used across the whole site. System.Web.Mvc.RequireHttpsAttribute performs a 
            // 302 Temporary redirect from a HTTP URL to a HTTPS URL. This filter gives you the option to perform a 
            // 301 Permanent redirect or a 302 temporary redirect.
            // filters.Add(new RedirectToHttpsAttribute(true));

            // X-Content-Type-Options - Adds the X-Content-Type-Options HTTP header. Stop IE9 and below from sniffing 
            //                          files and overriding the Content-Type header (MIME type).
            filters.Add(new XContentTypeOptionsAttribute());

            // X-Download-Options - Adds the X-Download-Options HTTP header. When users save the page, stops them from 
            //                      opening it and forces a save and manual open.
            filters.Add(new XDownloadOptionsAttribute());

            // X-Frame-Options - Adds the X-Frame-Options HTTP header. Stop clickjacking by stopping the page from 
            //                   opening in an iframe or only allowing it from the same origin.  
            filters.Add(
                new XFrameOptionsAttribute()
                {
                    Policy = XFrameOptionsPolicy.Deny
                });
        }

        /// <summary>
        /// Adds the Content-Security-Policy (CSP) and/or Content-Security-Policy-Report-Only HTTP headers. This 
        /// creates a white-list from where various content in a web page can be loaded from. (See
        /// <see cref="http://rehansaeed.com/content-security-policy-for-asp-net-mvc/"/> and
        /// <see cref="https://developer.mozilla.org/en-US/docs/Web/Security/CSP/CSP_policy_directives"/> 
        /// <see cref="https://github.com/NWebsec/NWebsec/wiki"/> and for more information).
        /// </summary>
        private static void AddContentSecurityPolicyFilters(GlobalFilterCollection filters)
        {
            // Content-Security-Policy - Add the Content-Security-Policy HTTP header to enable Content-Security-Policy.
            filters.Add(new CspAttribute());

            // Enables logging of CSP violations.
            filters.Add(new CspReportUriAttribute() { EnableBuiltinHandler = true });

            // default-src - Sets a default source list for a number of directives. If the other directives below are 
            //               not used then this is the default setting.
            filters.Add(
                new CspDefaultSrcAttribute()
                {
                    // Disallow everything by default.
                    None = true
                });


            // base-uri - This directive restricts the document base URL 
            //            (See http://www.w3.org/TR/html5/infrastructure.html#document-base-url).
            filters.Add(
                new CspBaseUriAttribute()
                {
                    // Allow base URL's from the same domain.
                    Self = true
                });
            // child-src - This directive restricts from where the protected resource can load web workers or embed 
            //             frames. This was introduced in CSP 2.0 to replace frame-src. frame-src should still be used 
            //             for older browsers.
            filters.Add(
                new CspChildSrcAttribute()
                {
                    // Allow web workers or embed frames from the same domain.
                    Self = false
                });
            // connect-src - This directive restricts which URIs the protected resource can load using script interfaces 
            //               (Ajax Calls and Web Sockets).
            filters.Add(
                new CspConnectSrcAttribute()
                {
#if DEBUG
                    // Allow Browser Link to work in debug mode only.
                    CustomSources = string.Join(" ", "localhost:*", "ws://localhost:*"),
#else
                    // Allow AJAX and Web Sockets to example.com.
                    CustomSources = string.Join(" ", ContentDeliveryNetwork.Other.OwnSTS, ContentDeliveryNetwork.Other.OwnSite),
#endif
                    // Allow all AJAX and Web Sockets calls from the same domain.
                    Self = true
                });
            // font-src - This directive restricts from where the protected resource can load fonts.
            filters.Add(
                new CspFontSrcAttribute()
                {
                    // Allow fonts from maxcdn.bootstrapcdn.com and google.
                    CustomSources = string.Join(
                        " ",
                        ContentDeliveryNetwork.MaxCdn.Domain,
                        ContentDeliveryNetwork.Google.FontDomain),
                    // Allow all fonts from the same domain.
                    Self = true
                });
            // form-action - This directive restricts which URLs can be used as the action of HTML form elements.
            filters.Add(
                new CspFormActionAttribute()
                {
                    // Allow forms to post back to the same domain.
                    Self = true
                });
            // frame-src - This directive restricts from where the protected resource can embed frames.
            //             This is now deprecated in favour of child-src but should still be used for older browsers.
            filters.Add(
                new CspFrameSrcAttribute()
                {
                    // Allow iFrames from the same domain.
                    Self = false
                });
            // frame-ancestors - This directive restricts from where the protected resource can embed frame, iframe, 
            //                   object, embed or applet's.
            filters.Add(
                new CspFrameAncestorsAttribute()
                {
                    // Allow frame, iframe, object, embed or applet's from the same domain.
                    Self = false
                });
            // img-src - This directive restricts from where the protected resource can load images.
            filters.Add(
                new CspImgSrcAttribute()
                {
                    CustomSources = "*",
                    // Allow images from the same domain.
                    Self = true,
                });
            // script-src - This directive restricts which scripts the protected resource can execute. 
            //              The directive also controls other resources, such as XSLT style sheets, which can cause the 
            //              user agent to execute script.
            filters.Add(
                new CspScriptSrcAttribute()
                {
                    // Allow scripts from the bundle CDN's.
                    CustomSources = string.Join(
                        " ",
#if DEBUG
                        // Allow Browser Link to work in debug mode only.
                        "localhost:*",
#endif
                        ContentDeliveryNetwork.Google.Domain,
                        ContentDeliveryNetwork.Google.GStatic,
                        ContentDeliveryNetwork.Google.TagManagerDomain,
                        ContentDeliveryNetwork.Google.GoogleAnalyticsDomain,
                        ContentDeliveryNetwork.Google.ApisDomain,
                        ContentDeliveryNetwork.Microsoft.Domain,
                        ContentDeliveryNetwork.Cloudflare.Domain),
                    // Allow scripts from the same domain.
                    Self = true,
                    // Allow the use of the eval() method to create code from strings. This is unsafe and can open your 
                    // site up to XSS vulnerabilities.
                    UnsafeEval = true
                });
            // media-src - This directive restricts from where the protected resource can load video and audio.
            filters.Add(
                new CspMediaSrcAttribute()
                {
                    // Allow audio and video from the same domain.
                    Self = false
                });
            // object-src - This directive restricts from where the protected resource can load plug-ins.
            filters.Add(
                new CspObjectSrcAttribute()
                {
                    // Allow plug-ins from the same domain.
                    Self = false
                });
            // style-src - This directive restricts which styles the user applies to the protected resource.
            filters.Add(
                new CspStyleSrcAttribute()
                {
                    // Allow CSS from maxcdn.bootstrapcdn.com and goole.
                    CustomSources = string.Join(
                        " ",
                        ContentDeliveryNetwork.MaxCdn.Domain,
                        ContentDeliveryNetwork.Google.FontDomain),
                    // Allow CSS from the same domain.
                    Self = true,
                    // Allow in-line CSS, this is unsafe and can open your site up to XSS vulnerabilities.
                    // Note: This is currently enable because Modernizr does not support CSP and includes in-line styles
                    // in its JavaScript files. This is a security hold. If you don't want to use Modernizr, be sure to 
                    // disable unsafe in-line styles. For more information See:
                    // http://stackoverflow.com/questions/26532234/modernizr-causes-content-security-policy-csp-violation-errors
                    // https://github.com/Modernizr/Modernizr/pull/1263
                    UnsafeInline = true
                });
        }
    }
}
