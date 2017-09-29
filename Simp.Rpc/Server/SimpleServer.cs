using System;
using System.Net;
using System.Threading.Tasks;
using Common;
using DotNetty.Common.Internal.Logging;
using Simp.Rpc.Client;
using Simp.Rpc.Codec;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;
using Simp.Rpc.Service;
using Simp.Rpc.Util;

namespace Simp.Rpc.Server
{
    public class SimpleServer : IServer
    {
        private readonly ILogger<SimpleServer> logger;

        public IRpcServiceContainer RpcServiceContainer { get; }
        private readonly ServerOptions serverOptions;
        private IChannel boundChannel;

        public SimpleServer(IRpcServiceContainer rpcServiceContainer, IServerOptionProvider serverOptionProvider, ILogger<SimpleServer> logger)
        {
            this.logger = logger;
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
            logger.LogInformation($"server start by netty with {boundChannel.LocalAddress}, channelId:{boundChannel.Id}");
        }

    }
}