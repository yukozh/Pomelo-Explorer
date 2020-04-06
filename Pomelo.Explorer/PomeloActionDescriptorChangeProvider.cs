using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;

namespace Pomelo.Explorer
{
    public class PomeloActionDescriptorChangeProvider : IActionDescriptorChangeProvider
    {
        public static PomeloActionDescriptorChangeProvider Instance { get; } = new PomeloActionDescriptorChangeProvider();

        public CancellationTokenSource TokenSource { get; private set; }

        public bool HasChanged { get; set; }

        public IChangeToken GetChangeToken()
        {
            TokenSource = new CancellationTokenSource();
            return new CancellationChangeToken(TokenSource.Token);
        }
    }
}
