
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using DotNetty.Common.Internal.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Simp.Rpc.Server;
using Simp.Rpc.Service;
using Simp.Rpc.Util;
using TestServiceContract;

namespace Server
{
    using System;
    using System.Threading.Tasks;

    class Program
    {
        private static readonly Assembly[] assembly =
        {
            Assembly.Load("TestServiceContract"),//服务
            Assembly.Load("Server")//实现
        };
         

        static void Main()
        {
            Assembly.Load("TestServiceContract");//服务

            ServiceCollection serviceCollection = new ServiceCollection();
            IServiceProvider serviceProvider = serviceCollection
                .AddLogging()
                .AddSingleton<IServer, SimpleServer>()
                .AddSingleton<IRpcServiceProvider, AttributeRpcServiceProvider>()
                .AddSingleton<IRpcServiceContainer, SimpleRpcServiceContainer>()
                .AddSingleton<IServerOptionProvider, SimpleServerOptionProvider>()
                .BuildServiceProvider();
             
            Task.Run(async () =>
            {
                serviceProvider.GetRequiredService<ILoggerFactory>().AddConsole((s, level) => true, true);

                InternalLoggerFactory.DefaultFactory.AddConsole((s, level) => true, true);

                await serviceProvider.GetRequiredService<IServer>().StartAsync(); 

                serviceProvider.GetRequiredService<ILoggerFactory>()
                    .CreateLogger("Main")
                    .LogInformation($"服务端启动成功，{DateTime.Now}。");
            });
            Console.ReadLine();
        }
    }
}