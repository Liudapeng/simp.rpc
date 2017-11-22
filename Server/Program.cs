
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
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
        private static readonly Func<string, LogLevel, bool> filter = (string category, LogLevel level) => true;

        private static readonly Assembly[] assembly =
        {
            Assembly.Load("TestServiceContract"),//服务
            Assembly.Load("Server")//实现
        };
         

        static void Main(string[] args)
        {  
            Assembly.Load("TestServiceContract");//服务

            ContainerBuilder builder = new ContainerBuilder();
            builder.Populate(new ServiceCollection());
            builder.RegisterType<LoggerFactory>().As<ILoggerFactory>().SingleInstance()
                .OnActivating(delegate (IActivatingEventArgs<LoggerFactory> a)
                { 
                    a.Instance.AddConsole(filter, true);
                });
            builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>));
            builder.RegisterType<SimpleServerOptionProvider>().As<IServerOptionProvider>();
            builder.RegisterType<AttributeRpcServiceProvider>().As<IRpcServiceProvider>().PropertiesAutowired().WithParameter(new TypedParameter(typeof(Assembly[]), assembly));
            builder.RegisterType<SimpleRpcServiceContainer>().As<IRpcServiceContainer>().PropertiesAutowired();
            builder.RegisterType<SimpleServer>().As<IServer>().PropertiesAutowired()
                .OnActivating(delegate
                { 
                    InternalLoggerFactory.DefaultFactory.AddConsole(filter, true);
                });

            AutofacServiceProvider serviceProvider = new AutofacServiceProvider(builder.Build());

            Task task = Task.Run(async delegate
            {
                await serviceProvider.GetRequiredService<IServer>().StartAsync();
                serviceProvider.GetRequiredService<ILoggerFactory>()
                    .CreateLogger("Main")
                    .LogInformation(string.Format("服务端启动成功，{0}。", DateTime.Now));
            });
            task.ContinueWith(delegate (Task t)
            {
                if (t.IsFaulted)
                {
                    Console.WriteLine(t.Exception.GetBaseException());
                }
                else
                {
                    Console.WriteLine(t.Status);
                }
            });
            Console.ReadLine();
        }
    }
}