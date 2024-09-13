using Microsoft.AspNetCore.Mvc;

namespace ShoppingApp.Controllers
{
    public class SellersPageController : Controller
    {
        public IActionResult AddEditItem()
        {
            return View();
        }

        public IActionResult UserHomePage()
        {
            return View();
        }
    }
}
