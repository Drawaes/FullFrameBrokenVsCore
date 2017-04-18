using Microsoft.AspNetCore.Server.Kestrel.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullFrameWork
{
    public class ProtocolSwitchConnectionFilter : IConnectionFilter
    {
        private readonly IConnectionFilter _previous;

        public ProtocolSwitchConnectionFilter(IConnectionFilter previous)
        {
            _previous = previous;
            if (_previous == null)
            {
                throw new ArgumentNullException();
            }
        }

        public async Task OnConnectionAsync(ConnectionFilterContext context)
        {
            var connection = context.Connection;
            var back2Back = new BackToBackStream(connection);
            context.Connection = back2Back;
            
            await _previous.OnConnectionAsync(context);
            var previousRequest = context.PrepareRequest;
            context.PrepareRequest = features =>
            {
                previousRequest?.Invoke(features);
            };
        }
    }
}
