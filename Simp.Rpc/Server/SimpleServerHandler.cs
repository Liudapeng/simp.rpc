using System.Collections.Generic;
using System.Linq;
using Simp.Rpc.Codec;
using Simp.Rpc.Service;

namespace Simp.Rpc.Server
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using DotNetty.Buffers;
    using DotNetty.Transport.Channels;
    using Common;
    using Newtonsoft.Json;

    public class SimpleServerHandler : SimpleChannelInboundHandler<SimpleRequestMessage>
    {
        private SimpleServer server;
        public SimpleServerHandler(IServer server)
        {
            this.server = server as SimpleServer;
        }

        protected override void ChannelRead0(IChannelHandlerContext context, SimpleRequestMessage message)
        {
            Console.WriteLine($"threadId:{Thread.CurrentThread.ManagedThreadId}");
            Task.Delay(100).Wait();
            if (message != null)
            {
                var executer = this.server.rpcServiceContainer.FindExecuter("TestRpcService", "ExecuteService");
                RpcParameterInfo[] rpcParameterInfos = { };
                if (message.Parameters != null && message.Parameters.Any())
                {
                    rpcParameterInfos = new RpcParameterInfo[message.Parameters.Length];
                    for (int i = 0; i < message.Parameters.Length; i++)
                    {
                        rpcParameterInfos[i] = new RpcParameterInfo
                        {
                            Value = BodyCodec.DeCode(message.Parameters[i].Value, message.Parameters[i].ValueType, executer.RpcParameters[i].Type)
                        };
                    }
                }
                
                var res = this.server.rpcServiceContainer.Execute("TestRpcService", "ExecuteService", rpcParameterInfos);
                 
                Console.WriteLine($"threadId:{Thread.CurrentThread.ManagedThreadId}, Received from client: " + JsonConvert.SerializeObject(message));
                Console.WriteLine($"threadId:{Thread.CurrentThread.ManagedThreadId}, Result: " + res);

                context.WriteAndFlushAsync(new SimpleResponseMessage { MessageID = message.MessageID });
            }
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();
         
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            context.CloseAsync();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("closed exception: " + context.Channel.Id);
            context.CloseAsync();
        }

    }
}