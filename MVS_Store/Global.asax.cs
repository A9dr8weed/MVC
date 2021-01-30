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

        // метод обробки запитів автентифікації
        protected void Application_AuthenticateRequest()
        {
            // перевірка чи користуавач авторизований
            if (User == null)
            {
                return;
            }

            // отримуємо ім'я користувача
            string userName = Context.User.Identity.Name;

            // оголошуємо масив ролей
            string[] roles = null;

            using (DB db = new DB())
            {
                // заповнюємо масив ролями
                UserDTO dto = db.Users.FirstOrDefault(x => x.UserName == userName);

                if (dto == null)
                {
                    return;
                }

                roles = db.UserRoles.Where(x => x.UserID == dto.ID).Select(x => x.Role.Name).ToArray();
            }
            // створюємо об'єкт інтерфейса IPrinciple
            IIdentity userIdentity = new GenericIdentity(userName);
            IPrincipal newUserObj = new GenericPrincipal(userIdentity, roles);

            // оголошуємо і ініціалізуємо даними Context.User
            Context.User = newUserObj;
        }
    }
}