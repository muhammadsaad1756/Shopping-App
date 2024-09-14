using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Models;

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

            return View(new Item());
        }
    }
}
