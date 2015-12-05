namespace deeP.SPAWeb
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // Improve SEO by stopping duplicate URL's due to case differences or trailing slashes.
            routes.AppendTrailingSlash = true;
            routes.LowercaseUrls = true;

            // IgnoreRoute - Tell the routing system to ignore certain routes for better performance.
            // Ignore .axd files.
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            // Ignore everything in the Content folder.
            routes.IgnoreRoute("Content/{*pathInfo}");
            // Ignore everything in the Scripts folder.
            routes.IgnoreRoute("Scripts/{*pathInfo}");
            // Ignore the Forbidden.html file.
            routes.IgnoreRoute("Error/Forbidden.html");
            // Ignore the GatewayTimeout.html file.
            routes.IgnoreRoute("Error/GatewayTimeout.html");
            // Ignore the ServiceUnavailable.html file.
            routes.IgnoreRoute("Error/ServiceUnavailable.html");
            // Ignore the humans.txt file.
            routes.IgnoreRoute("humans.txt");

            // Enable attribute routing.
            routes.MapMvcAttributeRoutes();

            // Normal routes are not required because we are using attribute routing. So we don't need this MapRoute 
            // statement. Unfortunately, Elmah.MVC has a bug in which some 404 and 500 errors are not logged without 
            // this route in place. So we include this but look out on these pages for a fix:
            // https://github.com/alexbeletsky/elmah-mvc/issues/60
            // https://github.com/RehanSaeed/ASP.NET-MVC-Boilerplate/issues/8
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });

            // Route all remaining URLs to the Home/Index, assuming the route is actually a deep link into the SPA
            routes.MapRoute(
                name: "Remaining",
                url: "{*url}",
                defaults: new { controller = "Home", action = "Index" });
        }
    }
}
