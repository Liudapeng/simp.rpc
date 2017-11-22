using System.Collections.Generic;
using System.Linq;
using DotNetty.Common.Internal.Logging;
using Microsoft.Extensions.Logging;
using Simp.Rpc.Codec;
using Simp.Rpc.Codec.Serializer;
using Simp.Rpc.Service;

namespace Simp.Rpc.Server
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using DotNetty.Buffers;
    using DotNetty.Transport.Channels;
    using Newtonsoft.Json;

    public class SimpleServerHandler : SimpleChannelInboundHandler<SimpleRequestMessage>
    {
        private readonly IInternalLogger Logger = InternalLoggerFactory.GetInstance<SimpleServerHandler>();

        private readonly SimpleServer server;
        private readonly ITypeCodec<SimpleParameter> typeCodec = new SimpleTypeCodec(new ProtoBufSerializer());

        public SimpleServerHandler(IServer server)
        {
            this.server = server as SimpleServer;
        }

        protected override void ChannelRead0(IChannelHandlerContext context, SimpleRequestMessage message)
        {
            Task.Delay(10).Wait();
            if (message != null)
            {
                SimpleResponseMessage simpleResponseMessage = new SimpleResponseMessage
                {
                    ContextID = message.ContextID,
                    MessageID = message.MessageID
                };

                object execRes = null;
                object[] args = null;
                try
                {
                    var executer = this.server.RpcServiceContainer.LookupExecuter(message.ServiceName, message.MethodName);
                    args = typeCodec.Decode(message.Parameters, executer.ArgTypes);
                    execRes = executer.Excute(args);
                    simpleResponseMessage.Success = true;
                }
                catch (Exception e)
                {
                    simpleResponseMessage.Success = false;
                    simpleResponseMessage.ErrorInfo = e.Message;
                    simpleResponseMessage.ErrorDetail = e.StackTrace;
                }

                simpleResponseMessage.Result = new SimpleParameter { Value = typeCodec.EnCode(execRes, out int typeCode), ValueType = typeCode };
                string pre = $"threadId:{Thread.CurrentThread.ManagedThreadId}-{context.Channel.Id.AsShortText()}";
                var log = $"{pre}, Received from client: ";
                log += $"{Environment.NewLine}{pre}, service: {message.ServiceName}";
                log += $"{Environment.NewLine}{pre}, method: {message.MethodName}";
                log += $"{Environment.NewLine}{pre}, params:{JsonConvert.SerializeObject(args)}";
                log += $"{Environment.NewLine}{pre}, Result: {JsonConvert.SerializeObject(execRes)}";
                log += $"{Environment.NewLine}{pre}, Success: {JsonConvert.SerializeObject(simpleResponseMessage.Success)}";
                log += $"{Environment.NewLine}{pre}, ErrorInfo: {JsonConvert.SerializeObject(simpleResponseMessage.ErrorInfo)}";
                log += $"{Environment.NewLine}{pre}, ErrorDetail: {JsonConvert.SerializeObject(simpleResponseMessage.ErrorDetail)}{Environment.NewLine}";
                Logger.Debug(log);
                context.WriteAndFlushAsync(simpleResponseMessage);
            }
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            context.CloseAsync();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Logger.Error("closed exception: " + context.Channel.RemoteAddress);
            context.CloseAsync();
        }
 
    }
}