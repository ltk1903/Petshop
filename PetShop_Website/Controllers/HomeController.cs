using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetShop_Website.Models;
using System.Data.Entity;

namespace PetShop_Website.Controllers
{
    public class HomeController : Controller
    {
        private PetShopContext db = new PetShopContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Shop(string search, int? categoryId, decimal? priceFrom, decimal? priceTo)
        {
            var products = db.Products.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.ProductName.Contains(search));
            }

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryID == categoryId.Value);
            }

            if (priceFrom.HasValue)
            {
                products = products.Where(p => p.Price >= priceFrom.Value);
            }

            if (priceTo.HasValue)
            {
                products = products.Where(p => p.Price <= priceTo.Value);
            }

            ViewBag.Categories = db.Categories.ToList();

            return View(products.ToList());
        }

        public ActionResult ProductDetail(int id)
        {
            var product = db.Products
                            .Include(p => p.Category) // RẤT QUAN TRỌNG
                            .FirstOrDefault(p => p.ProductID == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }



        public ActionResult AddToCart(int id)
        {
            var product = db.Products.Find(id);
            if (product == null) return HttpNotFound();

            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
            var item = cart.FirstOrDefault(p => p.Product.ProductID == id);

            if (item != null)
            {
                if (item.Quantity < product.StockQuantity)
                {
                    item.Quantity++;
                }
                else
                {
                    TempData["Error"] = "Sản phẩm đã đạt số lượng tồn kho.";
                }
            }
            else
            {
                if (product.StockQuantity > 0)
                {
                    cart.Add(new CartItem { Product = product, Quantity = 1 });
                }
                else
                {
                    TempData["Error"] = "Sản phẩm hiện đã hết hàng.";
                }
            }

            Session["Cart"] = cart;
            return RedirectToAction("Cart");
        }


        public ActionResult Cart()
        {
            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
            return View(cart);
        }

        [HttpPost]
        public ActionResult UpdateQuantity(int productId, int quantity)
        {
            var cart = Session["Cart"] as List<CartItem>;
            var product = db.Products.Find(productId);

            if (cart != null && product != null)
            {
                var item = cart.FirstOrDefault(p => p.Product.ProductID == productId);
                if (item != null)
                {
                    if (quantity <= product.StockQuantity && quantity > 0)
                    {
                        item.Quantity = quantity;
                    }
                    else
                    {
                        TempData["Error"] = $"Số lượng yêu cầu không hợp lệ. Tồn kho hiện tại: {product.StockQuantity}";
                    }
                }
            }
            return RedirectToAction("Cart");
        }


        public ActionResult RemoveFromCart(int id)
        {
            var cart = Session["Cart"] as List<CartItem>;
            if (cart != null)
            {
                var item = cart.FirstOrDefault(p => p.Product.ProductID == id);
                if (item != null)
                {
                    cart.Remove(item);
                }
            }
            return RedirectToAction("Cart");
        }


        public ActionResult Checkout()
        {
            ViewBag.Message = "Your checkout page.";

            return View();
        }
    }
}