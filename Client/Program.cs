using System;
using System.Collections.Generic; 
using System.Threading;
using System.Threading.Tasks; 
using Simp.Rpc.Invoker; 
using Newtonsoft.Json;
using ProtoBuf;
using Simp.Rpc;
using TestServiceContract;

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
                    Task[] tasks = new Task[10];
                    for (int j = 0; j < 1; j++)
                    {
                        tasks[j] = Task.Run(async () =>
                        {
                            var response = (await invokerFactory.CreateInvokerAsync("Server1")).InvokeAsync("TestRpcService", "ExecuteService", new List<object> {new TestServiceRequest(),new TestServiceRequest2()});

                            Console.WriteLine($"do other things : {Thread.CurrentThread.ManagedThreadId}");

                            Console.WriteLine("response:" + JsonConvert.SerializeObject(await response));

                            Console.WriteLine("all done");
                        });

                        tasks[j].ContinueWith(task =>
                        {
                            if (task.IsFaulted)
                            {
                                Console.WriteLine(task.Exception.GetBaseException());
                            }
                            else
                            {
                                Console.WriteLine(task.Status);
                            }
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

    [ProtoContract]
    public class RequestParamTest
    {
        [ProtoMember(1)]
        public string Name = "RequestParamTest"; 
    }
}