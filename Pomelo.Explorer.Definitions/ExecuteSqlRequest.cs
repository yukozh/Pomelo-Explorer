namespace Pomelo.Explorer.Definitions
{
    public class ExecuteSqlRequest
    {
        public string Database { get; set; }

        public string Sql { get; set; }

        public string[] Parameters { get; set; }

        public string[] Placeholders { get; set; }

        public string[] DbTypes { get; set; }
    }
}
