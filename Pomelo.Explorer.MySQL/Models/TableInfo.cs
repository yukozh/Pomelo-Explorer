namespace Pomelo.Explorer.MySQL.Models
{
    public class TableInfo
    {
        public string Name { get; set; }

        public int Columns { get; set; }

        public long Records { get; set; }

        public string Engine { get; set; }

        public string Charset { get; set; }

        public string Collate { get; set; }
    }
}
