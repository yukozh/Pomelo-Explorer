using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using Pomelo.Explorer.MySQL.Models;

namespace Pomelo.Explorer.MySQL
{
    public static class MySqlCommandSpliter
    {
        public static string[] SplitCommand(string command)
        {
            return command
                .Replace("\\;", "$$POMELOSEMICOLON$$")
                .Split(';')
                .Select(x => x.Replace("$$POMELOSEMICOLON$$", "\\;").Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();
        }

        private static string HandleColumnName(string column)
        {
            var index = column.IndexOf(" AS ", StringComparison.OrdinalIgnoreCase);
            if (index < 0)
            {
                index = column.IndexOf("`AS`", StringComparison.OrdinalIgnoreCase);
            }
            string ret;
            if (index >= 0)
            {
                ret = column.Substring(index + 4);
            }
            else
            {
                ret = column;
            }

            return TrimDelimiter(ret);
        }

        public static string TrimDelimiter(string column)
        {
            column = column.Trim();
            if (column.Length == 0) return column;
            if (column.First() == column.Last() && column.First() == '`')
            {
                return column.Trim('`');
            }
            else
            {
                return column;
            }
        }

        public static CommandAnalyzeResult AnalyzeCommand(string command)
        {
            var ret = new CommandAnalyzeResult();
            ret.IsSimpleSelect = true;
            if (!command.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                ret.IsSimpleSelect = false;
                return ret;
            }

            var columns = new List<string>();
            var stringBuilder = new StringBuilder();
            var fromIndex = command.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
            if (fromIndex < 0)
            {
                ret.IsSimpleSelect = false;
                return ret;
            }

            for (var i = "SELECT".Length; i < command.Length; ++i)
            {
                var ended = i == fromIndex;
                if (command[i] == ',' || ended)
                {
                    var column = HandleColumnName(stringBuilder.ToString());
                    columns.Add(column);
                    stringBuilder.Clear();
                    if (ended)
                    {
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
                stringBuilder.Append(command[i]);
            }
            var lastColumn = HandleColumnName(stringBuilder.ToString());
            if (!string.IsNullOrWhiteSpace(lastColumn))
            {
                columns.Add(lastColumn);
            }
            ret.Columns = columns;

            stringBuilder.Clear();
            var index = -1;
            for (var i = fromIndex + " FROM ".Length; i < command.Length; ++i)
            {
                if (command[i] != ' ')
                {
                    stringBuilder.Append(command[i]);
                }
                else
                {
                    index = i;
                    break;
                }
            }
            ret.Table = stringBuilder.ToString().Trim().Trim('`');
            if (index >= 0)
            {
                if (command.IndexOf(" JOIN ", index, StringComparison.OrdinalIgnoreCase) >= 0
                    || command.IndexOf(" GROUP BY ", index, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    ret.IsSimpleSelect = false;
                    return ret;
                }
            }

            return ret;
        }

        public static IEnumerable<MySQLTableColumn> GetTableColumns(string table, MySqlConnection conn)
        {
            conn.EnsureOpened();
            using (var cmd = new MySqlCommand($"SHOW COLUMNS FROM `{table}`", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    yield return new MySQLTableColumn
                    {
                        Field = reader[0].ToString(),
                        Type = reader[1].ToString(),
                        Null = reader[2].ToString(),
                        Key = reader[3].ToString(),
                        Default = DBNull.Value == reader[4] ? null : reader[4].ToString(),
                        Extra = reader[5].ToString()
                    };
                }
            }
        }

        public static bool IsContainedKeys(IEnumerable<string> queryColumns, IEnumerable<MySQLTableColumn> tableColumns)
        {
            if (queryColumns != null && queryColumns.Count() == 1 && queryColumns.First() == "*")
            {
                return true;
            }

            var keys = tableColumns.Where(x => x.Key == "PRI").Select(x => x.Field).ToList();
            if (keys.Count() == 0)
            {
                return false;
            }

            if (queryColumns == null)
            {
                return false;
            }

            return keys.All(x => queryColumns.Contains(x));
        }
    }
}
