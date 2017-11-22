using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Simp.Rpc.Codec;
using Simp.Rpc.Codec.Serializer;
using Simp.Rpc.Invoker;
using Simp.Rpc.Service.Attributes;

namespace Simp.Rpc.Client.Proxy
{
    public class InvokeProxy<T> : DispatchProxy
    {
        private static readonly ITypeCodec<SimpleParameter> typeCodec = new SimpleTypeCodec(new ProtoBufSerializer());
        private static readonly SimpleInvokerFactory invokerFactory = new SimpleInvokerFactory();
        private readonly Type type;

        public InvokeProxy()
        {
            type = typeof(T);
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            string serverName = type.GetCustomAttribute<RpcServiceContractAttribute>().Server;

            string serviceName = !string.IsNullOrEmpty(type.GetCustomAttribute<RpcServiceContractAttribute>().Name)
                ? type.GetCustomAttribute<RpcServiceContractAttribute>().Name
                : type.Name;

            SimpleResponseMessage simpleResponseMessage = null;
            var task = Task.Run(async () =>
            {
                var invoker = await invokerFactory.CreateInvokerAsync(serverName);
                simpleResponseMessage = await invoker.InvokeAsync(serviceName, targetMethod.Name, args);
            });

            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    throw t.Exception.GetBaseException();
                }
            }).Wait();

            return typeCodec.Decode(new[] { simpleResponseMessage.Result }, new[] { targetMethod.ReturnType }).FirstOrDefault();
        }

    }
}