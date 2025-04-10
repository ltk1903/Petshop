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
        // GET: Admin
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

        public ActionResult Order()
        {
            return View();
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