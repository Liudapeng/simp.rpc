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
        private readonly SimpleServer server;
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
                    var executer = this.server.rpcServiceContainer.LookupExecuter(message.ServiceName, message.MethodName);
                    args = BuildExecuteArgs(message.Parameters, executer.ArgTypes);
                    execRes = executer.Excute(args); 
                    simpleResponseMessage.Success = true; 
                }
                catch (Exception e)
                {
                    simpleResponseMessage.Success = false;
                    simpleResponseMessage.ErrorInfo = e.Message;
                    simpleResponseMessage.ErrorDetail = e.StackTrace;
                }

                simpleResponseMessage.Result = new SimpleParameter { Value = SimpleCodec.EnCode(execRes, out int typeCode), ValueType = typeCode };
                string pre = $"threadId:{Thread.CurrentThread.ManagedThreadId}{context.Channel.Id.AsShortText()}";
                Console.WriteLine($"{pre}, Received from client: ");
                Console.WriteLine($"{pre}, service: {message.ServiceName}");
                Console.WriteLine($"{pre}, method: {message.MethodName}");
                Console.WriteLine($"{pre}, params:{JsonConvert.SerializeObject(args)}");
                Console.WriteLine($"{pre}, Result: {JsonConvert.SerializeObject(execRes)}");

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
            Console.WriteLine("closed exception: " + context.Channel.Id);
            context.CloseAsync();
        }

        /// <summary>
        /// 
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