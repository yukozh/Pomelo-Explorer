using Microsoft.AspNetCore.Mvc;

namespace Pomelo.Explorer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}