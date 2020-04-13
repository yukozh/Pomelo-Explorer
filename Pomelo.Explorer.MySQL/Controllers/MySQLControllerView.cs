using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace Pomelo.Explorer.MySQL.Controllers
{
    public partial class MySqlController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> GetViews(string id, string database)
        {
            var conn = ConnectionHelper.Connections[id];
            using (var command = new MySqlCommand($"SHOW FULL TABLES FROM `{database}` WHERE `Table_type` = 'VIEW';", ConnectionHelper.Connections[id]))
            {
                await conn.EnsureOpenedAsync();
                var result = new List<string>();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(reader[0].ToString());
                    }
                }
                return Json(result);
            }
        }
    }
}
