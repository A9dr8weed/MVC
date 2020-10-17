using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MVS_Store
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(filters: GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(routes: RouteTable.Routes);
            BundleConfig.RegisterBundles(bundles: BundleTable.Bundles);
        }
    }
}
