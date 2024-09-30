using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Data;
using ShoppingApp.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ShoppingApp.Controllers
{
    [Authorize]
    public class BuyerController : Controller
    {
        private readonly AppDbContext _context;

        public BuyerController(AppDbContext context)
        {
            _context = context;
        }

        //ALL ITEMS REPORT

        public IActionResult AllItemsReport(string searchTerm)
        {
            var items = _context.Items.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                items = items.Where(i => i.Name.Contains(searchTerm));
            }

            var model = items.ToList();
            ViewData["searchTerm"] = searchTerm;
            return View(model);
        }

        //VIEW ITEM

        public IActionResult ViewItem(int id)
        {
            var item = _context.Items.FirstOrDefault(i => i.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        //ADD TO CART

        [HttpPost]
        public IActionResult AddToCart(int itemId, int quantity)
        {
            var buyerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var shoppingCart = _context.ShoppingCarts.FirstOrDefault(sc => sc.UserId == buyerId);

            if (shoppingCart == null)
            {
                shoppingCart = new ShoppingCart
                {
                    UserId = buyerId
                };
                _context.ShoppingCarts.Add(shoppingCart);
                _context.SaveChanges();
            }

            var cartItem = _context.CartItems
                .FirstOrDefault(ci => ci.BuyerId == buyerId && ci.ItemId == itemId);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    ItemId = itemId,
                    BuyerId = buyerId,
                    ShoppingCartId = shoppingCart.Id,
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

            _context.SaveChanges();
            return RedirectToAction("ShoppingCart");
        }

        //Shopping Cart

        public async Task<IActionResult> ShoppingCart()
        {
            var buyerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var cartItems = await _context.CartItems
                .Include(c => c.Item)
                .Where(c => c.BuyerId == buyerId)
                .ToListAsync();

            return View(cartItems);
        }

        [HttpPost] // Added this attribute to handle POST requests from the form
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();

                // Set TempData message
                TempData["Message"] = "Item has been removed from your cart.";
            }

            return RedirectToAction(nameof(ShoppingCart));
        }


        public IActionResult Checkout()
        {
            return View();
        }
    }
}


