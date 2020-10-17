using System.Web.Mvc;
using System.Web.Routing;

namespace MVS_Store
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute(url: "{resource}.axd/{*pathInfo}");

            routes.MapRoute("Account", "Account/{action}/{id}", new { controller = "Account", action = "Index", id = UrlParameter.Optional },
                new[] { "MVS_Store.Controllers" });

            routes.MapRoute("Cart", "Cart/{action}/{id}", new { controller = "Cart", action = "Index", id = UrlParameter.Optional },
                new[] { "MVS_Store.Controllers" });

            routes.MapRoute("Shop", "Shop/{action}/{name}", new { controller = "Shop", action = "Index", name = UrlParameter.Optional },
                new[] { "MVS_Store.Controllers" });

            routes.MapRoute("SidebarPartial", "Pages/SidebarPartial", new { controller = "Pages", action = "SidebarPartial" },
                new[] { "MVS_Store.Controllers" });

            routes.MapRoute("PagesMenuPartial", "Pages/PagesMenuPartial", new { controller = "Pages", action = "PagesMenuPartial" },
                new[] { "MVS_Store.Controllers" });

            routes.MapRoute("Pages", "{page}", new {controller = "Pages", action = "Index"},
                new[] {"MVS_Store.Controllers"});

            routes.MapRoute("Default", "", new { controller = "Pages", action = "Index" },
                new[] { "MVS_Store.Controllers" });
        }
    }
}