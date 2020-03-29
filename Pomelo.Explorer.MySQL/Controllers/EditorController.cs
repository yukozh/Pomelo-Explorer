using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Pomelo.Explorer.MySQL.Models;

namespace Pomelo.Explorer.MySQL.Controllers
{
    public class EditorController : Controller
    {
        public static Dictionary<string, object> SpecialValues = new Dictionary<string, object>();

        [HttpPost("/mysql/editor/set-string-special-value")]
        public IActionResult SetStringSpecialValue([FromBody]SetSpecialValueRequest request)
        {
            if (!SpecialValues.ContainsKey(request.Key))
            {
                SpecialValues.Add(request.Key, null);
            }
            SpecialValues[request.Key] = request.Value;
            return Json(true);
        }

        [HttpPost("/mysql/editor/set-blob-special-value")]
        public IActionResult SetBlobSpecialValue([FromBody]SetSpecialValueRequest request)
        {
            if (!SpecialValues.ContainsKey(request.Key))
            {
                SpecialValues.Add(request.Key, null);
            }
            if (System.IO.File.Exists(request.Value))
            {
                SpecialValues[request.Key] = Convert.FromBase64String(request.Value);
                return Json(true);
            }
            else 
            {
                return Json(false);
            }
        }

        [HttpPost("/mysql/editor/set-image-special-value")]
        public async Task<IActionResult> SetImageSpecialValue([FromBody]SetSpecialValueRequest request)
        {
            if (!SpecialValues.ContainsKey(request.Key))
            {
                SpecialValues.Add(request.Key, null);
            }
            SpecialValues[request.Key] = await System.IO.File.ReadAllBytesAsync(request.Value);
            return Json(true);
        }

        [HttpPost("/mysql/editor/remove-special-value/{id}")]
        public IActionResult RemoveSpecialValue(string id)
        {
            if (!SpecialValues.ContainsKey(id))
            {
                SpecialValues.Remove(id);
            }
            return Json(true);
        }

        [HttpGet("/mysql/editor/text/{key}")]
        public IActionResult Text(string key, [FromQuery]string type)
        {
            return View("Text", SpecialValues[key] as string);
        }

        [HttpGet("/mysql/editor/hex/{key}")]
        public IActionResult Hex(string key)
        {
            return View((SpecialValues[key] as byte[]).Select(x => string.Format("{0:X2}", x)).ToArray());
        }

        [HttpPost("/mysql/editor/hex/{key}")]
        public IActionResult Hex(string key, [FromBody]string[] hex)
        {
            if (!SpecialValues.ContainsKey(key))
            {
                SpecialValues.Add(key, null);
            }

            var bytes = hex.Select(x => byte.Parse(x, System.Globalization.NumberStyles.HexNumber)).ToArray();
            SpecialValues[key] = bytes;

            return Json(true);
        }
    }
}
