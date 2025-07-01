using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ChickenWeb.Models;
using ChickenWeb.Data;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ChickenWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        private bool IsAdminLoggedIn()
        {
            return HttpContext.Session.GetString("AdminLoggedIn") == "true";
        }

        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.Email == email);
            if (admin == null)
            {
                ViewBag.Error = "Invalid email";
                return View();
            }

            var hasher = new PasswordHasher<Admin>();
            var result = hasher.VerifyHashedPassword(admin, admin.PasswordHash, password);
            if (result == PasswordVerificationResult.Success)
            {
                HttpContext.Session.SetString("AdminLoggedIn", "true");
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Invalid password";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult Index()
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            return View(_context.MenuItems.ToList());
        }

        public IActionResult Create()
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MenuItem item, IFormFile imageFile)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");

            if (!ModelState.IsValid)
            {
                return View(item); // prevents adding invalid data
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Path.GetFileName(imageFile.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/menu");
                Directory.CreateDirectory(folderPath); // ensures folder exists

                var fullPath = Path.Combine(folderPath, fileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                item.Image = "/images/menu/" + fileName;
            }

            _context.MenuItems.Add(item);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        public IActionResult Edit(int id)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            var item = _context.MenuItems.Find(id);
            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(MenuItem item, IFormFile imageFile)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");

            var existing = _context.MenuItems.Find(item.Id);
            if (existing == null) return NotFound();

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Path.GetFileName(imageFile.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/menu");
                Directory.CreateDirectory(folderPath);

                var path = Path.Combine(folderPath, fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                existing.Image = "/images/menu/" + fileName;
            }

            existing.Name = item.Name;
            existing.Spices = item.Spices;
            existing.Price = item.Price;
            existing.Type = item.Type;

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");

            var item = _context.MenuItems.Find(id);
            if (item == null) return NotFound();

            _context.MenuItems.Remove(item);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
