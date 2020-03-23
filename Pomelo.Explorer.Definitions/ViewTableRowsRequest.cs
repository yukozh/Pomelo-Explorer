using System;
using System.Collections.Generic;
using System.Text;

namespace Pomelo.Explorer.Definitions
{
    public class ViewTableRowsRequest
    {
        public string Instance { get; set; }

        public string Database { get; set; }

        public string Table { get; set; }
    }
}
