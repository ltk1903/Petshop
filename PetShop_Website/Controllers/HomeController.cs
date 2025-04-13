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
                            .Include(p => p.Category) 
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
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null || cart.Count == 0)
            {
                TempData["Error"] = "Giỏ hàng trống!";
                return RedirectToAction("Cart");
            }

            var viewModel = new CheckoutViewModel
            {
                CartItems = cart,
                TotalAmount = cart.Sum(item => item.Product.Price * item.Quantity)
            };

            return View(viewModel);
        }


        [HttpPost]
        public ActionResult Checkout(CheckoutViewModel viewModel)
        {
            var cart = Session["Cart"] as List<CartItem>; 
            var user = Session["User"] as User;

            if (cart == null || cart.Count == 0)
            {
                TempData["Error"] = "Giỏ hàng trống!";
                return RedirectToAction("Cart");
            }

            if (user == null)
            {
                TempData["Error"] = "Bạn cần đăng nhập để thanh toán!";
                return RedirectToAction("Login", "Account");
            }

            decimal totalAmount = cart.Sum(item => item.Product.Price * item.Quantity);

            var order = new Order
            {
                UserID = user.UserID,
                OrderDate = DateTime.Now,
                OrderStatus = "Pending",
                TotalAmount = totalAmount,
                OrderDetails = new List<OrderDetail>()
            };

            foreach (var item in cart)
            {
                order.OrderDetails.Add(new OrderDetail
                {
                    ProductID = item.Product.ProductID,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                });

                var dbProduct = db.Products.Find(item.Product.ProductID);
                if (dbProduct != null && dbProduct.StockQuantity >= item.Quantity)
                {
                    dbProduct.StockQuantity -= item.Quantity;
                }
            }

            db.Orders.Add(order);
            db.SaveChanges();

            var payment = new Payment
            {
                OrderID = order.OrderID,
                PaymentMethod = viewModel.PaymentMethod,
                PaymentStatus = "Completed",
                PaymentDate = DateTime.Now,
                TransactionID = Guid.NewGuid().ToString()
            };

            db.Payments.Add(payment);
            db.SaveChanges();

            Session["Cart"] = null;
            TempData["Success"] = "Đặt hàng thành công!";
            return RedirectToAction("OrderSuccess");
        }



        public ActionResult OrderSuccess()
        {
            return View();
        }

    }
}