using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Simp.Rpc.Service
{
    public class SimpleRpcServiceContainer : IRpcServiceContainer
    {
        private IDictionary<string, RpcServiceInfo> _rpcServiceTable;
        private readonly IRpcServiceProvider _rpcServiceProvider;
        private IServiceProvider serviceProvider;

        public SimpleRpcServiceContainer(IRpcServiceProvider rpcServiceProvider)
        {
            this._rpcServiceProvider = rpcServiceProvider;
            BuildRpcService();
        }

        public void BuildRpcService()
        {
            this._rpcServiceTable = this._rpcServiceProvider.ScanRpcServices();
            var serviceCollection = new ServiceCollection();
            foreach (var rpcServiceInfo in this._rpcServiceTable.Values)
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
            RpcServiceInfo rpcService;
            if (!this._rpcServiceTable.TryGetValue(service, out rpcService))
                throw new Exception($"service: {service} not found");

            RpcMethodInfo rpcMethodInfo;
            if (!rpcService.Methods.TryGetValue(method, out rpcMethodInfo))
                throw new Exception($"method: {method} not found");

            return new ServiceExcuter(serviceProvider.GetService(rpcService.ServiceType), rpcMethodInfo);
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