using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Pomelo.Explorer.Controllers
{
    public class ExtensionController : Controller
    {
        // GET: /<controller>/
        public IActionResult List(bool Creatable)
        {
            IEnumerable<ExtensionDescriptor> descriptors = AssemblyHelper.ExtensionAssemblyContainer.Keys
                .Select(x => AssemblyHelper.GetExtensionDescriptor(AssemblyHelper.ExtensionAssemblyContainer[x]));

            if (Creatable)
            {
                descriptors = descriptors.Where(x => !string.IsNullOrEmpty(x.Create));
            }

            return Json(descriptors.ToList());
        }
    }
}
