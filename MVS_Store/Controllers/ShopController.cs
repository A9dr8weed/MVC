using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using MVS_Store.Models.Data;
using MVS_Store.Models.ViewModels.Shop;

namespace MVS_Store.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }

        public ActionResult CategoryMenuPartial()
        {
            // оголошення моделі типу List<> CategoryViewModel
            List<CategoryViewModel> categoryViewModelsList;

            // ініціалізація моделі даними
            using (DB db = new DB())
            {
                categoryViewModelsList = db.Categories.ToArray().OrderBy(x => x.Sorting)
                    .Select(x => new CategoryViewModel(x)).ToList();
            }

            // повертаємо часткове представлення з моделлю
            return PartialView("_CategoryMenuPartial", categoryViewModelsList);
        }

        // GET: Shop/Category/name
        public ActionResult Category(string name)
        {
            // оголошення списку типу List
            List<ProductViewModel> productViewModelsList;

            using (DB db = new DB())
            {
                // отримуємо ID категорії
                CategoryDTO categoryDTO = db.Categories.Where(x => x.Slug == name).FirstOrDefault();

                int catId = categoryDTO.ID;

                // ініціалізуємо список даними
                productViewModelsList = db.Products.ToArray().Where(x => x.CategoryID == catId)
                    .Select(x => new ProductViewModel(x)).ToList();

                // отримуємо ім'я категорії
                var productCat = db.Products.Where(x => x.CategoryID == catId).FirstOrDefault();

                // перевірка на NULL 
                if (productCat == null)
                {
                    var catName = db.Categories.Where(x => x.Slug == name).Select(x => x.Name).FirstOrDefault();
                    ViewBag.CategoryName = catName;
                }
                else
                {
                    ViewBag.CategoryName = productCat.CategoryName;
                }
            }

            // поаертаємо представлення з моделлю
            return View(productViewModelsList);
        }

        // GET: Shop/product-details/name
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            // оголошення моделей DTO і ViewModel 
            ProductDTO dto;
            ProductViewModel model;

            // ініціалізуємо ID продукта
            int id = 0;

            using (DB db  = new DB())
            {
                // перевіряємо чи доступний продукт
                if (!db.Products.Any(x => x.Slug.Equals(name)))
                {
                    return RedirectToAction("Index", "Shop");
                }

                // ініціалізуємо модель DTO даними
                dto = db.Products.Where(x => x.Slug == name).FirstOrDefault();

                // отримуємо ID
                id = dto.ID;

                // ініціалізуємо ViewModel даними
                model = new ProductViewModel(dto);
            }
            // отримуємо зображення із галереї
            model.GalleryImages = Directory
                .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                .Select(fn => Path.GetFileName(fn));

            // повертаємо модель в представлення
            return View("ProductDetails", model);
        }
    }
}