using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks; 
using Simp.Rpc.Address;
using Simp.Rpc.Route;

namespace Simp.Rpc.Invoker
{
    public class SimpleInvokerFactory : InvokerFactory<SimpleResponseMessage>
    {
        private readonly IDictionary<ServerDescription, Invoker<SimpleResponseMessage>> invokerMap = new ConcurrentDictionary<ServerDescription, Invoker<SimpleResponseMessage>>(new ServerDescriptionComparer());
        private readonly IServerRouteManager serverRouteManager = new SimpleServerRouteManager();
        private readonly IAddressProvider addressProvider = new PollingAddressProvider();
        readonly object locker = new object();

        public async Task<Invoker<SimpleResponseMessage>> CreateInvokerAsync(string serverName, string @group = "")
        {
            var server = await serverRouteManager.GetServerRouteAsync(serverName, @group);
            if (server == null)
                throw new Exception("serverroute not found");
             
            Invoker<SimpleResponseMessage> invoker;

            if (invokerMap.TryGetValue(server, out invoker))
                return invoker;

            lock (locker)
            {
                if (invokerMap.TryGetValue(server, out invoker))
                    return invoker;

                invoker = new SimpleInvoker(serverRouteManager, addressProvider, serverName, group);
                invokerMap.TryAdd(server, invoker);
                return invoker;
            }  
        }
    }
}