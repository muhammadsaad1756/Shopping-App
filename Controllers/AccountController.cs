using Microsoft.AspNetCore.Mvc;

namespace ShoppingApp.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Details()
        {
           
            ViewData["Title"] = "Details";
            ViewBag.IsLoggedIn = false; // or true, depending on your logic
            ViewBag.IsProfileComplete = false; // or true
            
            return View();
            //return View();
        }
    }
}
