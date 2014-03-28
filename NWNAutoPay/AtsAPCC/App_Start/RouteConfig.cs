using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AtsAPCC
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");
            routes.IgnoreRoute("Content/images/{*pathInfo}");

            routes.MapRoute(
                "Error",
                "Error/{action}/{id}",
                defaults: new { controller = "Error", action = "Index", id = "" }
            );

            routes.MapRoute(
                "AutoPay",
                "AutoPayCredit/{action}/{id}",
                defaults: new { controller = "AutoPayCredit", action = "Index", id = "" }
            );

            routes.MapRoute(
                "DefaultSubArea",
                "{originalArea}/{originalSubArea}/{originalController}/{originalAction}",
                defaults: new { controller = "AutoPayCredit", action = "RedirectSubArea", originalArea = "", originalSubArea = "", originalController = "", originalAction = "" }
            );

            routes.MapRoute(
                "DefaultArea",
                "{originalArea}/{originalController}/{originalAction}",
                defaults: new { controller = "AutoPayCredit", action = "RedirectArea", originalArea = "", originalController = "", originalAction = "" }
            );

            routes.MapRoute(
                "Default",
                "{originalController}/{originalAction}",
                defaults: new { controller = "AutoPayCredit", action = "Redirect", originalController = "", originalAction = "" }
            );
        }
    }
}