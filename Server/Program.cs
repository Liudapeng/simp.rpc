
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
        private static readonly Assembly[] assembly = { Assembly.Load("TestServiceContract"), Assembly.Load("Server") }; 
        private static readonly IServiceCollection serviceDICollection = new ServiceCollection();

        static void Main()
        {
            Task.Run(async () =>
            {
                IRpcServiceProvider rpcServiceProvider = new AttributeRpcServiceProvider(assembly);
                 
                IRpcServiceContainer rpcServiceContainer = new SimpleRpcServiceContainer(rpcServiceProvider);

                IServer server = new SimpleServer(rpcServiceContainer, new ServerOptions { EndPoint = new IPEndPoint(ServerSettings.Host, ServerSettings.Port) });

                await server.StartAsync();

                Console.WriteLine($"服务端启动成功，{DateTime.Now}。");
            });
            Console.ReadLine();
        }
    }
}