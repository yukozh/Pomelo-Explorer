using System.Collections.Generic;

namespace Pomelo.Explorer.MySQL.Models
{
    public class MySqlQueryResult
    {
        public string Command { get; set; }

        public string Table { get; set; }

        public long TimeSpan { get; set; }

        public long RowsAffected { get; set; }

        public bool Readonly { get; set; }

        public IEnumerable<string> Columns { get; set; }

        public IEnumerable<string> Keys { get; set; }

        public IEnumerable<string> ColumnTypes { get; set; }

        public IEnumerable<string> Nullable { get; set; }

        public List<List<string>> Rows { get; set; }
    }
}
