using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Models;
using ShoppingApp.Data;
using System.Linq;

namespace ShoppingApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // Login action
        public IActionResult Login()
        {
            ViewData["Title"] = "Login";
            return View();
        }

        [HttpPost]
        public IActionResult Login(string UserName, string Password)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == UserName && u.PasswordHash == Password);

            if (user != null)
            {
                // Set ViewBag properties based on user role and profile completion
                ViewBag.IsLoggedIn = true;
                ViewBag.IsProfileComplete = !string.IsNullOrEmpty(user.Name);

                // Redirect based on user role (Seller or Buyer)
                if (user.Role == "Seller")
                {
                    return RedirectToAction("UserHomePage", "SellersPage");
                }
                else if (user.Role == "Buyer")
                {
                    return RedirectToAction("AllItemsReport", "Buyer");
                }
            }

            ViewBag.IsLoggedIn = false; // Incorrect login attempt
            return View();
        }

        // UserDetails action
        public IActionResult UserDetails()
        {
            ViewData["Title"] = "User Details";
            return View(new User());
        }

        [HttpPost]
        public IActionResult UserDetails(User model)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == model.UserName);
            if (user != null)
            {
                user.Name = model.Name;
                user.Age = model.Age;
                user.ProfilePictureUrl = model.ProfilePictureUrl;
                user.PasswordHash = string.IsNullOrEmpty(model.PasswordHash) ? user.PasswordHash : model.PasswordHash;
                _context.SaveChanges();
                ViewBag.IsProfileComplete = true;
            }

            return View(model);
        }
    }
}
