using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MVS_Store.Models.Data;
using MVS_Store.Models.ViewModels.Cart;

namespace MVS_Store.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            // оголошуєсо List типу CartViewModel
            var cart = Session["cart"] as List<CartViewModel> ?? new List<CartViewModel>();

            // перевіряємо чи не пуста корзина
            if (cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Your cart is empty";
                return View();
            }
            // Складаємо сумму і звписуємо в ViewBag
            decimal total = 0m; 

            foreach (var item in cart)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;

            // повертаємо List в представлення
            return View(cart);
        }

        public ActionResult CartPartial()
        {
            // оголошення моделі CartViewModel
            CartViewModel model = new CartViewModel();

            // оголошення змінної для кількості
            int qty = 0;

            // оголощення змінної для ціни
            decimal price = 0m;

            // перевірка сессії корзини
            if (Session["Cart"] != null)
            {
                // отримуємо загальну кількість і ціну
                var list = (List<CartViewModel>)Session["cart"];

                foreach (var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;
                }

                model.Quantity = qty;
                model.Price = price;
            }
            else
            {
                // або установлюємо кількість і ціну в нуль
                model.Quantity = 0;
                model.Price = 0m;
            }
            // повернути часткове предаставлення з моделю
            return PartialView("_CartPartial", model);
        }
        
        public ActionResult AddToCartPartial(int id)
        {
            // оголошення List типу CartViewModel
            List<CartViewModel> cart = Session["cart"] as List<CartViewModel> ?? new List<CartViewModel>();

            // оголошення моделі CartViewModel
            CartViewModel model = new CartViewModel();

            using (DB db = new DB())
            {
                // отримати id продукта
                ProductDTO product = db.Products.Find(id);

                // перевіряємо чи є товар в корзині чи немає
                var productInCart = cart.FirstOrDefault(x => x.ProductID == id);

                // якщо немає, то додаємо товар
                if (productInCart == null)
                {
                    cart.Add(new CartViewModel()
                    {
                        ProductID = product.ID,
                        ProductName = product.Name,
                        Quantity = 1,
                        Price = product.Price,
                        Image = product.ImageName
                    });
                }
                else // якщо є, то додаємо одиницю товару
                {
                    productInCart.Quantity++;
                }
            }
            // отримуємо загальну кількість, ціну і додаємо в модель
            
            int qty = 0;
            decimal price = 0m;
            
            foreach (var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Quantity = qty;
            model.Price = price;

            // зберігаємо стан корзини в сессію
            Session["cart"] = cart;
            
            // повернути часткове представлення з моделлю
            return PartialView("_AddToCartPartial", model);
        }

        // GET: /cart/IncrementProduct
        public JsonResult IncrementProduct(int productId)
        {
            // оголошення List cart
            List<CartViewModel> cart = Session["cart"] as List<CartViewModel>;

            using (DB db = new DB())
            {
                // отримуємо модель CartViewModel з списку
                CartViewModel model = cart.FirstOrDefault(x => x.ProductID == productId);

                // додаємо кількість
                model.Quantity++;

                // зберігаємо необхідні дані
                var result = new {qty = model.Quantity, price = model.Price};

                // повертаємо JSON відповідь з даними
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: /cart/DecrementProduct
        public ActionResult DecrementProduct(int productId)
        {
            // оголошення List cart
            List<CartViewModel> cart = Session["cart"] as List<CartViewModel>;

            using (DB db = new DB())
            {
                // отримуємо модель CartViewModel з списку
                CartViewModel model = cart.FirstOrDefault(x => x.ProductID == productId);

                // віднімаємо кількість
                if (model.Quantity > 1)
                {
                    model.Quantity--;
                }
                else
                {
                    model.Quantity = 0;
                    cart.Remove(model);
                }

                // зберігаємо необхідні дані
                var result = new { qty = model.Quantity, price = model.Price };

                // повертаємо JSON відповідь з даними
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: /cart/RemoveProduct
        public void RemoveProduct(int productId)
        {
            // оголошення List cart
            List<CartViewModel> cart = Session["cart"] as List<CartViewModel>;

            using (DB db = new DB())
            {
                // отримуємо модель CartViewModel з списку
                CartViewModel model = cart.FirstOrDefault(x => x.ProductID == productId);

                cart.Remove(model);
            }
        }
    }
}