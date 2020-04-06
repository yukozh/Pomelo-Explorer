using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pomelo.Explorer.Models;
using ElectronNET.API;
using ElectronNET.API.Entities;

namespace Pomelo.Explorer.Controllers
{
    public class WindowsController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Open([FromBody]OpenWindowRequest request)
        {
            var options = new BrowserWindowOptions
            {
                Frame = true,
                DarkTheme = true
            };
            await Electron.WindowManager.CreateWindowAsync(options, $"http://localhost:{BridgeSettings.WebPort}{request.Url}");
            return Json(true);
        }
    }
}