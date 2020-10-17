using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MVS_Store.Models.Data;
using MVS_Store.Models.ViewModels.Pages;
// ReSharper disable All

namespace MVS_Store.Controllers
{
    public class PagesController : Controller
    {
        // GET: Index/{page}
        public ActionResult Index(string page = "")
        {
            // отримати/встановити короткий заголовок (Slug)
            if (page == "")
            {
                page = "home";
            }

            // оголошуємо модель і клас DTO(контекст даних)
            PageViewModel model;
            PagesDTO dto;
        
            // перевіряємо, чи доступна сторінка
            using (DB db = new DB())
            {
                if (!db.Pages.Any(x => x.Slug.Equals(page)))
                {
                    return RedirectToAction("Index", new { page = ""});
                }
            }
            
            // отримуємо DTO сторінки
            using (DB db = new DB())
            {
                dto = db.Pages.Where(x => x.Slug == page).FirstOrDefault();
            }

            // встановлюємо заголовок сторінки (Title)
            ViewBag.PageTitle = dto.Title;

            // перевірка бокову панель (Sidebar)
            if (dto.HasSidebar == true)
            {
                ViewBag.Sidebar = "Yes";
            }
            else
            {
                ViewBag.Sidebar = "No";
            }

            // заповнення моделі даними
            model = new PageViewModel(dto);

            // повертаємо модель в представлення
            return View(model);
        }

        public ActionResult PagesMenuPartial()
        {
            // ініціалізація списку PageViewModel
            List<PageViewModel> pageViewModelList;

            // отримуємо всі сторінки, крім home
            using (DB db = new DB())
            {
                pageViewModelList = db.Pages.ToArray().OrderBy(x => x.Sorting).Where(x => x.Slug != "home")
                    .Select(x => new PageViewModel(x)).ToList();
            }

            // повертаємо часткове представлення і список з даними
            return PartialView("_PagesMenuPartial", pageViewModelList);
        }

        public ActionResult SidebarPartial()
        {
            // оголошення моделі
            SidebarViewModel model;

            // ініціалізація моделі даними
            using (DB db = new DB())
            {
                SidebarDTO dto = db.Sidebars.Find(1);

                model = new SidebarViewModel(dto);
            }

            // повертаємо модель в часткове представлення
            return PartialView("_SidebarPartial", model);
        }
    }
}