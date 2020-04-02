﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Pomelo.Explorer.MySQL.Models;
using ElectronNET.API;
using ElectronNET.API.Entities;

namespace Pomelo.Explorer.MySQL.Controllers
{
    public class EditorController : Controller
    {
        public static Dictionary<string, string> SpecialValues = new Dictionary<string, string>();

        [HttpPost("/mysql/editor/set-string-special-value")]
        public IActionResult SetStringSpecialValue([FromBody]SetSpecialValueRequest request)
        {
            if (!SpecialValues.ContainsKey(request.Key))
            {
                SpecialValues.Add(request.Key, null);
            }
            SpecialValues[request.Key] = request.Value;
            foreach (var x in Electron.WindowManager.BrowserWindows)
            {
                Electron.IpcMain.Send(x, "mysql-" + request.Key, SpecialValues[request.Key]);
            }
            return Json(true);
        }

        [HttpPost("/mysql/editor/set-blob-special-value")]
        public async Task<IActionResult> SetBlobSpecialValue([FromBody]SetSpecialValueRequest request)
        {
            if (!SpecialValues.ContainsKey(request.Key))
            {
                SpecialValues.Add(request.Key, null);
            }

            var mainWindow = Electron.WindowManager.BrowserWindows.First();
            var options = new OpenDialogOptions
            {
                Properties = new OpenDialogProperty[] {
                        OpenDialogProperty.openFile
                }
            };

            request.Value = (await Electron.Dialog.ShowOpenDialogAsync(mainWindow, options)).FirstOrDefault();
            if (System.IO.File.Exists(request.Value))
            {
                SpecialValues[request.Key] = Convert.ToBase64String(System.IO.File.ReadAllBytes(request.Value));
                foreach (var x in Electron.WindowManager.BrowserWindows)
                {
                    Electron.IpcMain.Send(x, "mysql-" + request.Key, SpecialValues[request.Key]);
                }
                return Json(true);
            }
            else 
            {
                // Show dialog
                return Json(false);
            }
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

        [HttpGet("/mysql/editor/text/{type}/{key}")]
        public IActionResult Text(string key, string type)
        {
            return View("Text", new TextEditorViewModel 
            {
                Id = key,
                Value = SpecialValues[key] as string,
                IsJson = type == "json"
            });
        }

        [HttpGet("/mysql/editor/hex/{key}")]
        public IActionResult Hex(string key)
        {
            return View((Convert.FromBase64String(SpecialValues[key])).Select(x => string.Format("{0:X2}", x)).ToArray());
        }

        [HttpPost("/mysql/editor/hex/{key}")]
        public IActionResult Hex(string key, [FromBody]string[] hex)
        {
            if (!SpecialValues.ContainsKey(key))
            {
                SpecialValues.Add(key, null);
            }

            var bytes = hex.Select(x => byte.Parse(x, System.Globalization.NumberStyles.HexNumber)).ToArray();
            SpecialValues[key] = Convert.ToBase64String(bytes);

            return Json(true);
        }
    }
}
