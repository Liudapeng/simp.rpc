using System.Reflection;
using DotNetty.Common.Internal.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Simp.Rpc.Client.Proxy;
using Simp.Rpc.Server;
using Simp.Rpc.Service;

namespace Simp.Rpc.Util
{
    public static class SimpRpcServiceCollectionExtensions
    {
        public static IServiceCollection AddLog(this IServiceCollection services)
        {
            services.AddLogging(builder => { builder.AddFilter((string category, LogLevel level) => true).AddConsole(); });
            return services;
        }

        public static IServiceCollection AddClient(this IServiceCollection services)
        {
            services.AddProxy(new AttributeRpcServiceProvider());
            return services;
        }

        public static IServiceCollection AddServer(this IServiceCollection services)
        {
            services.AddSingleton<IServerOptionProvider, SimpleServerOptionProvider>();
            services.AddSingleton<IRpcServiceProvider, AttributeRpcServiceProvider>();
            services.AddSingleton<IRpcServiceContainer, SimpleRpcServiceContainer>();
            services.AddSingleton<IServer, SimpleServer>();
             
            //builder.RegisterType<SimpleServerOptionProvider>().As<IServerOptionProvider>();
            //builder.RegisterType<AttributeRpcServiceProvider>().As<IRpcServiceProvider>().PropertiesAutowired().WithParameter(new TypedParameter(typeof(Assembly[]), assembly));
            //builder.RegisterType<SimpleRpcServiceContainer>().As<IRpcServiceContainer>().PropertiesAutowired();
            //builder.RegisterType<SimpleServer>().As<IServer>().PropertiesAutowired()
            //    .OnActivating(delegate
            //    {
            //        InternalLoggerFactory.DefaultFactory.AddConsole(filter, true);
            //    });
            return services;
        }
    }
}