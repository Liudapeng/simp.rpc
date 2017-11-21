using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Simp.Rpc.Invoker;
using Newtonsoft.Json;
using ProtoBuf;
using Simp.Rpc;
using Simp.Rpc.Codec;
using Simp.Rpc.Service.Attributes;
using Simp.Rpc.Util;
using TestServiceContract;

namespace Client
{
    internal class Program
    {
        private static void RunClientAsync()
        {

            var invokerFactory = new SimpleInvokerFactory();

            do
            {
                try
                {
                    Task[] tasks = new Task[5];
                    for (int j = 0; j < tasks.Length; j++)
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
        private static void RunClientByProxyAsync()
        {
            var invokerFactory = new SimpleInvokerFactory();
            do
            {
                try
                {
                    Task task = Task.Run(async () =>
                    {
                        var response = (await invokerFactory.CreateInvokerAsync("Server1")).InvokeAsync("TestRpcService", "ExecuteService", new List<object> { new TestServiceRequest(), new TestServiceRequest2(), 10000 });

                        var res = await response;

                        Console.WriteLine("response:" + JsonConvert.SerializeObject(SimpleCodec.DeCode(res.Result.Value, res.Result.ValueType, typeof(TestServiceResponse))));
                    });

                    task.ContinueWith(t =>
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
            //RunClientAsync();
            //RunClientByProxyAsync();

            ServiceCollection serviceCollection = new ServiceCollection(); 
            serviceCollection.AddSingleton(InvokeSerice.Proxy<ITestRpcService>());

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            var res1 = serviceProvider.GetService<ITestRpcService>().GetServiceCount();
            var res2 = serviceProvider.GetService<ITestRpcService>().ExecuteService(new TestServiceRequest(), new TestServiceRequest2(), 11111);
             
            Console.ReadLine();
        }

    }

    public class InvokeSerice
    {
        public static T Proxy<T>()
        {
            return DispatchProxy.Create<T, InvokeProxy<T>>();
        }

    }

    public class InvokeProxy<T> : DispatchProxy
    {
        readonly SimpleInvokerFactory invokerFactory = new SimpleInvokerFactory();
        private readonly Type type;

        public InvokeProxy()
        {
            type = typeof(T);
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            Console.WriteLine("Invoke 远程服务调用！");

            var task = Task.Run(async () =>
            {
                var response = (await invokerFactory.CreateInvokerAsync("Server1"))
                .InvokeAsync(
                    !string.IsNullOrEmpty(type.GetCustomAttribute<RpcServiceContractAttribute>().Name) ? type.GetCustomAttribute<RpcServiceContractAttribute>().Name : type.Name,
                    targetMethod.Name,
                    args);

                return await response;
            });

            task.ContinueWith(t =>
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

            return BuildExecuteArgs(new[] { task.Result.Result }, new[] { targetMethod.ReturnType }).FirstOrDefault();
        }

        /// <summary>
        /// 根据本地参数类型解码请求参数
        /// </summary>
        /// <param name="encodeParameters"></param>
        /// <param name="decodeTypes"></param>
        /// <returns></returns>
        private object[] BuildExecuteArgs(SimpleParameter[] encodeParameters, Type[] decodeTypes)
        {
            object[] args = { };
            if (encodeParameters != null && encodeParameters.Any())
            {
                if (encodeParameters.Length != decodeTypes.Length)
                    throw new Exception("参数个数不匹配");

                args = new object[encodeParameters.Length];
                for (int i = 0; i < encodeParameters.Length; i++)
                {
                    args[i] = SimpleCodec.DeCode(encodeParameters[i].Value, encodeParameters[i].ValueType, decodeTypes[i]);
                }
            }
            return args;
        }
    }


}