using System.Threading.Tasks;
using DotNetty.Common.Utilities;
using Simp.Rpc.Invoker;

namespace Simp.Rpc.Client
{
    using System;
    using System.Text;
    using DotNetty.Buffers; 
    using DotNetty.Transport.Channels;
    using Common;

    public class SimpleClientHandler : SimpleChannelInboundHandler<SimpleResponseMessage>
    {
        private readonly Invoker<SimpleResponseMessage> invoker;
        public SimpleClientHandler(Invoker<SimpleResponseMessage> invoker)
        {
            this.invoker = invoker;
        }

        protected override void ChannelRead0(IChannelHandlerContext contex, SimpleResponseMessage msg)
        {  
            invoker.GetResponse(contex, msg);
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        { 
            GetPool(context).Release(context.Channel);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        { 
            GetPool(context).Closed(context.Channel);
            base.ChannelInactive(context);
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        { 
            GetPool(context).Closed(context.Channel); 
            context.CloseAsync();
            Console.WriteLine("Exception: " + exception);
        }

        private ClientChannelPool GetPool(IChannelHandlerContext context)
        {
            return context.Channel.GetAttribute(AttributeKey<ClientChannelPool>.ValueOf(typeof(ClientChannelPool).Name)).Get();
        }
    }
}