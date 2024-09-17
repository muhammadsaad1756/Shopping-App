using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Data;
using ShoppingApp.Models;
using System.Linq;

namespace ShoppingApp.Controllers
{
    public class SellersPageController : Controller
    {
        private readonly AppDbContext _context;

        public SellersPageController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult UserHomePage()
        {
            var userId = 1; // Hardcoded userId for now
            var itemsForSale = _context.Items.Where(item => item.SellerId == userId).ToList();
            return View(itemsForSale);
        }

        public IActionResult AddEditItem(int? id)
        {
            if (id == null)
            {
                return View(new Item());
            }
            var item = _context.Items.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        [HttpPost]
        public IActionResult SaveItem(Item item)
        {
            // Ensure model binding is working correctly
            if (item == null)
            {
                return BadRequest("Invalid item data.");
            }

            // Check model state for validation errors
            if (!ModelState.IsValid)
            {
                // Return back to the form with the model to show validation errors
                return View("AddEditItem", item);
            }

            // Check if the item is a new one (Id = 0) or an existing one
            if (item.Id == 0)
            {
                // Adding new item
                _context.Items.Add(item);
            }
            else
            {
                // Editing existing item
                var existingItem = _context.Items.Find(item.Id);
                if (existingItem == null)
                {
                    return NotFound("Item not found.");
                }

                // Update the existing item with the form values
                existingItem.Name = item.Name;
                existingItem.Description = item.Description;
                existingItem.Price = item.Price;
                existingItem.QuantityAvailable = item.QuantityAvailable;

                _context.Items.Update(existingItem);
            }

            // Save changes to the database
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Handle any database-related errors
                ModelState.AddModelError(string.Empty, "Unable to save changes: " + ex.Message);
                return View("AddEditItem", item);
            }

            // Redirect back to the user home page after saving
            return RedirectToAction("UserHomePage");
        }


        public IActionResult DeleteItem(int id)
        {
            var item = _context.Items.Find(id);
            if (item != null)
            {
                _context.Items.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction("UserHomePage");
        }
    }
}
