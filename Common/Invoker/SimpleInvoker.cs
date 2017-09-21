using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Common.Codec;
using Common.Invoker;
using Common.Route;
using DotNetty.Transport.Channels;
using Common.Address;

namespace Common.Invoker
{
    public class SimpleInvoker : Invoker<SimpleResponseMessage>
    {
        //private static readonly ConcurrentDictionary<ServerDescription, ClientChannelPool> clientChannelPoolMap = new ConcurrentDictionary<ServerDescription, ClientChannelPool>(new ServerDescriptionComparer());
        private readonly ConcurrentDictionary<string, TaskCompletionSource<SimpleResponseMessage>> invokeResult = new ConcurrentDictionary<string, TaskCompletionSource<SimpleResponseMessage>>();

        private ClientChannelPool currentClientChannelPool;

        private static readonly string clientName;
        private readonly string serverName;
        private readonly string group;

        private readonly IServerRouteManager serverRouteManager;
        private readonly ISerializer serializer = new ProtoBufSerializer();

        static SimpleInvoker()
        {
            clientName = "SimpleInvoker";
        }

        public SimpleInvoker(IServerRouteManager serverRouteManager, string serverName, string group = "")
        {
            this.serverRouteManager = serverRouteManager;
            this.serverName = serverName;
            this.group = group;

            InitChannel();
        }

        private void InitChannel()
        {
            ServerDescription server = this.serverRouteManager.GetServerRouteAsync(this.serverName, this.group).Result;
            currentClientChannelPool = new ClientChannelPool(
                    () => new IChannelHandler[]
                    {
                        new SimpleEncoder<SimpleRequestMessage>(serializer),
                        new SimpleDecoder<SimpleResponseMessage>(serializer),
                        new SimpleClientHandler(this)
                    },
                    server.ClientOptions
                );
            //if (!clientChannelPoolMap.TryGetValue(server, out currentClientChannelPool))
            //{
            //    currentClientChannelPool = new ClientChannelPool(
            //        () => new IChannelHandler[]
            //        {
            //            new SimpleEncoder<SimpleRequestMessage>(serializer),
            //            new SimpleDecoder<SimpleResponseMessage>(serializer),
            //            new SimpleClientHandler(this)
            //        },
            //        server.ClientOptions
            //    );
            //    clientChannelPoolMap.TryAdd(server, currentClientChannelPool);
            //}
        }

        private SimpleRequestMessage CreateRequestMessage(string service, string method, List<object> args)
        {
            SimpleRequestMessage requestMessage = new SimpleRequestMessage
            {
                ClientName = clientName,
                ServerName = serverName,
                ServiceName = service,
                MethodName = method,
                MessageID = Guid.NewGuid().ToString(),
            };
            if (args != null && args.Any())
            {
                requestMessage.Parameters = args.Select(arg =>
                {
                    int codeType = 0;
                    var simpleParameter = new SimpleParameter{Value = BodyCodec.EnCode(arg, out codeType) , ValueType = codeType };
                    return simpleParameter;
                }).ToArray();
            }
            return requestMessage;
        }
         
        public async Task<SimpleResponseMessage> InvokeAsync(string service, string method, List<object> args)
        {
            AddressBase address = this.serverRouteManager.GetAddressAsync(serverName, group).Result;
            if (address == null)
                throw new Exception("address not found");

            IChannel clientChannel = currentClientChannelPool.Acquire(address.CreateEndPoint);

            var requestMessage = CreateRequestMessage(service, method, args);
            requestMessage.ContextID = clientChannel.Id.AsShortText();

            var tcs = new TaskCompletionSource<SimpleResponseMessage>();
            invokeResult.TryAdd(requestMessage.MessageID, tcs);
            clientChannel.WriteAndFlushAsync(requestMessage); 
            return await tcs.Task ; 
        }


        public Task<SimpleResponseMessage> GetResponse(IChannelHandlerContext contex, SimpleResponseMessage msg)
        {
            TaskCompletionSource<SimpleResponseMessage> tcs;
            if (!invokeResult.TryGetValue(msg.MessageID, out tcs)) 
                throw new Exception("获取上下文失败"); 

            tcs.SetResult(msg);
            return tcs.Task;
        }
    }
}