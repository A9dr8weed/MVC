using MVS_Store.Models.Data;
using MVS_Store.Models.ViewModels.Account;
using System.CodeDom;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

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

        // POST: account/create-account
        [ActionName("create-account")]
        [HttpPost]
        public ActionResult CreateAccount(UserViewModel model)
        {
            // перевірка на валідність
            if (!ModelState.IsValid)
            {
                return View("CreateAccount", model);
            }

            // перевірка відповідності пароля
            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Password do not match!");
                return View("CreateAccount", model);
            }

            using (DB db = new DB())
            {
                // перевірка імені на унікальність
                if (db.Users.Any(x => x.UserName.Equals(model.UserName)))
                {
                    ModelState.AddModelError("", $"Username {model.UserName} is taken.");
                    model.UserName = "";
                    return View("CreateAccount", model);
                }

                // створюємо екземпляр класу UserDTO
                UserDTO userDTO = new UserDTO()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAddress = model.EmailAddress,
                    UserName = model.UserName,
                    Password = model.Password
                };

                // додаємо дані в модель
                db.Users.Add(userDTO);

                // зберігаємо дані
                db.SaveChanges();

                // додаємо роль користувачу
                int id = userDTO.ID;

                UserRoleDTO userRoleDTO = new UserRoleDTO()
                {
                    UserID = id,
                    RoleID = 2
                };

                db.UserRoles.Add(userRoleDTO);
                db.SaveChanges();
            }

            // записуємо повідомлення в TempData
            TempData["SM"] = "You are now registered and can login.";

            // переадресовуємо користувача
            return RedirectToAction("Login");
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

        // POST: Account/Login
        [HttpPost]
        public ActionResult Login(LoginUserViewModel model)
        {
            // перевірка моделі на валідність
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // перевірка користувача на валідність
            bool isValid = false;

            using (DB db = new DB())
            {
                if (db.Users.Any(x => x.UserName.Equals(model.UserName) && x.Password.Equals(model.Password)))
                {
                    isValid = true;
                }

                if (!isValid)
                {
                    ModelState.AddModelError("", "Invalid username or password");
                    return View(model);
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    return Redirect(FormsAuthentication.GetRedirectUrl(model.UserName, model.RememberMe));
                }
            }
        }

        // GET: /account/logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}