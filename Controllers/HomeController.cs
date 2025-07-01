// HomeController.cs (Corrected without changing structure)
using ChickenWeb.Data;
using ChickenWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;


namespace ChickenWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var menuItems = _context.MenuItems.ToList();
            return View(menuItems);
        }
        [Authorize]
        public IActionResult Order()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SubmitOrder([FromBody] OrderRequest request)
        {
            if (request == null || request.Cart == null || !request.Cart.Any())
                return BadRequest("Cart is empty");

            var userEmail = User.Identity?.Name ?? "Guest";

            foreach (var item in request.Cart)
            {
                var order = new OrderItem
                {
                    Name = request.Name,
                    Phone = request.Phone,
                    Address = request.Address,
                    Notes = request.Notes,
                    ItemName = item.Name,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    OrderedAt = DateTime.Now,
                    UserEmail = userEmail
                };

                _context.OrderItems.Add(order);
            }

            _context.SaveChanges();
            return Ok(new { orderId = DateTime.Now.Ticks }); // Temporary orderId
        }

        public IActionResult OrderSummary()
        {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
                return RedirectToAction("Login", "Account");

            var latestTime = _context.OrderItems
                .Where(o => o.UserEmail == userEmail)
                .OrderByDescending(o => o.OrderedAt)
                .Select(o => o.OrderedAt)
                .FirstOrDefault();

            if (latestTime == default)
                return NotFound("No recent orders found.");

            var items = _context.OrderItems
                .Where(o => o.UserEmail == userEmail && o.OrderedAt == latestTime)
                .ToList();

            var summary = new OrderSummaryViewModel
            {
                Name = items.First().Name,
                Phone = items.First().Phone,
                Address = items.First().Address,
                Notes = items.First().Notes,
                OrderDate = latestTime,
                Items = items.Select(i => new CartItem
                {
                    Name = i.ItemName,
                    Price = i.Price,
                    Quantity = i.Quantity
                }).ToList()
            };

            return View(summary);
        }

        

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
