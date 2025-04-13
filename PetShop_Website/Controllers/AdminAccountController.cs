using System.Linq;
using System.Web.Mvc;
using PetShop_Website.Models;
using System;

namespace PetShop_Website.Controllers
{
    public class AdminAccountController : Controller
    {
        private PetShopContext db = new PetShopContext(); 

        // GET: AdminAccount/Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            ViewBag.Step = "Bắt đầu kiểm tra";

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin!";
                return View();
            }

            // Dùng AsNoTracking để tránh proxy
            var admin = db.Users.AsNoTracking().FirstOrDefault(u => u.Email == email && u.Role == "Admin");

            if (admin == null)
            {
                ViewBag.Error = "Không tìm thấy tài khoản admin!";
                return View();
            }

            if (!BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash))
            {
                ViewBag.Error = "Sai mật khẩu!";
                return View();
            }

            ViewBag.Step = "Thành công, chuyển hướng";

            // Clone đối tượng admin để tránh proxy
            var cleanAdmin = new User
            {
                UserID = admin.UserID,
                Username = admin.Username,
                Email = admin.Email,
                Role = admin.Role,
                CreatedAt = admin.CreatedAt
            };

            Session["Admin"] = cleanAdmin;
            return RedirectToAction("Dashboard", "Admin");
        }


        public ActionResult Register()
        {
            return View();
        }

        public ActionResult SeedAdmin()
        {
            string email = "khang1903@gmail.com";
            string password = "khang1903";
            string username = "Khangle";

            if (db.Users.Any(u => u.Email == email))
                return Content("Admin đã tồn tại.");

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var admin = new User
            {
                Username = username,
                Email = email,
                PasswordHash = hashedPassword,
                Role = "Admin",
                CreatedAt = DateTime.Now
            };

            db.Users.Add(admin);
            db.SaveChanges();

            return Content("Tạo admin thành công!");
        }

        public ActionResult Logout()
        {
            Session["Admin"] = null;
            return RedirectToAction("Login", "AdminAccount");
        }

    }
}
