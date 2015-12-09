namespace deeP.SPAWeb.Constants
{
    public static class ContentDeliveryNetwork
    {
        public static class Google
        {
            public const string Domain = "ajax.googleapis.com";
            public const string GStatic = "*.gstatic.com";
            public const string GoogleAnalyticsDomain = "www.google-analytics.com";
            public const string TagManagerDomain = "www.googletagmanager.com";
            
            public const string ApisDomain = "*.googleapis.com";
            public const string FontDomain = "fonts.googleapis.com fonts.gstatic.com";
            public const string JQueryUrl = "//ajax.googleapis.com/ajax/libs/jquery/2.1.4/jquery.min.js";

            public const string AngularJSUrl = "//ajax.googleapis.com/ajax/libs/angularjs/1.4.8/angular.min.js";
            public const string AngularJSRouteUrl = "//ajax.googleapis.com/ajax/libs/angularjs/1.4.8/angular-route.min.js";
            public const string AngularJSTouchUrl = "//ajax.googleapis.com/ajax/libs/angularjs/1.4.8/angular-touch.min.js";
        }

        public static class Cloudflare
        {
            public const string Domain = "cdnjs.cloudflare.com";

            public const string AngularJSUIBootstrapUrl = "//cdnjs.cloudflare.com/ajax/libs/angular-ui-bootstrap/0.14.3/ui-bootstrap-tpls.min.js";
            public const string AngularJSLocalStorageUrl = "//cdnjs.cloudflare.com/ajax/libs/angular-local-storage/0.2.2/angular-local-storage.min.js";

            public const string AngularJSGoogleMapsUrl = "//cdnjs.cloudflare.com/ajax/libs/angular-google-maps/2.2.1/angular-google-maps.min.js";

            public const string MomentJSUrl = "//cdnjs.cloudflare.com/ajax/libs/moment.js/2.10.6/moment.min.js";
            public const string AngularMomentUrl = "//cdnjs.cloudflare.com/ajax/libs/angular-moment/0.10.3/angular-moment.min.js";

            public const string LodashUrl = "//cdnjs.cloudflare.com/ajax/libs/lodash.js/3.10.1/lodash.min.js";

            public const string FastClickUrl = "//cdnjs.cloudflare.com/ajax/libs/fastclick/1.0.6/fastclick.min.js";
        }

        public static class MaxCdn
        {
            public const string Domain = "maxcdn.bootstrapcdn.com";
            public const string FontAwesomeUrl = "//maxcdn.bootstrapcdn.com/font-awesome/4.2.0/css/font-awesome.min.css";
        }

        public static class Microsoft
        {
            public const string Domain = "ajax.aspnetcdn.com";
            public const string JQueryValidateUrl = "//ajax.aspnetcdn.com/ajax/jquery.validate/1.14.0/jquery.validate.min.js";
            public const string JQueryValidateUnobtrusiveUrl = "//ajax.aspnetcdn.com/ajax/mvc/5.2.3/jquery.validate.unobtrusive.min.js";
            public const string ModernizrUrl = "//ajax.aspnetcdn.com/ajax/modernizr/modernizr-2.8.3.js";
            public const string BootstrapUrl = "//ajax.aspnetcdn.com/ajax/bootstrap/3.3.5/bootstrap.min.js";
        }

        public static class Other
        {
            public const string OwnSTS = "deep-sts.azurewebsites.net:*";
            public const string OwnSite = "deep-purple.azurewebsites.net:*";
            public const string ImageCDN = "cdn1.momondo.net";
        }
    }
}