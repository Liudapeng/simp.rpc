using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simp.Rpc.Service
{
    public class RpcServiceContainer
    {
        private IDictionary<string, RpcServiceInfo> _rpcServices;
        private readonly IServiceProvider serviceProvider;

        public RpcServiceContainer(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public RpcServiceContainer BuildRpcServices(IDictionary<string, RpcServiceInfo> rpcServices)
        {
            if (rpcServices == null || !rpcServices.Values.Any())
                return this;

            this._rpcServices = rpcServices;
            foreach (var rpcService in this._rpcServices.Values)
            {
                if (rpcService.IsImpl)
                {
                    rpcService.Instance = serviceProvider.GetService(rpcService.ServiceType);
                }
            }
            return this;
        }

        public object Execute(string service, string method, RpcParameterInfo[] args)
        {
            RpcServiceInfo rpcService;
            if (!this._rpcServices.TryGetValue(service, out rpcService))
                throw new Exception($"service: {service} not found");

            RpcMethodInfo rpcMethodInfo;
            if (!rpcService.Methods.TryGetValue(method, out rpcMethodInfo))
                throw new Exception($"method: {method} not found");

            object result = rpcMethodInfo.MethodInfo.Invoke(rpcService.Instance, args.Select(arg => arg.Value).ToArray());

            return result;
        }

        public RpcMethodInfo FindExecuter(string service, string method)
        {
            RpcServiceInfo rpcService;
            if (!this._rpcServices.TryGetValue(service, out rpcService))
                throw new Exception($"service: {service} not found");

            RpcMethodInfo rpcMethodInfo;
            if (!rpcService.Methods.TryGetValue(method, out rpcMethodInfo))
                throw new Exception($"method: {method} not found");

            return rpcMethodInfo;
        }
    }
}