using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ShoppingApp.Data;
using ShoppingApp.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ShoppingApp.Controllers
{
    [Authorize]
    public class SellersPageController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SellersPageController(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        //GET LOGGED IN USERS

        private int GetLoggedInUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId) ? userId : 0;
        }

        //USER HOME PAGE

        public IActionResult UserHomePage(string searchString)
        {
            var userId = GetLoggedInUserId();
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewData["CurrentFilter"] = searchString;

            var itemsForSale = _context.Items.Where(item => item.SellerId == userId);

            if (!string.IsNullOrEmpty(searchString))
            {
                itemsForSale = itemsForSale.Where(i => i.Name.Contains(searchString) || i.Description.Contains(searchString));
            }

            return View(itemsForSale.ToList());
        }

        //ADD EDIT ITEM

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
                // If no ID, it's a new item
                return View(new Items());
            }

            var item = _context.Items.Find(id);
            if (item == null || item.SellerId != userId)
            {
                return NotFound();
            }

            return View(item);
        }

        //SAVE ITEM

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveItem(Items item)
        {
            var userId = GetLoggedInUserId();
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            // Log user ID for debugging
            Console.WriteLine($"User ID: {userId}");

            // Assign SellerId
            item.SellerId = userId;

            // Server-side validation
            if (!ModelState.IsValid)
            {
                // Log model state errors for debugging
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }

                // Return view with validation errors to be displayed to the user
                return View("AddEditItem", item);
            }

            if (item.Id == 0) // Adding a new item
            {
                item.SellerId = userId; // Ensure this is set correctly
                _context.Items.Add(item);
            }
            else // Editing an existing item
            {
                var existingItem = _context.Items.Find(item.Id);
                if (existingItem == null || existingItem.SellerId != userId)
                {
                    return NotFound();
                }

                // Check if any fields are updated
                bool isUpdated = existingItem.Name != item.Name ||
                                 existingItem.Description != item.Description ||
                                 existingItem.Price != item.Price ||
                                 existingItem.QuantityAvailable != item.QuantityAvailable;

                if (!isUpdated)
                {
                    // Log that no changes were made
                    Console.WriteLine("No changes made to the item.");

                    TempData["InfoMessage"] = "No changes were made to the item.";
                    return View("AddEditItem", item); // Return the same view without saving
                }

                // Update fields only if changes were made
                existingItem.Name = item.Name;
                existingItem.Description = item.Description;
                existingItem.Price = item.Price;
                existingItem.QuantityAvailable = item.QuantityAvailable;
            }

            _context.SaveChanges();
            TempData["SuccessMessage"] = item.Id == 0 ? "Item added successfully." : "Item updated successfully.";

            return RedirectToAction("UserHomePage");
        }

        //DELETE ITEMS

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
                TempData["SuccessMessage"] = "Item deleted successfully!";
            }

            return RedirectToAction("UserHomePage");
        }
    }
}
