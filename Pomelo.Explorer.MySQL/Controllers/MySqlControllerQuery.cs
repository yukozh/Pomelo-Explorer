using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public async Task<IActionResult> ExecuteNonQuery([FromRoute]string id, [FromBody]ExecuteSqlRequest request)
        {
            var conn = ConnectionHelper.Connections[id];
            using (var command = new MySqlCommand(request.Sql, ConnectionHelper.Connections[id]))
            {
                command.CommandText = $"USE `{request.Database}`; \r\n" + command.CommandText;
                if (request.Parameters != null)
                {
                    for (var i = 0; i < request.Parameters.Length; ++i)
                    {
                        MySqlParameter param;
                        if (request.Parameters[i] != null)
                        {
                            var converter = MySqlTypeMapper.Map[MySqlDbTypeParser.Parse(request.DbTypes[i].ToString()).Type];
                            var value = request.Parameters[i];
                            param = new MySqlParameter(request.Placeholders[i], converter(value));
                        }
                        else
                        {
                            param = new MySqlParameter(request.Placeholders[i], null);
                        }
                        command.Parameters.Add(param);
                    }
                }
                try
                {
                    await conn.EnsureOpenedAsync();
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
                return Json(await command.ExecuteNonQueryAsync());
            }
        }

        [HttpPost]
        public IActionResult ExecuteResult([FromRoute]string id, [FromBody]ExecuteSqlRequest request)
        {
            var conn = ConnectionHelper.Connections[id];
            var ret = new List<MySqlQueryResult>();
            var splitedCommands = MySqlCommandSpliter.SplitCommand(request.Sql);
            using (var command = new MySqlCommand())
            {
                command.Connection = conn;
                command.CommandText = $"USE `{request.Database}`;";
                try
                {
                    command.ExecuteNonQuery();
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
                foreach (var x in splitedCommands)
                {
                    var res = new MySqlQueryResult();
                    res.Command = x;
                    var analyze = MySqlCommandSpliter.AnalyzeCommand(x);
                    res.Table = analyze.Table;
                    command.CommandText = x;
                    var begin = DateTime.Now;
                    IEnumerable<MySqlTableColumn> tableColumns = null;
                    try
                    {
                        tableColumns = MySqlCommandSpliter.GetTableColumns(res.Table, conn).ToList();
                        res.Readonly = string.IsNullOrWhiteSpace(analyze.Table) || !(analyze.IsSimpleSelect && MySqlCommandSpliter.IsContainedKeys(analyze.Columns, tableColumns));
                    }
                    catch (MySqlException)
                    {
                        res.Readonly = true;
                    }
                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (res.Readonly)
                            {
                                res.Columns = GenerateColumnsFromReader(reader).ToList();
                                res.ColumnTypes = null;
                                res.Nullable = null;
                                res.Keys = null;
                                res.RowsAffected = reader.RecordsAffected;
                            }
                            else
                            {
                                res.Columns = analyze.Columns;
                                if (res.Columns.Count() == 1 && res.Columns.First() == "*")
                                {
                                    res.Columns = tableColumns.Select(x => x.Field);
                                }
                                else
                                {
                                    res.Columns = analyze.Columns;
                                }
                                res.ColumnTypes = res.Columns.Select(x => tableColumns.SingleOrDefault(y => y.Field == x)?.Type);
                                res.Nullable = res.Columns.Select(x => tableColumns.SingleOrDefault(y => y.Field == x)?.Null);
                                res.Keys = tableColumns.Where(x => x.Key == "PRI").Select(x => x.Field);
                                res.RowsAffected = reader.RecordsAffected;
                            }
                            if (reader.HasRows)
                            {
                                res.Rows = new List<List<string>>();
                                while (reader.Read())
                                {
                                    var row = new List<string>();
                                    for (var i = 0; i < reader.FieldCount; ++i)
                                    {
                                        if (reader.IsDBNull(i))
                                        {
                                            row.Add(null);
                                        }
                                        else if (reader.GetFieldType(i) == typeof(byte[]))
                                        {
                                            row.Add(Convert.ToBase64String((byte[])reader[i]));
                                        }
                                        else
                                        {
                                            row.Add(reader[i].ToString());
                                        }
                                    }
                                    res.Rows.Add(row);
                                }
                            }
                        }
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
                    var end = DateTime.Now;
                    res.TimeSpan = Convert.ToInt64((end - begin).TotalMilliseconds);
                    ret.Add(res);
                }
            }
            return Json(ret);
        }

        private IEnumerable<string> GenerateColumnsFromReader(MySqlDataReader reader)
        {
            for (var i = 0; i < reader.FieldCount; ++i)
            {
                yield return reader.GetName(i);
            }
        }
    }
}
