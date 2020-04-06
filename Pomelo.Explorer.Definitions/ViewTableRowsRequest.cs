namespace Pomelo.Explorer.Definitions
{
    public class ViewTableRowsRequest
    {
        public string Database { get; set; } = null;

        public string Table { get; set; } = null;

        public ConditionExpression Expression { get; set; } = null;

        public int Page { get; set; } = 0;
    }
}
