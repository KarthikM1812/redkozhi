using ChickenWeb.Domain.Entities;
using ChickenWeb.Domain.Interfaces.IAdmin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ChickenWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        private bool IsAdminLoggedIn()
        {
            return HttpContext.Session.GetString("AdminLoggedIn") == "true";
        }

        // Login GET
        public IActionResult Login() => View();

        // Login POST
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var (isValid, admin) = await _adminService.ValidateLoginAsync(email, password);
            if (!isValid || admin == null)
            {
                ViewBag.Error = "Invalid credentials.";
                return View();
            }

            HttpContext.Session.SetString("AdminLoggedIn", "true");
            return RedirectToAction("Index");
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // Admin Dashboard
        public async Task<IActionResult> Index()
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");

            var items = await _adminService.GetMenuItemsAsync();
            return View(items);
        }

        // Create GET
        public IActionResult Create()
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            return View();
        }

        // Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MenuItem item, IFormFile? imageFile)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");

            if (!ModelState.IsValid)
                return View(item);

            try
            {
                await _adminService.CreateMenuItemAsync(item, imageFile);
                return RedirectToAction("Index");
            }
            catch (ApplicationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(item);
            }
        }

        // Edit GET
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");

            var item = await _adminService.GetMenuItemAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        // Edit POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MenuItem item, IFormFile? imageFile)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");

            if (!ModelState.IsValid)
                return View(item);

            try
            {
                await _adminService.EditMenuItemAsync(item, imageFile);
                return RedirectToAction("Index");
            }
            catch (ApplicationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(item);
            }
        }

        // Delete
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");

            await _adminService.DeleteMenuItemAsync(id);
            return RedirectToAction("Index");
        }
    }
}
