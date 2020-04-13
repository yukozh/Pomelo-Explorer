using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Pomelo.Explorer.Definitions;
using Pomelo.Explorer.MySQL.Models;

namespace Pomelo.Explorer.MySQL.Controllers
{
    public partial class MySqlController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> GetTables(string id, string database)
        {
            var conn = ConnectionHelper.Connections[id];
            using (var command = new MySqlCommand($"SHOW TABLE STATUS FROM `{database}` WHERE `Comment` <> 'VIEW';", ConnectionHelper.Connections[id]))
            {
                await conn.EnsureOpenedAsync();
                var result = new List<TableInfo>();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(new TableInfo 
                        {
                            Name = reader["Name"].ToString(),
                            Charset = "utf8mb4",
                            Collate = reader["Collation"].ToString(),
                            Columns = 0,
                            Records = Convert.ToInt64(reader["Rows"]),
                            Engine = reader["Engine"].ToString()
                        });
                    }
                }
                return Json(result);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetTableRows([FromRoute]string id, [FromBody]ViewTableRowsRequest request)
        {
            const int pageSize = 1000;
            var condition = "";
            if (request.Expression != null)
            {
                condition = "WHERE " + MySqlConditionExpressionTranslator.GenerateSql(request.Expression);
            }
            var sql = $"SELECT * FROM `{request.Database}`.`{request.Table}` {condition} LIMIT {pageSize} OFFSET {request.Page * pageSize}";

            var conn = ConnectionHelper.Connections[id];
            using (var command = new MySqlCommand(sql, ConnectionHelper.Connections[id]))
            {
                await conn.EnsureOpenedAsync();
                var result = new List<List<string>>();
                var columns = new List<string>();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    for (var i = 0; i < reader.FieldCount; ++i)
                    {
                        columns.Add(reader.GetName(i));
                    }

                    while (await reader.ReadAsync())
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
                        result.Add(row);
                    }
                }

                return Json(new TableResponse
                { 
                    Columns = columns,
                    Values = result
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetTableColumns([FromRoute]string id, [FromBody]GetTableColumnsRequest request)
        {
            var conn = ConnectionHelper.Connections[id];
            using (var command = new MySqlCommand($"SHOW COLUMNS IN `{request.Database}`.`{request.Table}`;", ConnectionHelper.Connections[id]))
            {
                await conn.EnsureOpenedAsync();
                var result = new List<MySqlTableColumn>();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(new MySqlTableColumn
                        {
                            Field = reader[0].ToString(),
                            Type = reader[1].ToString(),
                            Null = reader[2].ToString(),
                            Key = reader[3].ToString(),
                            Default = DBNull.Value == reader[4] ? null : reader[4].ToString(),
                            Extra = reader[5].ToString()
                        });
                    }
                }
                return Json(result);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetFullTableColumns([FromRoute]string id, [FromBody]GetTableColumnsRequest request)
        {
            var conn = ConnectionHelper.Connections[id];
            using (var command = new MySqlCommand(
                @$"
SELECT 
    `COLUMN_NAME`, `ORDINAL_POSITION`, `COLUMN_DEFAULT`, `IS_NULLABLE`, `DATA_TYPE`, `CHARACTER_MAXIMUM_LENGTH`, `NUMERIC_PRECISION`, `NUMERIC_SCALE`, `DATETIME_PRECISION`, `CHARACTER_SET_NAME`, `COLLATION_NAME`, `COLUMN_KEY`, `EXTRA`, `COLUMN_COMMENT`, `GENERATION_EXPRESSION`
FROM 
    `INFORMATION_SCHEMA`.`COLUMNS` 
WHERE 
    `TABLE_SCHEMA` = @schema 
    AND 
    `TABLE_NAME` = @table
ORDER BY 
    `ORDINAL_POSITION` ASC;", conn))
            {
                command.Parameters.Add(new MySqlParameter("schema", request.Database));
                command.Parameters.Add(new MySqlParameter("table", request.Table));
                await conn.EnsureOpenedAsync();
                var result = new List<MySqlTableColumnFull>();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(new MySqlTableColumnFull
                        {
                            Name = reader["COLUMN_NAME"].ToString(),
                            Type = reader["DATA_TYPE"].ToString(),
                            Nullable = reader["IS_NULLABLE"].ToString() == "YES",
                            Key = reader["COLUMN_KEY"].ToString(),
                            Default = DBNull.Value == reader["COLUMN_DEFAULT"] ? null : reader["COLUMN_DEFAULT"].ToString(),
                            Extra = reader["EXTRA"].ToString(),
                            CharMaxLength = DBNull.Value == reader["CHARACTER_MAXIMUM_LENGTH"] ? null : (long?)Convert.ToInt64(reader["CHARACTER_MAXIMUM_LENGTH"]),
                            NumericPrecision = DBNull.Value == reader["NUMERIC_PRECISION"] ? null : (long?)Convert.ToInt64(reader["NUMERIC_PRECISION"]),
                            NumericScale = DBNull.Value == reader["NUMERIC_SCALE"] ? null : (long?)Convert.ToInt64(reader["NUMERIC_SCALE"]),
                            DatetimePrecision = DBNull.Value == reader["DATETIME_PRECISION"] ? null : (long?)Convert.ToInt64(reader["DATETIME_PRECISION"]),
                            Charset = DBNull.Value == reader["CHARACTER_SET_NAME"] ? null : reader["CHARACTER_SET_NAME"].ToString(),
                            Collation = DBNull.Value == reader["COLLATION_NAME"] ? null : reader["COLLATION_NAME"].ToString(),
                            Comment = reader["COLUMN_COMMENT"].ToString(),
                            GenerationExpression = reader["GENERATION_EXPRESSION"].ToString(),
                            OrdinalPosition = Convert.ToInt64(reader["ORDINAL_POSITION"])
                        });
                    }
                }
                return Json(result);
            }
        }
    }
}
