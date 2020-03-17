using Microsoft.AspNetCore.Mvc;

namespace Pomelo.Explorer.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}