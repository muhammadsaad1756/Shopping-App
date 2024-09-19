using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ShoppingApp.Data;
using ShoppingApp.Models;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // Login action
        public IActionResult Login()
        {
            ViewData["Title"] = "Login";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == userName && u.PasswordHash == password);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                if (user.Role == "Seller")
                {
                    return RedirectToAction("UserHomePage", "SellersPage");
                }
                else if (user.Role == "Buyer")
                {
                    return RedirectToAction("AllItemsReport", "Buyer");
                }
            }

            ViewBag.IsLoggedIn = false;
            return View();
        }

        // UserDetails action
        public IActionResult UserDetails()
        {
            ViewData["Title"] = "User Details";
            var model = new Users
            {
                Role = "Seller" // Set default role to "Seller"
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult UserDetails(Users model)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == model.UserName);

            if (user != null)
            {
                user.Name = model.Name;
                user.Age = model.Age;
                user.ProfilePictureUrl = string.IsNullOrEmpty(model.ProfilePictureUrl) ? "defaultProfilePic.jpg" : model.ProfilePictureUrl;
                user.Role = model.Role;
                user.PasswordHash = string.IsNullOrEmpty(model.PasswordHash) ? user.PasswordHash : model.PasswordHash;
                _context.SaveChanges();
                ViewBag.IsProfileComplete = true;
                return RedirectToAction(user.Role == "Seller" ? "UserHomePage" : "AllItemsReport", user.Role == "Seller" ? "SellersPage" : "Buyer");
            }
            else
            {
                model.ProfilePictureUrl = string.IsNullOrEmpty(model.ProfilePictureUrl) ? "defaultProfilePic.jpg" : model.ProfilePictureUrl;
                _context.Users.Add(model);
                _context.SaveChanges();
                return RedirectToAction(model.Role == "Seller" ? "UserHomePage" : "AllItemsReport", model.Role == "Seller" ? "SellersPage" : "Buyer");
            }
        }

        // Logout action
        public IActionResult Logout()
        {
            // Clear the session
            _httpContextAccessor.HttpContext.Session.Remove("UserId");
            return RedirectToAction("Login");
        }
    }
}
