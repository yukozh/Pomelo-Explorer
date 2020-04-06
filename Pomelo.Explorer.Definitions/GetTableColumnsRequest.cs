namespace Pomelo.Explorer.Definitions
{
    public class GetTableColumnsRequest
    {
        public string Database { get; set; } = null;

        public string Table { get; set; } = null;
    }
}
