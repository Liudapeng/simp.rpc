using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Simp.Rpc.Invoker;
using Newtonsoft.Json;
using ProtoBuf;
using Simp.Rpc;
using Simp.Rpc.Codec;
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
                    for (int j = 0; j < 5; j++)
                    {
                        tasks[j] = Task.Run(async () =>
                        {
                            var response = (await invokerFactory.CreateInvokerAsync("Server1")).InvokeAsync("TestRpcService", "ExecuteService", new List<object> { new TestServiceRequest(), new TestServiceRequest2(), 10000 });
                             
                            var res = await response;

                            Console.WriteLine("response:" + JsonConvert.SerializeObject(SimpleCodec.DeCode(res.Result.Value, res.Result.ValueType, typeof(TestServiceResponse)))); 
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