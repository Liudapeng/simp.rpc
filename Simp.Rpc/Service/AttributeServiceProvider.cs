using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Simp.Rpc.Service.Attributes;

namespace Simp.Rpc.Service
{
    public class AttributeRpcServiceProvider : IRpcServiceProvider
    {
        private Type[] _types;

        public AttributeRpcServiceProvider()
        {
            InitByAssemblies(AppDomain.CurrentDomain.GetAssemblies());
        }

        public AttributeRpcServiceProvider(Assembly[] assemblies)
        {
            InitByAssemblies(assemblies);
        }

        private void InitByAssemblies(Assembly[] assemblies)
        {
            var types = assemblies.Where(i => i.IsDynamic == false).SelectMany(i => i.ExportedTypes).ToArray();
            InitByTypes(types);
        }

        private void InitByTypes(Type[] types)
        {
            this._types = types;
        }

        /// <summary>
        /// 
        /// </summary> 
        /// <returns></returns>
        public IDictionary<string, RpcServiceInfo> ScanRpcServices()
        {
            //查找服务 
            var services = _types.Where(i => i.IsInterface && i.GetCustomAttribute<RpcServiceContractAttribute>() != null).ToList();
            Console.WriteLine($"服务列表：{Environment.NewLine}{string.Join(Environment.NewLine, services.Select(i => (!string.IsNullOrEmpty(i.GetCustomAttribute<RpcServiceContractAttribute>().Name) ? i.GetCustomAttribute<RpcServiceContractAttribute>().Name : i.Name) + " " + i.GetCustomAttribute<RpcServiceContractAttribute>().Description))}");

            //查找服务实现
            var serviceImpls = _types.Where(i => services.Any(service => i.IsClass && service.IsAssignableFrom(i))).ToList();
            IDictionary<string, RpcServiceInfo> serviceInfos = new Dictionary<string, RpcServiceInfo>();
            foreach (var service in services)
            {
                //别名
                var rpcServiceAttr = service.GetCustomAttribute<RpcServiceContractAttribute>();
                //多个实现类时用RpcServiceIgnore标记禁用的实现，如果有多个则取第一个
                var serviceImpl = serviceImpls.Find(impl => service.IsAssignableFrom(impl) && impl.GetCustomAttribute<RpcServiceIgnoreAttribute>() == null);
                var serviceName = string.IsNullOrEmpty(rpcServiceAttr.Name) ? service.Name : rpcServiceAttr.Name;
                var serviceDesc = rpcServiceAttr.Description;
                
                var rpcServiceInfo = new RpcServiceInfo
                {
                    IsImpl = serviceImpl != null,
                    Name = serviceName,
                    Description = serviceDesc,
                    ServiceType = service,
                    ImplServiceType = serviceImpl,
                    Methods = new Dictionary<string, RpcMethodInfo>()
                };

                foreach (var methodInfo in service.GetMethods())
                {
                    if (methodInfo.GetCustomAttribute<RpcServiceIgnoreAttribute>() != null)
                        continue;
                    Console.WriteLine($"methodInfo：{methodInfo.Name}");
                    var rpcMethodAttr = methodInfo.GetCustomAttribute<RpcServiceContractAttribute>();
                    var methodName = string.IsNullOrEmpty(rpcMethodAttr?.Name) ? methodInfo.Name : rpcMethodAttr.Name;
                    var methodDesc = rpcMethodAttr?.Description;
                    RpcMethodInfo rpcMethodInfo = new RpcMethodInfo
                    {
                        Name = methodName,
                        Description = methodDesc,
                        MethodInfo = methodInfo,
                        RpcParameters = methodInfo.GetParameters().Select(p => new RpcParameterInfo { Name = p.Name, Type = p.ParameterType }).ToArray(),
                        RpcReturnType = new RpcParameterInfo { Type = methodInfo.ReturnType }
                    };
                    rpcServiceInfo.Methods.TryAdd(rpcMethodInfo.Name, rpcMethodInfo);
                }
                serviceInfos.Add(rpcServiceInfo.Name, rpcServiceInfo);
            }
            return serviceInfos;
        }
    }
}