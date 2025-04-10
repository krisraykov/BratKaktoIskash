using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using WebApplication6.Data;
using WebApplication6.Models;

namespace WebApplication6.Controllers
{
    public class UserController : Controller
    {
        public readonly ApplicationDbContext _db;
        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                if (_db.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email is already registered.");
                    return View(user);
                }
                if (user.Password != Request.Form["ConfirmPassword"])
                {
                    ModelState.AddModelError("Password", "Passwords are not matching.");
                    return View(user);
                }
                user.Password = HashPassword(user.Password);

                _db.Users.Add(user);
                _db.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(user);
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
        public ActionResult Profile()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User user)
        {
            if (Request.Form["Email"] != "" && Request.Form["Password"] != "")
            {
                if (_db.Users.Any(u => u.Email == user.Email && u.Password == HashPassword(user.Password)))
                {
                    var myUser = _db.Users.FirstOrDefault(u => u.Email == user.Email && u.Password == HashPassword(user.Password));
                    HttpContext.Session.SetString("UserId", myUser.Id.ToString());
                    if (Request.Form["RememberMe"].ToString() == "true")
                    {
                        Response.Cookies.Append("UserId", myUser.Id.ToString(), new CookieOptions { Expires = DateTime.Now.AddDays(30) });
                    }
                    return RedirectToAction("Profile");
                }
            }
            return View();
        }
        [HttpGet]
        public ActionResult Profile(User user)
        {
            int userId = int.Parse(HttpContext.Session.GetString("UserId"));

            if (userId == null)
            {
                return RedirectToAction("Login");
            }
            var myUser = _db.Users.Where(u => u.Id == userId).First();
            ViewBag.Name = myUser.Name;
            ViewBag.LastName = myUser.LastName;
            ViewBag.Email = myUser.Email;
            if (user == null)
            {
                return RedirectToAction("Login");
            }
            return View(user);
        }
        public ActionResult Delete()
        {
            var myUser = _db.Users.FirstOrDefault(u => u.Id == int.Parse(HttpContext.Session.GetString("UserId")));
            _db.Users.Remove(myUser);
            _db.SaveChanges();

            return RedirectToAction("Login");
        }
        [HttpPost]

        public ActionResult Update(User user)
        {
            var myUser = _db.Users.FirstOrDefault(u => u.Id == int.Parse(HttpContext.Session.GetString("UserId")));

            myUser.Name = user.Name;
            myUser.LastName = user.LastName;
            myUser.Email = user.Email;

            _db.Users.Update(myUser);
            _db.SaveChanges();

            return RedirectToAction("Profile");
        }
    }
}
