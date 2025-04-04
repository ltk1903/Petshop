using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetShop_Website.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // GET: /Account/Register
        public ActionResult Register()
        {
            return View();
        }
    }
}