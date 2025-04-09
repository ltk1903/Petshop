using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetShop_Website.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult Product()
        {
            return View();
        }

        public ActionResult Stock()
        {
            return View();
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