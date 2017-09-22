 
using Common.Codec;

namespace Server
{
    using System;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading.Tasks;
    using DotNetty.Codecs;
    using DotNetty.Handlers.Logging;
    using DotNetty.Handlers.Tls;
    using DotNetty.Transport.Bootstrapping;
    using DotNetty.Transport.Channels;
    using DotNetty.Transport.Channels.Sockets;
    using Common;

    class Program
    {
        static async Task RunServerAsync()
        {
            ConfigHelper.SetConsoleLogger();
            ISerializer serializer = new ProtoBufSerializer();
            var bossGroup = new MultithreadEventLoopGroup(1);
            var workerGroup = new MultithreadEventLoopGroup(); 
            try
            {
                var bootstrap = new ServerBootstrap();
                bootstrap
                    .Group(bossGroup, workerGroup)
                    .Channel<TcpServerSocketChannel>()
                    .Option(ChannelOption.SoBacklog, 100)
                    .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline; 
                        pipeline.AddLast("enc", new SimpleEncoder<SimpleResponseMessage>(serializer));
                        pipeline.AddLast("dec", new SimpleDecoder<SimpleRequestMessage>(serializer)); 
                        pipeline.AddLast("handle", new SimpleServerHandler());
                    }));

                IChannel boundChannel = await bootstrap.BindAsync(ServerSettings.Port);

                Console.ReadLine();

                await boundChannel.CloseAsync();
            }
            finally
            {
                await Task.WhenAll(
                    bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                    workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
            }
        }

        static void Main() => RunServerAsync().Wait();
    }
}