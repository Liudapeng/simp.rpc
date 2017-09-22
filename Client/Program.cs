using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Address;
using Common.Route;
using Common;
using Common.Codec;
using Common.Invoker;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Newtonsoft.Json;

namespace Client
{
    internal class Program
    {
        private static void RunClientAsync()
        {
            ConfigHelper.SetConsoleLogger();
            var invokerFactory = new SimpleInvokerFactory();
 
            do
            {
                try
                { 
                    for (int j = 0; j < 10; j++)
                    {  
                        Task.Run(async () =>
                        {
                            var response = invokerFactory.CreateInvoker("Server1").InvokeAsync("service", "method", new List<object> { 1, 2, 3, "123" });

                            Console.WriteLine($"do other things : {Thread.CurrentThread.ManagedThreadId}"); 

                            Console.WriteLine("response:" + JsonConvert.SerializeObject(await response));

                            Console.WriteLine("all done");
                        });

                    }
                     
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
            RunClientAsync(); 
            Console.ReadLine();
        }
         
    }
}