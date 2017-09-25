using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading;
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
        private readonly ConcurrentDictionary<string, TaskCompletionSource<SimpleResponseMessage>> invokeResult = new ConcurrentDictionary<string, TaskCompletionSource<SimpleResponseMessage>>();

        private ClientChannelPool currentClientChannelPool;
        private ServerDescription server;

        private static readonly string clientName;
        private readonly string serverName;
        private readonly string group;

        private readonly IServerRouteManager serverRouteManager;
        private readonly IAddressProvider addressProvider;
        private readonly ISerializer serializer = new ProtoBufSerializer();

        static SimpleInvoker()
        {
            clientName = "SimpleInvoker";
        }

        public SimpleInvoker(IServerRouteManager serverRouteManager,IAddressProvider addressProvider, string serverName, string group = "")
        {
            this.serverRouteManager = serverRouteManager;
            this.addressProvider = addressProvider;
            this.serverName = serverName;
            this.group = group;

           Task.Run(async () => { await InitChannel(); }).Wait();
        }

        private async Task InitChannel()
        {
            server = await this.serverRouteManager.GetServerRouteAsync(this.serverName, this.group);
            currentClientChannelPool = new ClientChannelPool(
                    () => new IChannelHandler[]
                    {
                        new SimpleEncoder<SimpleRequestMessage>(serializer),
                        new SimpleDecoder<SimpleResponseMessage>(serializer),
                        new SimpleClientHandler(this)
                    },
                    server.ClientOptions
                ); 
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
            AddressBase address = await this.addressProvider.AcquireAsync(this.server.AddressList); 
            if (address == null)
                throw new Exception("address not found");

            Task<IChannel> clientChannelTask = currentClientChannelPool.AcquireAsync(address.CreateEndPoint);

            var requestMessage = CreateRequestMessage(service, method, args);

            IChannel clientChannel = await clientChannelTask;
            requestMessage.ContextID = clientChannel.Id.AsShortText();

            //响应结果接收task
            var tcs = new TaskCompletionSource<SimpleResponseMessage>(); 
            invokeResult.TryAdd(requestMessage.MessageID, tcs);

            clientChannel.WriteAndFlushAsync(requestMessage);

            //请求超时
            //var cts_request = new CancellationTokenSource();
            //Task requestTask = clientChannel.WriteAndFlushAsync(requestMessage); 
            //if (!requestTask.Wait(server.ClientOptions.WriteTimeout, cts_request.Token))
            //{
            //    invokeResult.TryRemove(requestMessage.ContextID, out TaskCompletionSource<SimpleResponseMessage> _);
            //    cts_request.Cancel();
            //    throw new Exception("request timeout: MessageId: {requestMessage.MessageID}");
            //}

            //响应超时
            var cts_response = new CancellationTokenSource();
            if (!tcs.Task.Wait(server.ClientOptions.ReadTimeout, cts_response.Token))
            {
                invokeResult.TryRemove(requestMessage.MessageID, out TaskCompletionSource<SimpleResponseMessage> _);
                cts_response.Cancel();
                throw new Exception($"response timeout: MessageId: {requestMessage.MessageID}");
            }

            var result = await tcs.Task; 
            invokeResult.TryRemove(requestMessage.MessageID, out TaskCompletionSource<SimpleResponseMessage> _);
            return result;
        }


        public Task<SimpleResponseMessage> GetResponse(IChannelHandlerContext contex, SimpleResponseMessage msg)
        {
            TaskCompletionSource<SimpleResponseMessage> tcs;
            if (!invokeResult.TryGetValue(msg.MessageID, out tcs)) 
                throw new Exception("find request context error"); 

            tcs.SetResult(msg);
            return tcs.Task;
        }
    }
}