namespace Pomelo.Explorer.MySQL.Models
{
    public class MySqlTableColumnFull
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public long OrdinalPosition { get; set; }

        public long? CharMaxLength { get; set; }

        public long? NumericPrecision { get; set; }

        public long? NumericScale { get; set; }

        public long? DatetimePrecision { get; set; }

        public string Charset { get; set; }

        public string Collation { get; set; }

        public bool Nullable { get; set; }

        public string Key { get; set; }

        public string Default { get; set; }

        public string Extra { get; set; }

        public string Comment { get; set; }

        public string GenerationExpression { get; set; }
    }
}
