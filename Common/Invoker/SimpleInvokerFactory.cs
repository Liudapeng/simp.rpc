using System;
using System.Collections.Concurrent;
using Common.Address;
using Common.Route;

namespace Common.Invoker
{
    public class SimpleInvokerFactory : InvokerFactory<SimpleResponseMessage>
    {
        private static readonly ConcurrentDictionary<ServerDescription, Invoker<SimpleResponseMessage>> invokerMap = new ConcurrentDictionary<ServerDescription, Invoker<SimpleResponseMessage>>(new ServerDescriptionComparer());
        private static readonly IServerRouteManager serverRouteManager = new SimpleServerRouteManager();
        private readonly IAddressProvider addressProvider = new PollingAddressProvider(); 

        public Invoker<SimpleResponseMessage> CreateInvoker(string serverName, string @group = "")
        {
            var server = serverRouteManager.GetServerRouteAsync(serverName, @group).Result;
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