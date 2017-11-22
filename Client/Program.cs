using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Simp.Rpc.Client.Proxy;
using Simp.Rpc.Service;
using TestServiceContract;

namespace Client
{
    internal class Program
    {
        private static ITestRpcService testRpcService;

        private static void RunClientByProxyAsync()
        {
            do
            {
                try
                {
                    var res1 = testRpcService.GetServiceCount();
                    var res2 = testRpcService.ExecuteService(new TestServiceRequest(), new TestServiceRequest2(), 11111);
                    var res3 = testRpcService.ExecuteServiceList(new List<TestServiceRequest>{ new TestServiceRequest() , new TestServiceRequest() });

                    Console.WriteLine(JsonConvert.SerializeObject(res1));
                    Console.WriteLine(JsonConvert.SerializeObject(res2));
                    Console.WriteLine(JsonConvert.SerializeObject(res3));
                }
                catch (AggregateException e)
                {
                    Console.WriteLine(e);
                }

                Console.ReadLine();

            } while (true);
        }

        private static void Main()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
 
            serviceCollection.AddProxy(new AttributeRpcServiceProvider());

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            testRpcService = serviceProvider.GetService<ITestRpcService>();

            RunClientByProxyAsync();

            Console.ReadLine();
        }
    }
}