
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Client;
using Microsoft.Extensions.DependencyInjection;
using Simp.Rpc.Server;
using Simp.Rpc.Service;
using TestServiceContract;

namespace Server
{
    using System;
    using System.Threading.Tasks;

    class Program
    {
        static void Main()
        {
            Task.Run(async () =>
            {
                Assembly.Load("TestServiceContract");

                IServiceCollection serviceDICollection = new ServiceCollection();

                IDictionary<string, RpcServiceInfo> rpcServices = await new AttributeRpcServiceProvider(serviceDICollection).ScanRpcServices();

                IServiceProvider serviceProvider = serviceDICollection.BuildServiceProvider();

                RpcServiceContainer rpcServiceContainer = new RpcServiceContainer(serviceProvider).BuildRpcServices(rpcServices);
 
                IServer server = new SimpleServer(rpcServiceContainer, new ServerOptions { EndPoint = new IPEndPoint(ServerSettings.Host, ServerSettings.Port) });
                await server.StartAsync();

                Console.WriteLine($"服务端启动成功，{DateTime.Now}。");
            });
            Console.ReadLine();
        }
    }
}