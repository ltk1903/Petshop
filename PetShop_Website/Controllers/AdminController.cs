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

        public ActionResult Revenue()
        {
            return View();
        }

        public ActionResult Delivery()
        {
            return View();
        }

        public ActionResult Users()
        {
            return View();
        }

        public ActionResult Category()
        {
            return View();
        }
    }
}