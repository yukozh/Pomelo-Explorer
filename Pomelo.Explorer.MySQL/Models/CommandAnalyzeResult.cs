using System.Collections.Generic;

namespace Pomelo.Explorer.MySQL.Models
{
    public class CommandAnalyzeResult
    {
        public bool IsSimpleSelect { get; set; }

        public List<string> Columns { get; set; }

        public string Table { get; set; }
    }
}
