using System;
using System.Collections.Concurrent;
using System.Threading.Tasks; 
using Simp.Rpc.Address;
using Simp.Rpc.Route;

namespace Simp.Rpc.Invoker
{
    public class SimpleInvokerFactory : InvokerFactory<SimpleResponseMessage>
    {
        private static readonly ConcurrentDictionary<ServerDescription, Invoker<SimpleResponseMessage>> invokerMap = new ConcurrentDictionary<ServerDescription, Invoker<SimpleResponseMessage>>(new ServerDescriptionComparer());
        private static readonly IServerRouteManager serverRouteManager = new SimpleServerRouteManager();
        private readonly IAddressProvider addressProvider = new PollingAddressProvider(); 

        public async Task<Invoker<SimpleResponseMessage>> CreateInvokerAsync(string serverName, string @group = "")
        {
            var server =await serverRouteManager.GetServerRouteAsync(serverName, @group);
            if (server == null)
                throw new Exception("serverroute not found");

            Invoker<SimpleResponseMessage> invoker;
            if (!invokerMap.TryGetValue(server, out invoker))
            {
                invoker = new SimpleInvoker(serverRouteManager, addressProvider, serverName, group);
                invokerMap.TryAdd(server, invoker);
            }

            return invoker;
        }
    }
}