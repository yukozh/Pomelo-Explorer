using System;
using System.Collections.Generic;
using System.Text;

namespace Pomelo.Explorer.Definitions
{
    public class TableResponse
    {
        public List<string> Columns { get; set; }

        public List<List<string>> Values { get; set; }
    }
}
