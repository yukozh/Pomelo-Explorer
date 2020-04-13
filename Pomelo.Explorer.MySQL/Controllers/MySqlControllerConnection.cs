using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Text.Json;
using System.Reflection;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Pomelo.Explorer.Definitions;
using Pomelo.Explorer.MySQL.Models;

namespace Pomelo.Explorer.MySQL.Controllers
{
    public partial class MySqlController : Controller
    {
        [HttpPost]
        public IActionResult CreateConnection([FromBody]CreateConnectionRequest request)
        {
            if (!ConnectionHelper.Connections.ContainsKey(request.InstanceId))
            {
                var client = new MySqlConnection($"Server={request.Address}; Port={request.Port}; Uid={request.Username}; Pwd={request.Password}; Pooling=False; AllowUserVariables=True; Keepalive=60;");
                if (string.IsNullOrEmpty(request.InstanceId))
                {
                    request.InstanceId = DateTime.UtcNow.Ticks.ToString();
                }
                ConnectionHelper.Connections.Add(request.InstanceId, client);
            }
            return Json(new CreateConnectionResponse
            {
                Id = request.InstanceId
            });
        }


        [HttpPost]
        public async Task<IActionResult> OpenConnection(string id)
        {
            if (!ConnectionHelper.Connections.ContainsKey(id))
            {
                return NotFound(id);
            }

            try
            {
                await ConnectionHelper.Connections[id].OpenAsync();
            }
            catch (MySqlException ex)
            {
                Response.StatusCode = 400;
                return Json(new DBError
                {
                    Code = ex.Number,
                    Message = ex.Message
                });
            }

            return Json("OK");
        }

        [HttpPost]
        public async Task<IActionResult> TestConnection([FromBody]CreateConnectionRequest request)
        {
            using (var client = new MySqlConnection($"Server={request.Address}; Port={request.Port}; Uid={request.Username}; Pwd={request.Password}; Pooling=False; AllowUserVariables=True;"))
            {
                try
                {
                    await client.OpenAsync();
                }
                catch (MySqlException ex)
                {
                    Response.StatusCode = 400;
                    return Json(new DBError
                    {
                        Code = ex.Number,
                        Message = ex.Message
                    });
                }
                return Json("OK");
            }
        }
    }
}
