using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.Explorer
{
    public struct ExtensionDescriptor
    {
        public string Id { get; set; }

        public string Version { get; set; }

        public string Icon { get; set; }

        public string Create { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
    }
}
