using Microsoft.AspNetCore.Mvc;

namespace ShoppingApp.Controllers
{
    public class BuyerController : Controller
    {
        public IActionResult AllItemsReport()
        {
            return View();
        }

        public IActionResult ShoppingCart()
        {
            return View();
        }

        public IActionResult ViewItem()
        {
            return View();
        }

    }
}
