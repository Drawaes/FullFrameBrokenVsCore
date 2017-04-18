using Microsoft.AspNetCore.Server.Kestrel;
using Microsoft.AspNetCore.Server.Kestrel.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullFrameWork
{
    public static class HttpFilterExtension
    {
        public static KestrelServerOptions Switcheroo(this KestrelServerOptions options)
        {
            var prevFilter = options.ConnectionFilter ?? new NoOpConnectionFilter();
            options.ConnectionFilter = new ProtocolSwitchConnectionFilter(prevFilter);
            return options;
        }
    }
}
