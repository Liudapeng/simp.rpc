using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Simp.Rpc.Service;

namespace Simp.Rpc.Client.Proxy
{
    public static class ProxyHelper
    {
        public static T Proxy<T>()
        {
            return DispatchProxy.Create<T, InvokeProxy<T>>();
        }

        public static IServiceCollection AddProxy(this IServiceCollection serviceCollection, IRpcServiceProvider rpcServiceProvider)
        {
            var serviceDict = rpcServiceProvider.ScanRpcServices();
            foreach (var rpcServiceInfo in serviceDict)
            {
                MethodInfo mi = typeof(ProxyHelper).GetMethod("Proxy").MakeGenericMethod(rpcServiceInfo.Value.ServiceType);
                serviceCollection.AddSingleton(rpcServiceInfo.Value.ServiceType, mi.Invoke(null, null));
            }
            return serviceCollection;
        }
    }
}