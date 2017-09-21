using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common.Address;
using Common.Route;
using Common;
using Common.Codec;
using Common.Invoker;
using DotNetty.Transport.Channels;
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
                var response = invokerFactory.CreateInvoker("Server1").InvokeAsync("service", "method", new List<object> { 1, 2, 3, "123" });

                Console.WriteLine("do other things");

                Console.WriteLine("response:" + JsonConvert.SerializeObject(response.Result));

                Console.WriteLine("all done");

                Console.ReadLine();
            } while (true);
        }

        private static void Main()
        {
            RunClientAsync();
        }
    }
}