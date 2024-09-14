using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Models;
using ShoppingApp.Data;
using System.Linq;


namespace ShoppingApp.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            ViewData["Title"] = "Login";
            return View();
        }

        public IActionResult UserDetails()
        {

            ViewData["Title"] = "User Details";
            ViewBag.IsLoggedIn = false; // or true, depending on your logic
            ViewBag.IsProfileComplete = false; // or true

            return View(new User());
            //return View();
        }
    }
}
