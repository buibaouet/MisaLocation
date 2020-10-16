using Microsoft.AspNetCore.Mvc;

namespace MISA.LocationAPI.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index(string message)
        {
            ViewBag.param = message;
            return View();
        }
    }
}