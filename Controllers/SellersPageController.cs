using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ShoppingApp.Data;
using ShoppingApp.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ShoppingApp.Controllers
{
    public class SellersPageController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SellersPageController(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetLoggedInUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId) ? userId : 0;
        }

        [Authorize(Roles = "Seller")]
        public IActionResult UserHomePage()
        {
            var userId = GetLoggedInUserId();
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var itemsForSale = _context.Items.Where(item => item.SellerId == userId).ToList();
            return View(itemsForSale);
        }

        [Authorize(Roles = "Seller")]
        public IActionResult AddEditItem(int? id)
        {
            var userId = GetLoggedInUserId();
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null)
            {
                return View(new Item());
            }

            var item = _context.Items.Find(id);
            if (item == null || item.SellerId != userId)
            {
                return NotFound();
            }

            return View(item);
        }
         

        [HttpPost]
        public IActionResult SaveItem(Item item)
        {
            // Replace this with your method to get the logged-in user's ID
            var userId = GetLoggedInUserId();

            if (userId == 0)
            {
                return RedirectToAction("Login", "Account"); // Ensure the user is logged in
            }

            if (item.Id == 0)
            {
                // Adding a new item
                item.SellerId = userId; // Set the seller ID to the logged-in user
                _context.Items.Add(item); // Save new item
            }
            else
            {
                // Editing an existing item
                var existingItem = _context.Items.Find(item.Id);

                if (existingItem == null || existingItem.SellerId != userId)
                {
                    return NotFound(); // Ensure the item belongs to the logged-in seller
                }

                // Update the item details
                existingItem.Name = item.Name;
                existingItem.Description = item.Description;
                existingItem.Price = item.Price;
                existingItem.QuantityAvailable = item.QuantityAvailable;
            }

            // Save changes to the database
            _context.SaveChanges();
            return RedirectToAction("UserHomePage");
        }

        // Helper method to get the logged-in user's ID (you need to implement this)
        


        public IActionResult DeleteItem(int id)
        {
            var userId = GetLoggedInUserId();
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var item = _context.Items.Find(id);
            if (item != null && item.SellerId == userId)
            {
                _context.Items.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction("UserHomePage");
        }
    }
}
