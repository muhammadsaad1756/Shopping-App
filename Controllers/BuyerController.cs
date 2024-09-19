using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Data;
using ShoppingApp.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace ShoppingApp.Controllers
{
    public class BuyerController : Controller
    {
        private readonly AppDbContext _context;

        public BuyerController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult AllItemsReport(string searchTerm)
        {
            var items = _context.Items.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                items = items.Where(i => i.Name.Contains(searchTerm));
            }

            var model = items.ToList();

            // Pass searchTerm to ViewData for persisting search box value
            ViewData["searchTerm"] = searchTerm;

            return View(model);
        }


        public IActionResult ViewItem(int id)
        {
            var item = _context.Items.FirstOrDefault(i => i.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // Updated AddToCart action to handle missing ShoppingCart association
        [HttpPost]
        public IActionResult AddToCart(int itemId, int quantity)
        {
            var buyerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); // Ensure BuyerId is retrieved

            // Check if a shopping cart exists for this buyer
            var shoppingCart = _context.ShoppingCarts.FirstOrDefault(sc => sc.UserId == buyerId);

            // If no shopping cart exists for the buyer, create one
            if (shoppingCart == null)
            {
                shoppingCart = new ShoppingCart
                {
                    UserId = buyerId
                };
                _context.ShoppingCarts.Add(shoppingCart);
                _context.SaveChanges(); // Save changes to generate ShoppingCartId
            }

            // Retrieve the cart items for the current user
            var cartItem = _context.CartItems
                .FirstOrDefault(ci => ci.BuyerId == buyerId && ci.ItemId == itemId);

            // Add or update cart item
            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    ItemId = itemId,
                    BuyerId = buyerId,
                    ShoppingCartId = shoppingCart.Id, // Link to the shopping cart
                    Quantity = quantity,
                    TotalPrice = _context.Items.Find(itemId).Price * quantity
                };
                _context.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
                cartItem.TotalPrice = _context.Items.Find(itemId).Price * cartItem.Quantity;
                _context.CartItems.Update(cartItem);
            }

            _context.SaveChanges(); // Save changes to the database

            return RedirectToAction("ShoppingCart");
        }

        public async Task<IActionResult> ShoppingCart()
        {
            var buyerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); // Ensure BuyerId is retrieved

            // Fetch cart items for the buyer, including related Item details
            var cartItems = await _context.CartItems
                .Include(c => c.Item) // Eager loading of related Item
                .Where(c => c.BuyerId == buyerId)
                .ToListAsync();

            return View(cartItems);
        }

        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ShoppingCart));
        }

        public IActionResult Checkout()
        {
            return View();
        }
    }
}
