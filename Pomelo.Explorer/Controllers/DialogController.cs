using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pomelo.Explorer.Models;
using ElectronNET.API;
using ElectronNET.API.Entities;

namespace Pomelo.Explorer.Controllers
{
    public class DialogController : Controller
    {
        // GET: /<controller>/
        [HttpPost]
        public async Task<IActionResult> Show([FromBody]PopDialogRequest request)
        {
            //if (HybridSupport.IsElectronActive)
            {
                var options = new MessageBoxOptions(request.Message)
                {
                    Type = Enum.Parse<MessageBoxType>(request.Icon, true),
                    Title = request.Title
                };
                var result = await Electron.Dialog.ShowMessageBoxAsync(options);
            }
            return Json("OK");
        }
    }
}
