using System.Web.Mvc;

namespace MVS_Store.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        // GET: account/create-account
        [ActionName("create-account")]
        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        // GET: Account/Login
        [HttpGet]
        public ActionResult Login()
        {
            // підтвердити, що користувач не авторизований
            string userName = User.Identity.Name;

            if (!string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("user-profile");
            }

            // повертаємо представлення
            return View();
        }
    }
}