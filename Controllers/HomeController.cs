using Microsoft.AspNetCore.Mvc;

namespace ShoppingApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Home";
            ViewBag.IsLoggedIn = HttpContext.Session.GetString("IsLoggedIn") == "true";
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
            ViewBag.ProfilePictureUrl = HttpContext.Session.GetString("ProfilePictureUrl");

            return View();
        }
    }
}
