using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProtoBuf.Meta;
using Simp.Rpc.Util;

namespace Simp.Rpc.Service
{
    public class SimpleRpcServiceContainer : IRpcServiceContainer
    {
        public ILogger<SimpleRpcServiceContainer> Logger { get; set; }

        private readonly IDictionary<string, ServiceExcuter> excutersCache = new ConcurrentDictionary<string, ServiceExcuter>();
        private readonly IDictionary<string, RpcServiceInfo> rpcServiceTable = new ConcurrentDictionary<string, RpcServiceInfo>();
        private readonly IRpcServiceProvider rpcServiceProvider;
        private IServiceProvider serviceProvider;
        private static readonly object locker = new object();

        public SimpleRpcServiceContainer(IRpcServiceProvider rpcServiceProvider)
        {
            this.rpcServiceProvider = rpcServiceProvider;
            this.BuildRpcService();
        }

        private void BuildRpcService()
        {
            var serviceCollection = new ServiceCollection();
            string[] packages = GetServiceNameSpaces();
            var allServices = this.rpcServiceProvider.ScanRpcServices();
            foreach (KeyValuePair<string, RpcServiceInfo> item in allServices)
            {
                var rpcServiceInfo = item.Value;

                if (rpcServiceInfo.IsImpl && packages.Any(package => rpcServiceInfo.ImplServiceType.FullName.StartsWith(package)))
                {
                    rpcServiceTable.Add(item);
                    serviceCollection.AddSingleton(rpcServiceInfo.ServiceType, rpcServiceInfo.ImplServiceType);
                }
            }

            this.serviceProvider = serviceCollection.BuildServiceProvider();
        }

        /// <summary>
        /// 获取注册的服务集合，可供外部程序进行注册到注册中心等操作
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, RpcServiceInfo> GetRegisterServices()
        {
            return rpcServiceTable;
        }

        /// <summary>
        /// 查找执行器
        /// </summary>
        /// <param name="service"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public ServiceExcuter LookupExecuter(string service, string method)
        {
            ServiceExcuter excuter;
            string excuterKey = String.Format($"{service}.{method}");
            if (excutersCache.TryGetValue(excuterKey, out excuter))
                return excuter;

            lock (locker)
            {
                if (excutersCache.TryGetValue(excuterKey, out excuter))
                    return excuter;

                RpcServiceInfo rpcService;
                if (!this.rpcServiceTable.TryGetValue(service, out rpcService))
                    throw new Exception($"service: {service} not found");

                RpcMethodInfo rpcMethodInfo;
                if (!rpcService.Methods.TryGetValue(method, out rpcMethodInfo))
                    throw new Exception($"method: {method} not found");

                excuter = new ServiceExcuter(serviceProvider.GetService(rpcService.ServiceType), rpcMethodInfo);
                excutersCache.TryAdd(excuterKey, excuter);
                return excuter;
            }
        }
         
        /// <summary>
        /// 只扫描指定包下的实现类，防止将其他contract下的服务注入
        /// </summary>
        /// <returns></returns>
        private string[] GetServiceNameSpaces()
        {
            string[] packages;
            string servicePackage = ConfigHelper.Configuration["app:namespace"];
            if (String.IsNullOrWhiteSpace(servicePackage))
            {
                packages = new[] { ConfigHelper.Configuration["app:name"] };
            }
            else
            {
                packages = servicePackage.Split(",", StringSplitOptions.RemoveEmptyEntries);
            }

            return packages;
        }

    }
}