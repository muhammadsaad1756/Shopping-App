using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ShoppingApp.Data;
using ShoppingApp.Models;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ShoppingApp.Controllers
{
    //[Authorize]
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // Login action (GET)
        public IActionResult Login()
        {
            ViewData["Title"] = "Login";
            return View();
        }

        // Login action (POST)

        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage = "Username and Password are required.";
                return View();
            }

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

                // Set session values for the logged-in user
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("UserName", user.UserName);
                HttpContext.Session.SetString("ProfilePictureUrl", user.ProfilePictureUrl ?? "defaultProfilePic.jpg");
                HttpContext.Session.SetString("IsLoggedIn", "true");

                return user.Role == "Seller"
                    ? RedirectToAction("UserHomePage", "SellersPage")
                    : RedirectToAction("AllItemsReport", "Buyer");
            }

            ViewBag.ErrorMessage = "Invalid login attempt.";
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
            // Check if user exists by username
            var user = _context.Users.FirstOrDefault(u => u.UserName == model.UserName);

            if (user != null)
            {
                // Update user details
                user.Name = model.Name;
                user.Age = model.Age;
                user.ProfilePictureUrl = model.ProfilePictureUrl;
                user.Role = model.Role; // Ensure role is updated as well
                user.PasswordHash = string.IsNullOrEmpty(model.PasswordHash) ? user.PasswordHash : model.PasswordHash;
                _context.SaveChanges();
                ViewBag.IsProfileComplete = true;
                return RedirectToAction("UserHomePage", "SellersPage");
            }
            else
            {
                // If user doesn't exist, add new user
                _context.Users.Add(model);
                _context.SaveChanges();
                return RedirectToAction("UserHomePage", "SellersPage");
            }
        }
    

        // Logout method

        public async Task<IActionResult> Logout()
        {
            // Sign out of cookie-based authentication
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Clear session
            HttpContext.Session.Clear();

            // Redirect to login page
            return RedirectToAction("Login");
        }

        // Optional: Access Denied for unauthorized access

        public IActionResult AccessDenied()
        {
            return View();
        }

        // Optional: Method to check if user is logged in (this can be used in other controllers)
        public bool IsUserLoggedIn()
        {
            return HttpContext.Session.GetString("IsLoggedIn") == "true";
        }
    }
}
