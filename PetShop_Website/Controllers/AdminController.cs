using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetShop_Website.Models;
using System.Data.Entity;
using System.IO;

namespace PetShop_Website.Controllers
{
    public class AdminController : Controller
    {
        private PetShopContext db = new PetShopContext();
        public ActionResult Dashboard()
        {
            if (Session["Admin"] == null)
                return RedirectToAction("Login", "AdminAccount");

            return View();
        }

        public ActionResult Product()
        {
            ViewBag.CategoryList = new SelectList(db.Categories.ToList(), "CategoryId", "CategoryName");
            return View();
        }

        [HttpPost]
        public ActionResult Product(Product product, HttpPostedFileBase ImageFile)
        {
            if (ImageFile != null && ImageFile.ContentLength > 0)
            {
                var fileName = Path.GetFileName(ImageFile.FileName);
                var path = Path.Combine(Server.MapPath("/Content/images/"), fileName);
                ImageFile.SaveAs(path);
                product.ImageURL = "/Content/images/" + fileName;
            }

            product.CreatedAt = DateTime.Now;

            db.Products.Add(product);
            db.SaveChanges();

            return RedirectToAction("Stock");
        }

        public ActionResult Stock()
        {
            var products = db.Products.Include(p => p.Category).ToList();
            return View(products);
        }

        public ActionResult EditProduct(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
                return HttpNotFound();

            ViewBag.Categories = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProduct(Product product, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = db.Products.AsNoTracking().FirstOrDefault(p => p.ProductID == product.ProductID);
                if (existingProduct == null)
                {
                    return HttpNotFound();
                }

                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var path = Path.Combine(Server.MapPath("/Content/images/"), fileName);
                    ImageFile.SaveAs(path);
                    product.ImageURL = "/Content/images/" + fileName;
                }
                else
                {
                    product.ImageURL = existingProduct.ImageURL;
                }

                product.CreatedAt = existingProduct.CreatedAt;
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Stock");
            }

            ViewBag.Categories = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }



        public ActionResult DeleteProduct(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
                return HttpNotFound();

            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Stock");
        }

        public ActionResult Order()
        {
            var orders = db.Orders.Include(o => o.User).Include(o => o.OrderDetails).Include(o => o.Payment).ToList();
            return View(orders);
        }

        public ActionResult OrderDetails(int id)
        {
            var order = db.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails.Select(d => d.Product))
                .Include(o => o.Payment)
                .FirstOrDefault(o => o.OrderID == id);

            if (order == null)
                return HttpNotFound();

            return View(order);
        }

        [HttpPost]
        public ActionResult UpdateOrderStatus(int id, string status)
        {
            var order = db.Orders.Find(id);
            if (order == null)
                return HttpNotFound();

            order.OrderStatus = status;
            db.SaveChanges();

            return RedirectToAction("Order");
        }

        public ActionResult Revenue(string filter)
        {
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.Now;

            switch (filter)
            {
                case "day":
                    startDate = DateTime.Today;
                    break;
                case "week":
                    startDate = DateTime.Today.AddDays(-7);
                    break;
                case "month":
                    startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    break;
                case "year":
                    startDate = new DateTime(DateTime.Today.Year, 1, 1);
                    break;
                default:
                    break;
            }

            var orders = db.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .ToList();

            var orderIds = orders.Select(o => o.OrderID).ToList();

            var orderDetails = db.OrderDetails
                .Where(od => orderIds.Contains(od.OrderID))
                .ToList();

            ViewBag.TotalOrders = orders.Count;
            ViewBag.TotalSoldProducts = orderDetails.Sum(od => od.Quantity);
            ViewBag.TotalRevenue = orderDetails.Sum(od => od.Quantity * od.UnitPrice);

            var revenueByDate = orders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key.ToString("dd/MM"),
                    Revenue = g.SelectMany(o => o.OrderDetails).Sum(od => od.Quantity * od.UnitPrice)
                })
                .OrderBy(r => r.Date)
                .ToList();

            ViewBag.ChartLabels = revenueByDate.Select(r => r.Date).ToList();
            ViewBag.ChartValues = revenueByDate.Select(r => r.Revenue).ToList();

            return View();
        }



        public ActionResult Delivery()
        {
            return View();
        }

        public ActionResult Users()
        {
            var users = db.Users.OrderByDescending(u => u.CreatedAt).ToList();
            return View(users);
        }

        public ActionResult SetRole(int id, string role)
        {
            var user = db.Users.Find(id);
            if (user == null)
                return HttpNotFound();

            // Chỉ cho phép giá trị hợp lệ
            if (role != "Admin" && role != "User")
            {
                TempData["Error"] = "Vai trò không hợp lệ.";
                return RedirectToAction("Users");
            }

            user.Role = role;
            db.SaveChanges();

            TempData["Success"] = "Phân quyền thành công!";
            return RedirectToAction("Users");
        }



        public ActionResult BlockUser(int id)
        {
            var user = db.Users.Find(id);
            if (user != null)
            {
                user.IsBlocked = true;
                db.SaveChanges();
            }

            return RedirectToAction("Users");
        }

        public ActionResult UnblockUser(int id)
        {
            var user = db.Users.Find(id);
            if (user != null)
            {
                user.IsBlocked = false;
                db.SaveChanges();
            }

            return RedirectToAction("Users");
        }



        public ActionResult Category()
        {
            var categories = db.Categories.OrderByDescending(c => c.CreatedAt).ToList();
            return View(categories);
        }

        [HttpPost]
        public ActionResult AddCategory(string categoryName)
        {
            if (!string.IsNullOrEmpty(categoryName))
            {
                var category = new Category
                {
                    CategoryName = categoryName,
                    CreatedAt = DateTime.Now
                };
                db.Categories.Add(category);
                db.SaveChanges();
            }

            return RedirectToAction("Category");
        }

        // Xoá danh mục
        public ActionResult DeleteCategory(int id)
        {
            var category = db.Categories.Find(id);
            if (category != null)
            {
                db.Categories.Remove(category);
                db.SaveChanges();
            }

            return RedirectToAction("Category");
        }

        // Cập nhật danh mục (GET)
        public ActionResult EditCategory(int id)
        {
            var category = db.Categories.Find(id);
            if (category == null)
                return HttpNotFound();

            return View(category);
        }

        // Cập nhật danh mục (POST)
        [HttpPost]
        public ActionResult EditCategory(Category updatedCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(updatedCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Category");
            }

            return View(updatedCategory);
        }
    }
}