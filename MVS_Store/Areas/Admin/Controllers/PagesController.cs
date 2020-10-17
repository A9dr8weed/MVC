using MVS_Store.Models.Data;
using MVS_Store.Models.ViewModels.Pages;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MVS_Store.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            // об'являємо список для представлення (PageViewModel)
            List<PageViewModel> pageList;

            // ініціалізація списку (DB)
            using (DB db = new DB())
            {
                pageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageViewModel(x)).ToList();
                // змінній присвоюємо об'єкти з бази даних в масив, відсортовуємо по полю, вибираємо всі об'єкти, і додаємо в відсортований список
            }

            // повертаємо список в представлення
            return View(pageList);
        }

        // GET: Admin/Pages/AddPage
        [HttpGet] // отримати
        public ActionResult AddPage()
        {
            return View();
        }

        // POST: Admin/Pages/AddPage
        [HttpPost] // отримати і обробити дані
        public ActionResult AddPage(PageViewModel model)
        {
            // перевірка моделі на валідність
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (DB Db = new DB())
            {

                // об'являємо змінну для короткого опису (Slug)
                string slug;

                // ініціалізаці класу PageDTO
                PagesDTO dto = new PagesDTO();

                // присвоєння заголовка моделі
                dto.Title = model.Title.ToUpper(); // заголовки з великої букви

                // перевірка чи є короткий опис, якщо немає, то присвоюємо
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                // перевірка на унікальність заголовку і короткого опису
                if (Db.Pages.Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That Title Already Exist.");
                    return View(model); //вертаємо користувачу повторно ввід, щоб не втратити дані
                }
                else if (Db.Pages.Any(x => x.Slug == model.Slug))
                {
                    ModelState.AddModelError("", "That Slug Already Exist.");
                    return View(model);
                }

                // присвоюємо значення моделі
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;

                // зберігаємо модель в БД
                Db.Pages.Add(dto);
                Db.SaveChanges();
            }

            // передаємо повідомлення через TempData
            TempData["SM"] = "You Has Added A New Pages";

            // переадресація користувача на метод INDEX
            return RedirectToAction("Index");
        }

        // GET: Admin/Pages/EditPage//id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            // об'явлення моделі PageViewModel
            PageViewModel model;

            using (DB db = new DB()) // конструкція відкриває підключення до бази даних
            {
                // отримуємо дані сторінки
                PagesDTO dto = db.Pages.Find(id);

                // перевірка, чи доступна сторінка
                if (dto == null)
                {
                    return Content("The Page Does Not Exist.");
                }

                // ініціалізація моделі даними
                model = new PageViewModel(dto);
            }

            // повертаємо модель в представлення
            return View(model);
        }

        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult EditPage(PageViewModel model)
        {
            // перевірка моделі на валідність
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (DB db = new DB())
            {
                // отримуємо ID сторінки
                int id = model.ID;

                // оголошення змінної для Slug
                string slug = "home";

                // отримуємо сторінку по ID
                PagesDTO dto = db.Pages.Find(id);

                // присвоєння  назви з отриманої моделі в DTO
                dto.Title = model.Title;

                // перевірка Slug і присвоєння, якщо необхідно
                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }

                // перевірка Slug і Title на унікальність
                if (db.Pages.Where(x => x.ID != id).Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That Title Already Exist.");
                    return View(model);
                }
                else if (db.Pages.Where(x => x.ID != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "That Slug Already Exist.");
                    return View(model);
                }

                // запис значень, що залишилися в DTO
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;

                // збереження змін в БД
                db.SaveChanges();
            }

            // повідомлення користувачу про успішне збереження (Tempdata)
            TempData["SM"] = "You Have Edited The Page.";

            // переадресація користувача на сторінку яку він редагував
            return RedirectToAction("EditPage");
        }

        // GET: Admin/Pages/PageDetails/id
        [HttpGet]
        public ActionResult PageDetails(int id)
        {
            // оголошення модель PageViewModel 
            PageViewModel model;

            using (DB db = new DB())
            {
                // отримуємо сторінку
                PagesDTO dto = db.Pages.Find(id);

                // перевіряємо чи сторінка доступна
                if (dto == null)
                {
                    return Content("The Page Does Not Exist");
                }

                // присвоюємо моделі інформацію з БД
                model = new PageViewModel(dto);
            }

            // повертаємо модель в представлення
            return View(model);
        }

        // GET: Admin/Pages/DeletePage/id
        [HttpGet]
        public ActionResult DeletePage(int id)
        {
            using (DB db = new DB())
            {
                // отримуємо сторінку
                PagesDTO dto = db.Pages.Find(id);

                // видалення сторінки
                db.Pages.Remove(dto);

                // зберігаємо зміни в базі
                db.SaveChanges();
            }

            // додаємо повідомлення про успішне видалення сторінки
            TempData["SM"] = "You Have Deleted A Page!";

            // переадресація на сторінку Index
            return RedirectToAction("Index");
        }

        // POST: Admin/Pages/ReorderPages
        [HttpPost]
        public void ReorderPages(int [] id)
        {
            using (DB db = new DB())
            {
                // реалізуємо лічильник
                int count = 1;

                // ініціалізація моделі даних
                PagesDTO dto;

                // встановлюємо сортування для кожної сторінки
                foreach(var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;

                    db.SaveChanges();

                    count++;
                }
            }
        }

        // GET: Admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar()
        {
            // ініціалізація моделі
            SidebarViewModel model;
            
            using (DB db = new DB())
            {
                // отримуємо дані з DTO
                SidebarDTO dto = db.Sidebars.Find(1); // жосткі значення в коді не бажано добавляти

                // заповнюємо модель даними
                model = new SidebarViewModel(dto);
            }
            
            // повернути представлення з моделлю
            return View(model);
        }

        // POST: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarViewModel model)
        {
            using (DB db = new DB())
            {
                // отримати дані з DTO
                SidebarDTO dto = db.Sidebars.Find(1); // жосткі значення не допустимі

                // присоєння даних в Body
                dto.Body = model.Body;

                // зберігаємо
                db.SaveChanges();
            }

            // повідомлення в TempData
            TempData["SM"] = "You Have Edited Sidebar!";

            // переадресація користувача
            return RedirectToAction("EditSidebar");
        }
    }
}