using System.Web;
using System.Web.Mvc;

namespace MVS_Store
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(filter: new HandleErrorAttribute());
        }
    }
}
