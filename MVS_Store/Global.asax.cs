using MVS_Store.Models.Data;
using System.Linq;
using System.Security.Principal;
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

        // ����� ������� ������ ��������������
        protected void Application_AuthenticateRequest()
        {
            // �������� �� ����������� �������������
            if (User == null)
            {
                return;
            }

            // �������� ��'� �����������
            string userName = Context.User.Identity.Name;

            // ��������� ����� �����
            string[] roles = null;

            using (DB db = new DB())
            {
                // ���������� ����� ������
                UserDTO dto = db.Users.FirstOrDefault(x => x.UserName == userName);

                if (dto == null)
                {
                    return;
                }

                roles = db.UserRoles.Where(x => x.UserID == dto.ID).Select(x => x.Role.Name).ToArray();
            }
            // ��������� ��'��� ���������� IPrinciple
            IIdentity userIdentity = new GenericIdentity(userName);
            IPrincipal newUserObj = new GenericPrincipal(userIdentity, roles);

            // ��������� � ���������� ������ Context.User
            Context.User = newUserObj;
        }
    }
}