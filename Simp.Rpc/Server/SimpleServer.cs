using System;
using System.Net;
using System.Threading.Tasks;
using Common;
using Simp.Rpc.Client;
using Simp.Rpc.Codec;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Simp.Rpc.Service;

namespace Simp.Rpc.Server
{
    public class SimpleServer : IServer
    {
        public readonly IRpcServiceContainer rpcServiceContainer;
        private readonly ServerOptions serverOptions;
        private IChannel boundChannel;

        public SimpleServer(IRpcServiceContainer rpcServiceContainer, ServerOptions serverOptions)
        {
            this.serverOptions = serverOptions;
            this.rpcServiceContainer = rpcServiceContainer;
        }

        public async Task StartAsync()
        {
            await RunServerAsync(this.serverOptions.EndPoint);
        }

        async Task RunServerAsync(EndPoint endPoint)
        {
            ConfigHelper.SetConsoleLogger();
            ISerializer serializer = new ProtoBufSerializer();
            var bossGroup = new MultithreadEventLoopGroup(1);
            var workerGroup = new MultithreadEventLoopGroup();
             
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
                    pipeline.AddLast("handle", new SimpleServerHandler(this));
                }));

            boundChannel = await bootstrap.BindAsync(endPoint);
            Console.WriteLine($"Server Start by {boundChannel.LocalAddress},{boundChannel.Id}");
        }

    }
}