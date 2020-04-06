using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
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

        [HttpPost]
        public IActionResult StoreInstance([FromBody] Instance request)
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var path = Path.Combine(appDataPath, "Pomelo Explorer", request.InstanceId + ".json");
            var json = JsonSerializer.Serialize(request);
            System.IO.File.WriteAllText(path, json);
            return Json("ok");
        }

        [HttpGet]
        public IActionResult Instance()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var path = Path.Combine(appDataPath, "Pomelo Explorer");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var files = Directory.EnumerateFiles(path, "*.json");
            var ret = new List<Instance>(files.Count());
            foreach (var x in files)
            {
                var instance = JsonSerializer.Deserialize<Instance>(System.IO.File.ReadAllText(x));
                ret.Add(instance);
            }
            return Json(ret);
        }

        [HttpPost]
        public IActionResult RemoveInstance(string id)
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var path = Path.Combine(appDataPath, "Pomelo Explorer", id + ".json");
            System.IO.File.Delete(path);
            return Json(true);
        }
    }
}
