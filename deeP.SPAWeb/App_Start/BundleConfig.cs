namespace deeP.SPAWeb
{
    using System.Web.Optimization;
    using deeP.SPAWeb.Constants;

    public static class BundleConfig
    {
        /// <summary>
        /// For more information on bundling, visit <see cref="http://go.microsoft.com/fwlink/?LinkId=301862"/>.
        /// </summary>
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Enable CDN usage. 
            // We are using Google's CDN where possible and then Microsoft if not available for better 
            // performance (Google is more likely to have been cached by the users browser).
            // Protocol (http:) is omitted from the CDN URL on purpose to allow the browser to choose the protocol.
            bundles.UseCdn = true;

            AddCss(bundles);
            AddJavaScript(bundles);
        }

        private static void AddCss(BundleCollection bundles)
        {
            // Bootstrap - Twitter Bootstrap CSS (http://getbootstrap.com/).
            // TODO: configure CDN
            bundles.Add(new StyleBundle(
                "~/Content/css")
                .Include("~/Content/bootstrap/site.css")
                .Include("~/Content/site.css"));

            // Font Awesome - Icons using font (http://fortawesome.github.io/Font-Awesome/).
            bundles.Add(new StyleBundle(
                "~/Content/fa",
                ContentDeliveryNetwork.MaxCdn.FontAwesomeUrl)
                .Include("~/Content/fontawesome/site.css"));

            // Angular related CSS files
            // TODO: configure CDN
            bundles.Add(new StyleBundle("~/Content/angular-css").Include(
                "~/Content/angular-csp.css",
                "~/Content/angular-block-ui.css",
                "~/Content/ng-sortable.css"));
        }

        private static void AddJavaScript(BundleCollection bundles)
        {
            // jQuery - The JavaScript helper API (http://jquery.com/).
            Bundle jqueryBundle = new ScriptBundle("~/bundles/jquery", ContentDeliveryNetwork.Google.JQueryUrl)
                .Include("~/Scripts/jquery-{version}.js");
            bundles.Add(jqueryBundle);

            // jQuery Validate - Client side JavaScript form validation (http://jqueryvalidation.org/).
            Bundle jqueryValidateBundle = new ScriptBundle(
                "~/bundles/jqueryval", 
                ContentDeliveryNetwork.Microsoft.JQueryValidateUrl)
                .Include("~/Scripts/jquery.validate*");
            bundles.Add(jqueryValidateBundle);

            // Microsoft jQuery Validate Unobtrusive - Validation using HTML data- attributes 
            // http://stackoverflow.com/questions/11534910/what-is-jquery-unobtrusive-validation
            Bundle jqueryValidateUnobtrusiveBundle = new ScriptBundle(
                "~/bundles/jqueryvalunobtrusive", 
                ContentDeliveryNetwork.Microsoft.JQueryValidateUnobtrusiveUrl)
                .Include("~/Scripts/jquery.validate*");
            bundles.Add(jqueryValidateUnobtrusiveBundle);

            // Modernizr - Allows you to check if a particular API is available in the browser (http://modernizr.com).
            // Note: The current version of Modernizr does not support Content Security Policy (CSP) (See FilterConfig).
            // See here for details: https://github.com/Modernizr/Modernizr/pull/1263 and 
            // http://stackoverflow.com/questions/26532234/modernizr-causes-content-security-policy-csp-violation-errors
            Bundle modernizrBundle = new ScriptBundle(
                "~/bundles/modernizr", 
                ContentDeliveryNetwork.Microsoft.ModernizrUrl)
                .Include("~/Scripts/modernizr-*");
            bundles.Add(modernizrBundle);

            // Bootstrap - Twitter Bootstrap JavaScript (http://getbootstrap.com/).
            Bundle bootstrapBundle = new ScriptBundle(
                "~/bundles/bootstrap", 
                ContentDeliveryNetwork.Microsoft.BootstrapUrl)
                .Include("~/Scripts/bootstrap.js");
            bundles.Add(bootstrapBundle);

            // Moment JS
            Bundle momentBundle = new ScriptBundle("~/bundles/moment", ContentDeliveryNetwork.Cloudflare.MomentJSUrl)
                .Include("~/Scripts/moment.js");
            bundles.Add(momentBundle);

            // Lodash
            Bundle lodashBundle = new ScriptBundle("~/bundles/lodash", ContentDeliveryNetwork.Cloudflare.LodashUrl)
            .Include("~/Scripts/lodash.js");
            bundles.Add(lodashBundle);

            // FastClick
            Bundle fastClickBundle = new ScriptBundle("~/bundles/fastclick", ContentDeliveryNetwork.Cloudflare.FastClickUrl)
                .Include("~/Scripts/fastclick.js");
            bundles.Add(fastClickBundle);
            // TODO: configure CDN
            Bundle statefulFastClickBundle = new ScriptBundle("~/bundles/stateful-fastclick")
                .Include("~/Scripts/stateful-fastclick.js");
            bundles.Add(statefulFastClickBundle);

            // Angular JS and modules
            Bundle angularBundle = new ScriptBundle("~/bundles/angular", ContentDeliveryNetwork.Google.AngularJSUrl)
                .Include("~/Scripts/angular.js");
            bundles.Add(angularBundle);
            Bundle angularRouteBundle = new ScriptBundle("~/bundles/angular-route", ContentDeliveryNetwork.Google.AngularJSRouteUrl)
                .Include("~/Scripts/angular-route.js");
            bundles.Add(angularRouteBundle);
            Bundle angularTouchBundle = new ScriptBundle("~/bundles/angular-touch", ContentDeliveryNetwork.Google.AngularJSTouchUrl)
              .Include("~/Scripts/angular-touch.js");
            bundles.Add(angularTouchBundle);
            // TODO: configure CDN
            Bundle angularStatefulFastClickBundle = new ScriptBundle("~/bundles/angular-stateful-fastclick")
                .Include("~/Scripts/angular-stateful-fastclick.js");
            bundles.Add(angularStatefulFastClickBundle);
            Bundle angularUIBootstrapBundle = new ScriptBundle("~/bundles/angular-ui-bootstrap", ContentDeliveryNetwork.Cloudflare.AngularJSUIBootstrapUrl)
                .Include("~/Scripts/angular-ui/ui-bootstrap-tpls.js");
            bundles.Add(angularUIBootstrapBundle);
            // TODO: configure CDN
            Bundle angularUUIDBundle = new ScriptBundle("~/bundles/angular-uuid")
                .Include("~/Scripts/angular-uuid2.js");
            bundles.Add(angularUUIDBundle);
            Bundle angularMomentBundle = new ScriptBundle("~/bundles/angular-moment", ContentDeliveryNetwork.Cloudflare.AngularMomentUrl)
                .Include("~/Scripts/angular-moment.js");
            bundles.Add(angularMomentBundle);
            // TODO: configure CDN
            Bundle angularSimpleLoggerBundle = new ScriptBundle("~/bundles/angular-simple-logger")
                .Include("~/Scripts/angular-simple-logger.light.js");
            bundles.Add(angularSimpleLoggerBundle);
            Bundle angularGoogleMapsBundle = new ScriptBundle("~/bundles/angular-google-maps", ContentDeliveryNetwork.Cloudflare.AngularJSGoogleMapsUrl)
                .Include("~/Scripts/angular-google-maps.js");
            bundles.Add(angularGoogleMapsBundle);
            // TODO: configure CDN
            Bundle angularBlockUIBundle = new ScriptBundle("~/bundles/angular-block-ui")
                .Include("~/Scripts/angular-block-ui.js");
            bundles.Add(angularBlockUIBundle);
            // TODO: configure CDN
            Bundle angularChronicleBundle = new ScriptBundle("~/bundles/angular-chronicle")
                .Include("~/Scripts/chronicle.js");
            bundles.Add(angularChronicleBundle);
            // TODO: configure CDN
            Bundle angularSortableBundle = new ScriptBundle("~/bundles/ng-sortable")
                .Include("~/Scripts/ng-sortable.js");
            bundles.Add(angularSortableBundle);
            // TODO: configure CDN
            Bundle angularFileUploadBundle = new ScriptBundle("~/bundles/angular-file-upload")
                .Include("~/Scripts/angular-file-upload.min.js");
            bundles.Add(angularFileUploadBundle);

            // Bundles for the site's own Angular artifacts
            // TODO: configure CDN
            Bundle deePInitBundle = new ScriptBundle("~/bundles/deePInit")
                .Include("~/Scripts/deeP/deePInit.js");
            bundles.Add(deePInitBundle);

            // TODO: configure CDN
            Bundle deePCoreBundle = new ScriptBundle("~/bundles/deePApp")
                .Include("~/Scripts/deeP/app-*");
            bundles.Add(deePCoreBundle);

            // Script bundle for the site. The fall-back scripts are for when a CDN fails, in this case we load a local
            // copy of the script instead.
            Bundle failoverCoreBundle = new ScriptBundle("~/bundles/site")
                .Include("~/Scripts/Fallback/styles.js")
                .Include("~/Scripts/Fallback/scripts.js")
                .Include("~/Scripts/site.js");
            bundles.Add(failoverCoreBundle);
        }
    }
}
