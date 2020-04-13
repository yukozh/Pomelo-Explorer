using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace Pomelo.Explorer.MySQL.Controllers
{
    public partial class MySqlController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> GetDatabases(string id)
        {
            var conn = ConnectionHelper.Connections[id];
            using (var command = new MySqlCommand("SHOW DATABASES;", ConnectionHelper.Connections[id]))
            {
                await conn.EnsureOpenedAsync();
                var result = new List<string>();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(reader["Database"].ToString());
                    }
                }
                return Json(result);
            }
        }
    }
}
