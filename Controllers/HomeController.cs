using ChickenWeb.Domain.Entities;
using ChickenWeb.Domain.Interfaces.IHome;
using ChickenWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ChickenWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService _context;

        public HomeController(IHomeService context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var menuItems = await _context.GetMenuItems();
            return View(menuItems);
        }

        [Authorize]
        public IActionResult Order()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitOrder([FromBody] OrderRequest request)
        {
            try
            {
                var userEmail = User.Identity?.Name ?? "Guest";
                var groupKey = Guid.NewGuid().ToString();
                var createdAt = DateTime.Now;

                await _context.SubmitOrderAsync(request, userEmail, groupKey, createdAt);

                // ✅ Return the redirect URL back to JavaScript
                return Ok(new
                {
                    redirectUrl = Url.Action("OrderSummary", "Home", new { orderId = groupKey })
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest("Argument error: " + ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Server error: " + ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> OrderSummary(string orderId)
        {
            var userEmail = User.Identity?.Name ?? "Guest";

            var items = await _context.GetOrderItemsByGroupKey(orderId);

            if (items == null || !items.Any())
                return Content("No recent orders found."); // Fallback message

            var model = new OrderSummary
            {
                Name = items.First().Name,
                Phone = items.First().Phone,
                Address = items.First().Address,
                Notes = items.First().Notes,
                OrderDate = items.First().CreatedAt,
                Items = items.Select(i => new CartItem
                {
                    Name = i.ItemName,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };

            return View(model);
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