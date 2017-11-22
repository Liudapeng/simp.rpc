using System;
using System.Net;
using System.Threading.Tasks;
using DotNetty.Common.Internal.Logging;
using Simp.Rpc.Codec;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;
using Simp.Rpc.Codec.Serializer;
using Simp.Rpc.Codec.TransportCoder;
using Simp.Rpc.Service; 

namespace Simp.Rpc.Server
{
    public class SimpleServer : IServer
    { 
        public ILogger<SimpleServer> Logger { get; set; } 

        public IRpcServiceContainer RpcServiceContainer { get; }
        private readonly ServerOptions serverOptions;
        private IChannel boundChannel;

        public SimpleServer(IRpcServiceContainer rpcServiceContainer, IServerOptionProvider serverOptionProvider)
        { 
            this.serverOptions = serverOptionProvider.GetOption();
            this.RpcServiceContainer = rpcServiceContainer; 
        }

        public async Task StartAsync()
        {
            await RunServerAsync(this.serverOptions.EndPoint);
        }
          
        async Task RunServerAsync(EndPoint endPoint)
        { 
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
            Logger?.LogInformation($"server start by netty with {boundChannel.LocalAddress}, channelId:{boundChannel.Id}");
        }

    }
}