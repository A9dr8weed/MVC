using MVS_Store.Models.Data;
using MVS_Store.Models.ViewModels.Shop;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using PagedList;
// ReSharper disable All

namespace MVS_Store.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop
        [HttpGet]
        public ActionResult Categories()
        {
            // оголошення моделі типу List
            List<CategoryViewModel> categoryViewModelList;

            using (DB db = new DB())
            {
                // ініціалізуємо модель даними
                categoryViewModelList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryViewModel(x)).ToList();
            }

            // повертаємо List в представлення
            return View(categoryViewModelList);
        }

        // POST: Admin/Shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            // оголошення string змінну ID
            string id;

            using (DB db = new DB())
            {
                // перевірка імені категорії на унікальність
                if (db.Categories.Any(x => x.Name == catName))
                {
                    return "titletaken";
                }

                // ініціалізація моделі DTO
                CategoryDTO dto = new CategoryDTO();

                // заповнення даними моделі
                dto.Name = catName;
                dto.Slug = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;

                // зберігаємо
                db.Categories.Add(dto);
                db.SaveChanges();

                // отримуємо ID, щоб повернути його в представлення
                id = dto.ID.ToString();
            }

            // повертаємо ID в представлення
            return id;
        }

        // POST: Admin/Shop/ReorderCategories
        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            using (DB db = new DB())
            {
                // реалізуємо лічильник
                int count = 1;

                // ініціалізація моделі даних
                CategoryDTO dto;

                // встановлюємо сортування для кожної сторінки
                foreach (var catId in id)
                {
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;

                    db.SaveChanges();

                    count++;
                }
            }
        }

        // GET: Admin/Shop/DeleteCategory/id
        [HttpGet]
        public ActionResult DeleteCategory(int id)
        {
            using (DB db = new DB())
            {
                // отримуємо модель категорії
                CategoryDTO dto = db.Categories.Find(id);

                // видалення категорію
                db.Categories.Remove(dto);

                // зберігаємо зміни в базі
                db.SaveChanges();
            }

            // додаємо повідомлення про успішне видалення сторінки
            TempData["SM"] = "You Have Deleted A Category!";

            // переадресація на сторінку Index
            return RedirectToAction("Categories");
        }

        // POST: Admin/Shop/RenameCategory/id
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            using (DB db = new DB())
            {
                // перевірка на унікальність
                if (db.Categories.Any(x => x.Name == newCatName))
                {
                    return "titletaken";
                }

                // отримуємо модель DTO
                CategoryDTO dto = db.Categories.Find(id);

                // редагуємо модель DTO
                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();

                // зберігаємо зміни
                db.SaveChanges();
            }

            // повертаємо слово
            return "OK";
        }

        // GET: Admin/Shop/AddProducts
        [HttpGet]
        public ActionResult AddProduct()
        {
            // оголошення моделі даних
            ProductViewModel model = new ProductViewModel();

            using (DB db = new DB())
            {
                // додаємо в модель категорії з бази в модель
                model.Categories = new SelectList(db.Categories.ToList(), "id", "Name");
            }

            // повертаємо модель в представлення
            return View(model);
        }

        // POST: Admin/Shop/AddProducts
        [HttpPost]
        public ActionResult AddProduct(ProductViewModel model, HttpPostedFileBase file)
        {
            // перевірка моделі на валідність
            if (!ModelState.IsValid)
            {
                using (DB db = new DB())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    return View(model);
                }
            }

            // перевірка імені продукта на унікальність
            using (DB db = new DB())
            {
                if (db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "That Product Name Is Taken");
                    return View(model);
                }
            }

            // оголошення змінної ProductID
            int id;

            // Ініціалізуємо і зберігаємо в базу модель на основі ProductDTO
            using (DB db = new DB())
            {
                ProductDTO product = new ProductDTO();

                product.Name = model.Name;
                product.Slug = model.Name.Replace(" ", "-").ToLower();
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryID = model.CategoryID;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.ID == model.CategoryID);
                product.CategoryName = catDTO.Name;

                db.Products.Add(product);
                db.SaveChanges();

                id = product.ID;
            }

            // додаємо повідомлення користувачу в TempData
            TempData["SM"] = "You Have Added A Product!";

            #region UploadImage

            // створюємо необхідні ссилки директорій
            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            // перевіряємо наявність директорій (якщо немає, то створюємо)
            if (!Directory.Exists(pathString1))
            {
                Directory.CreateDirectory(pathString1);
            }
            if (!Directory.Exists(pathString2))
            {
                Directory.CreateDirectory(pathString2);
            }
            if (!Directory.Exists(pathString3))
            {
                Directory.CreateDirectory(pathString3);
            }
            if (!Directory.Exists(pathString4))
            {
                Directory.CreateDirectory(pathString4);
            }
            if (!Directory.Exists(pathString5))
            {
                Directory.CreateDirectory(pathString5);
            }

            // перевіряємо чи був завантажений такий файл
            if (file != null && file.ContentLength > 0)
            {
                // отримуємо розширення файла
                string ext = file.ContentType.ToLower();

                // перевіряємо розширення файлу
                if (ext != "image/jpg" && ext != "image/jpeg" && ext != "image/pjpeg" && ext != "image/gif" &&
                    ext != "image/x-png" && ext != "image/png")
                {
                    using (DB db = new DB())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "The Image Wasn`t Uploaded - Wrong Inage Extension");
                        return View(model);
                    }
                }

                // оголошення змінної з іменем зображення
                string imageName = file.FileName;

                // зберігаємо ім'я зображення в модель DTO
                using (DB db = new DB())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }

                // назначаємо шлях до оригінального і зменшеного зображення
                var path = string.Format($"{pathString2}\\{imageName}");
                var path2 = string.Format($"{pathString3}\\{imageName}");

                // зберігаємо оригінальне зображення
                file.SaveAs(path);

                // створюємо і зберігаємо зменшену копію
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200).Crop(1, 1);
                img.Save(path2);
            }

            #endregion

            // переадресація користувача 
            return RedirectToAction("AddProduct");
        }

        // GET: Admin/Shop/Products
        [HttpGet]
        public ActionResult Products(int? page, int? catId)
        {
            // оголошення ProductViewModel типу List
            List<ProductViewModel> listOfProductViewModels;

            // встановлюємо номер сторінки
            var pageNumber = page ?? 1; // якщо повернеться null то автоматично вернеться 1

            using (DB db = new DB())
            {
                // ініціалізуємо List і заповнюємо даними
                listOfProductViewModels = db.Products.ToArray()
                    .Where(x => catId == null || catId == 0 || x.CategoryID == catId)
                    .Select(x => new ProductViewModel(x)).ToList();

                // заповнюємо категорії даними
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                // встановлюємо вибрану категорію
                ViewBag.SelectedCat = catId.ToString();
            }
            
            // встановлюємо посторінкову навігацію
            var onePageOfProducts = listOfProductViewModels.ToPagedList(pageNumber, 3);
            ViewBag.onePageOfProducts = onePageOfProducts;

            // повертаємо в представлення
            return View(listOfProductViewModels);
        }

        // GET: Admin/Shop/Product/id
        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            // оголошення моделі ProductViewModel
            ProductViewModel model;

            using (DB db = new DB())
            {
                // отримуємо продукт
                ProductDTO dto = db.Products.Find(id);

                // перевірка, чи доступний продукт
                if (dto == null)
                {
                    return Content("That Product Isn`t Exist.");
                }

                // ініціалізуємо модель даними
                model = new ProductViewModel(dto);

                // створюємо список категорій
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                // отримуємо всі зображення з галереї
                model.GalleryImages = Directory
                    .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                    .Select(fn => Path.GetFileName(fn));
            }

            // повертаємо модель в представлення
            return View(model);
        }

        // POST: Admin/Shop/EditProduct
        [HttpPost]
        public ActionResult EditProduct(ProductViewModel model, HttpPostedFileBase file)
        {
            // отримуємо id продукта
            int id = model.ID;

            // заповнення списку категоріями і зображеннями
            using (DB db = new DB())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }

            model.GalleryImages = Directory
                .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                .Select(fn => Path.GetFileName(fn));

            // перевірка моделі на валідність
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // перевірка імені продукту на унікальність
            using (DB db = new DB())
            {
                if (db.Products.Where(x => x.ID != id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "That Product Name Is Taken!");
                    return View(model);
                }
            }

            // оновлюємо продукт в БД
            using (DB db = new DB())
            {
                ProductDTO dto = db.Products.Find(id);

                dto.Name = model.Name;
                dto.Slug = model.Name.Replace(" ", "-").ToLower();
                dto.Description = model.Description;
                dto.Price = model.Price;
                dto.CategoryID = model.CategoryID;
                dto.ImageName = model.ImageName;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.ID == model.CategoryID);
                dto.CategoryName = catDTO.Name;

                db.SaveChanges();
            }

            // встановлюємо повідомлення в TempData
            TempData["SM"] = "You Have Edited The Product";

            #region Image Upload

            // перевірка загрузки файлу
            if (file != null && file.ContentLength > 0)
            {
                // отримуємо розширення файлу
                string ext = file.ContentType.ToLower();

                // перевіряємо розширення
                if (ext != "image/jpg" && ext != "image/jpeg" && ext != "image/pjpeg" && ext != "image/gif" &&
                    ext != "image/x-png" && ext != "image/png")
                {
                    using (DB db = new DB())
                    {
                        ModelState.AddModelError("", "The Image Wasn`t Uploaded - Wrong Inage Extension");
                        return View(model);
                    }
                }

                // встановлюємо шляхи для завантаження
                var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

                var pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
                var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");

                // видалення існуючих файлів і директорій
                DirectoryInfo di1 = new DirectoryInfo(pathString1);
                DirectoryInfo di2 = new DirectoryInfo(pathString2);

                foreach (var file2 in di1.GetFiles())
                {
                    file2.Delete();
                }

                foreach (var file3 in di2.GetFiles())
                {
                    file3.Delete();
                }

                // зберігаємо ім'я зображення
                string imageName = file.FileName;

                using (DB db = new DB())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }

                // зберігаємо оригінал і прев'ю версії
                var path = string.Format($"{pathString1}\\{imageName}");
                var path2 = string.Format($"{pathString2}\\{imageName}");

                file.SaveAs(path);

                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200).Crop(1, 1);
                img.Save(path2);
            }

            #endregion

            // переадресація користувача
            return RedirectToAction("EditProduct");
        }

        // POST: Admin/Shop/DeleteProduct/id
        [HttpPost]
        public ActionResult DeleteProduct(int id)
        {
            // видалення товару з БД
            using (DB db = new DB())
            {
                ProductDTO dto = db.Products.Find(id);
                db.Products.Remove(dto);

                db.SaveChanges();
            }

            // видалення директорій товару (зображення)
            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
            var pathString = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());

            if (Directory.Exists(pathString))
            {
                Directory.Delete(pathString, true);
            }

            // переадресація користувача
            return RedirectToAction("Products");
        }

        // POST: Admin/Shop/SaveGalleryImages/id
        [HttpPost]
        public void SaveGalleryImages(int id)
        {
            // перебір всіх отриманих файлів
            foreach (string fileName in Request.Files)
            {
                // ініціалізуємо файли
                HttpPostedFileBase file = Request.Files[fileName];

                // перевірка на null
                if (file != null && file.ContentLength > 0)
                {
                    // назначаємо шляхи до директорій
                    var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

                    string pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
                    string pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

                    // назначаємо шляхи зображень
                    var path = string.Format($"{pathString1}\\{file.FileName}");
                    var path2 = string.Format($"{pathString2}\\{file.FileName}");

                    // зберігаємо оригінальні і зменшені копії
                    file.SaveAs(path);

                    WebImage img = new WebImage(file.InputStream);
                    img.Resize(200, 200).Crop(1, 1);
                    img.Save(path2);
                }
            }
        }

        // POST: Admin/Shop/DeleteImage/id, imageName
        [HttpPost]
        public void DeleteImage(int id, string imageName)
        {
            string fullPath1 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/" + imageName);
            string fullPath2 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/Thumbs/" + imageName);

            if (System.IO.File.Exists(fullPath1))
            {
                System.IO.File.Delete(fullPath1);
            }

            if (System.IO.File.Exists(fullPath2))
            {
                System.IO.File.Delete(fullPath2);
            }
        }
    }
}