using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Pomelo.Explorer.MySQL.Controllers
{
    public class MySQLController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
