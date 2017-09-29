using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProtoBuf.Meta;

namespace Simp.Rpc.Service
{
    public class SimpleRpcServiceContainer : IRpcServiceContainer
    {
        private readonly ILogger<SimpleRpcServiceContainer> logger;

        private readonly IDictionary<string, ServiceExcuter> excutersCache = new ConcurrentDictionary<string, ServiceExcuter>();
        private IDictionary<string, RpcServiceInfo> rpcServiceTable;
        private readonly IRpcServiceProvider rpcServiceProvider;
        private IServiceProvider serviceProvider;
        private static readonly object locker = new object();

        public SimpleRpcServiceContainer(IRpcServiceProvider rpcServiceProvider, ILogger<SimpleRpcServiceContainer> logger)
        {
            this.logger = logger;
            this.rpcServiceProvider = rpcServiceProvider;
            this.BuildRpcService();
        }

        private void BuildRpcService()
        {
            this.rpcServiceTable = this.rpcServiceProvider.ScanRpcServices();
            var serviceCollection = new ServiceCollection();
            foreach (var rpcServiceInfo in this.rpcServiceTable.Values)
            {
                if (rpcServiceInfo.IsImpl)
                {
                    serviceCollection.AddSingleton(rpcServiceInfo.ServiceType, rpcServiceInfo.ImplServiceType);
                }
            }
            this.serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public ServiceExcuter LookupExecuter(string service, string method)
        {
            ServiceExcuter excuter;
            if (excutersCache.TryGetValue(service, out excuter))
                return excuter;

            lock (locker)
            {
                if (excutersCache.TryGetValue(service, out excuter))
                    return excuter;

                RpcServiceInfo rpcService;
                if (!this.rpcServiceTable.TryGetValue(service, out rpcService))
                    throw new Exception($"service: {service} not found");

                RpcMethodInfo rpcMethodInfo;
                if (!rpcService.Methods.TryGetValue(method, out rpcMethodInfo))
                    throw new Exception($"method: {method} not found");

                excuter = new ServiceExcuter(serviceProvider.GetService(rpcService.ServiceType), rpcMethodInfo);
                excutersCache.TryAdd(service, excuter);
                return excuter;
            }
        }

        //public RpcServiceContainer BuildRpcServices(IDictionary<string, RpcServiceInfo> rpcServices)
        //{
        //    if (rpcServices == null || !rpcServices.Values.Any())
        //        return this;

        //    this._rpcServiceTable = rpcServices;
        //    foreach (var rpcService in this._rpcServiceTable.Values)
        //    {
        //        if (rpcService.IsImpl)
        //        {
        //           //
        //        }
        //    }
        //    return this;
        //}

        //public object Execute(string service, string method, RpcParameterInfo[] args)
        //{
        //    RpcServiceInfo rpcService;
        //    if (!this._rpcServiceTable.TryGetValue(service, out rpcService))
        //        throw new Exception($"service: {service} not found");

        //    RpcMethodInfo rpcMethodInfo;
        //    if (!rpcService.Methods.TryGetValue(method, out rpcMethodInfo))
        //        throw new Exception($"method: {method} not found");

        //    object result = rpcMethodInfo.MethodInfo.Invoke(rpcService.Instance, args.Select(arg => arg.Value).ToArray());

        //    return result;
        //}


    }
}