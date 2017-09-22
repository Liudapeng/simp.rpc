namespace Common
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
        public SimpleServerHandler()
        {
            //Console.WriteLine("create");
        }

        protected override void ChannelRead0(IChannelHandlerContext context, SimpleRequestMessage message)
        {
            Console.WriteLine($"theaterId:{Thread.CurrentThread.ManagedThreadId}");
            Task.Delay(100).Wait();
            if (message != null)
            {
                Console.WriteLine($"theaterId:{Thread.CurrentThread.ManagedThreadId},Received from client: " + JsonConvert.SerializeObject(message));

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