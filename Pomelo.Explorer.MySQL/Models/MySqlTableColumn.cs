﻿namespace Pomelo.Explorer.MySQL.Models
{
    public class MySqlTableColumn
    {
        public string Field { get; set; }

        public string Type { get; set; }

        public string Null { get; set; }

        public string Key { get; set; }

        public string Default { get; set; }

        public string Extra { get; set; }
    }
}
