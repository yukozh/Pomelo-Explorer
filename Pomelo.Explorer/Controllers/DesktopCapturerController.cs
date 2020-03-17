using Microsoft.AspNetCore.Mvc;

namespace Pomelo.Explorer.Controllers
{
    public class DesktopCapturerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}