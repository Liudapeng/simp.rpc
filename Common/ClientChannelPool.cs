using System.Reflection.Metadata;
using Common.Address;

namespace Common
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using DotNetty.Codecs;
    using DotNetty.Common.Utilities;
    using DotNetty.Transport.Bootstrapping;
    using DotNetty.Transport.Channels;
    using DotNetty.Transport.Channels.Sockets;
    using System.Collections.Generic;

    public class ClientChannelPool
    {
        public ConcurrentDictionary<EndPoint, ConcurrentDictionary<string, IChannel>> EndPointGroupPools { get; } = new ConcurrentDictionary<EndPoint, ConcurrentDictionary<string, IChannel>>(new EndPointComparer());
        private readonly ConcurrentDictionary<EndPoint, ConcurrentQueue<IChannel>> EndPointGroupQueues = new ConcurrentDictionary<EndPoint, ConcurrentQueue<IChannel>>(new EndPointComparer());
        private Bootstrap Bootstrap;
        private static readonly object initLocker = new object();

        private readonly Func<IChannelHandler[]> ChannleHandlersProvider;
        private readonly ClientOptions clientOptions;

        public ClientChannelPool(Func<IChannelHandler[]> channleHandlersProvider, ClientOptions clientOptions)
        {
            this.ChannleHandlersProvider = channleHandlersProvider;
            this.clientOptions = clientOptions;
            this.InitBootstrap();
        }

        void InitBootstrap()
        {
            Bootstrap = new Bootstrap()
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Group(new MultithreadEventLoopGroup());

            Bootstrap.Handler(new ActionChannelInitializer<ISocketChannel>(c =>
            {
                IChannelPipeline pipeline = c.Pipeline;
                IChannelHandler[] channleHandlers = this.ChannleHandlersProvider();
                if (channleHandlers != null && channleHandlers.Any())
                {
                    foreach (IChannelHandler channleHandler in channleHandlers)
                    {
                        pipeline.AddLast(channleHandler);
                    }
                }
            }));
        }

        public string State => $"ClientChannelPool: GroupPool size:{this.EndPointGroupPools.Count}, pool size:{this.EndPointGroupPools.Values.Sum(v => v.Count)}, GroupQueue size:{this.EndPointGroupQueues.Count}, queue size:{this.EndPointGroupQueues.Values.Sum(v => v.Count)}";

        public IChannel Acquire(Func<EndPoint> endPointProvider)
        {
            var endPoint = endPointProvider();
            IChannel channel;
            do
            {
                try
                {
                    ConcurrentQueue<IChannel> queue;
                    if (!this.EndPointGroupQueues.TryGetValue(endPoint, out queue) || queue == null || queue.IsEmpty)
                    {
                        channel = this.New(endPoint);
                        if (channel == null || !channel.Active)
                            throw new ChannelException(string.Format("channel connectAsync error! remoteAddress:{0}", endPoint.ToString()));

                        ConcurrentDictionary<string, IChannel> pool;
                        if (!this.EndPointGroupPools.TryGetValue(endPoint, out pool))
                        {
                            pool = new ConcurrentDictionary<string, IChannel>();
                            if (this.EndPointGroupPools.TryAdd(endPoint, pool))
                            {
                                pool.TryAdd(GetChannelId(channel), channel);
                            }
                        }
                        else
                        {
                            pool.TryAdd(GetChannelId(channel), channel);
                        }
                    }
                    else if (!queue.TryDequeue(out channel))
                    {
                        break;
                    }

                    if (channel.Active)
                        break;
                }
                catch (OperationCanceledException)
                {
                    //TODO 
                    throw;
                }
            } while (true);

#if DEBUG
            Console.WriteLine(this.State);
#endif
            return channel;
        }

        public bool Release(IChannel channel)
        {
            var remoteAddress = channel.RemoteAddress;
            ConcurrentDictionary<string, IChannel> pool;
            IChannel channelInPool;
            if (EndPointGroupPools.TryGetValue(remoteAddress, out pool) && pool.TryGetValue(GetChannelId(channel), out channelInPool))
            {
                if (channel.Active)
                {
                    ConcurrentQueue<IChannel> queue;
                    if (!this.EndPointGroupQueues.TryGetValue(remoteAddress, out queue) || queue == null)
                    {
                        queue = new ConcurrentQueue<IChannel>();
                        this.EndPointGroupQueues.TryAdd(remoteAddress, queue);
                    }
                    queue.Enqueue(channel);
                    return true;
                }
            }
            return false;
        }

        public bool Closed(IChannel channel)
        {
            if (channel.Active)
            {
                channel.CloseAsync();
            }
            ConcurrentDictionary<string, IChannel> pool;
            IChannel channelInPool;
            if (EndPointGroupPools.TryGetValue(channel.RemoteAddress, out pool) && pool.TryGetValue(GetChannelId(channel), out channelInPool))
            {
                return pool.TryRemove(GetChannelId(channel), out IChannel _);
            }
            return false;
        }

        private IChannel New(EndPoint endPoint)
        {
            ConcurrentDictionary<string, IChannel> pool;
            if (EndPointGroupPools.TryGetValue(endPoint, out pool) && pool.Count >= clientOptions.MaxConnections)
                throw new Exception("pool is full");

            IChannel channel = Bootstrap.ConnectAsync(endPoint).Result;
            channel.GetAttribute(AttributeKey<ClientChannelPool>.ValueOf(typeof(ClientChannelPool).Name)).Set(this);
            return channel;
        }

        private string GetChannelId(IChannel channel)
        {
            var localAddress = channel.LocalAddress as IPEndPoint;
            if (localAddress != null)
                return ((IPEndPoint)channel.LocalAddress).Address + ":" + ((IPEndPoint)channel.LocalAddress).Port;
            return channel.LocalAddress.ToString();
        }
    }


}