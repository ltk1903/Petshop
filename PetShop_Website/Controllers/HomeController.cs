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


        public ActionResult Cart()
        {
            ViewBag.Message = "Your cart page.";

            return View();
        }

        public ActionResult Checkout()
        {
            ViewBag.Message = "Your checkout page.";

            return View();
        }
    }
}