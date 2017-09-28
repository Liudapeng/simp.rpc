using System.Collections.Generic;
using System.Threading;
using DotNetty.Common.Utilities;
using Simp.Rpc.Address;
using Simp.Rpc.Client;

namespace Simp.Rpc.Client
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using DotNetty.Transport.Bootstrapping;
    using DotNetty.Transport.Channels;
    using DotNetty.Transport.Channels.Sockets;

    public class ClientChannelPool
    {
        public IDictionary<EndPoint, ConcurrentDictionary<string, IChannel>> EndPointGroupPools { get; } = new ConcurrentDictionary<EndPoint, ConcurrentDictionary<string, IChannel>>(new EndPointComparer());
        private readonly IDictionary<EndPoint, ConcurrentQueue<IChannel>> EndPointGroupQueues = new ConcurrentDictionary<EndPoint, ConcurrentQueue<IChannel>>(new EndPointComparer());
        private Bootstrap Bootstrap;
        private static readonly object newChannelLocker = new object();
        private static readonly object releaseChannelLocker = new object();

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
                .Option(ChannelOption.ConnectTimeout, new TimeSpan(0, 0, 0, 0, clientOptions.ConnectTimeout))
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

        public string State => $"GroupPool count:{this.EndPointGroupPools.Count}, pool size:{this.EndPointGroupPools.Values.Sum(v => v.Count(m => m.Value != null))}, GroupQueue count:{this.EndPointGroupQueues.Count}, idle size:{this.EndPointGroupQueues.Values.Sum(v => v.Count)}";

        public async Task<IChannel> AcquireAsync(Func<EndPoint> endPointProvider)
        {
            var endPoint = endPointProvider();
            IChannel channel;
            do
            {
                ConcurrentQueue<IChannel> queue;
                if (!this.EndPointGroupQueues.TryGetValue(endPoint, out queue) || queue == null || queue.IsEmpty)
                {
                    channel = await this.NewAsync(endPoint);
                    if (channel == null || !channel.Active)
                        throw new ChannelException(string.Format("channel connectAsync error! remoteAddress:{0}", endPoint.ToString()));

                }
                else if (!queue.TryDequeue(out channel))
                {
                    break;
                }

                if (channel.Active)
                    break;
            } while (true);

#if DEBUG
            Console.WriteLine($"AcquireAsync: {this.State}");
#endif
            return channel;
        }

        public bool Release(IChannel channel)
        {
            var remoteAddress = channel.RemoteAddress;
            ConcurrentDictionary<string, IChannel> pool;
            IChannel channelInPool;
            if (this.EndPointGroupPools.TryGetValue(remoteAddress, out pool) && pool != null && pool.TryGetValue(GetChannelId(channel), out channelInPool))
            {
                if (channel.Active)
                {
                    ConcurrentQueue<IChannel> queue;
                    if (this.EndPointGroupQueues.TryGetValue(remoteAddress, out queue))
                    {
                        queue.Enqueue(channel);
#if DEBUG
                        Console.WriteLine($"Release1: {this.State}");
#endif
                        return true;
                    }

                    lock (releaseChannelLocker)
                    {
                        if (this.EndPointGroupQueues.TryGetValue(remoteAddress, out queue))
                        {
                            queue.Enqueue(channel);
#if DEBUG
                            Console.WriteLine($"Release2: {this.State}");
#endif
                            return true;
                        }

                        queue = new ConcurrentQueue<IChannel>();
                        this.EndPointGroupQueues.TryAdd(remoteAddress, queue);
                        queue.Enqueue(channel);
#if DEBUG
                        Console.WriteLine($"Release3: {this.State}");
#endif
                        return true;
                    }  
                }
            }
#if DEBUG
            Console.WriteLine($"Release4: {this.State}");
#endif
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
            if (this.EndPointGroupPools.TryGetValue(channel.RemoteAddress, out pool) && pool != null && pool.TryGetValue(GetChannelId(channel), out channelInPool))
            {
                return pool.TryRemove(GetChannelId(channel), out IChannel _);
            }
            return false;
        }

        private async Task<IChannel> NewAsync(EndPoint endPoint)
        {
            ConcurrentDictionary<string, IChannel> pool;
            string tempChannelId = Guid.NewGuid().ToString() + DateTime.Now.Ticks;

            lock (newChannelLocker)
            {
                Console.WriteLine(this.GetHashCode());
                bool res = this.EndPointGroupPools.TryGetValue(endPoint, out pool);
                if (!res)
                {
                    pool = new ConcurrentDictionary<string, IChannel>();
                    this.EndPointGroupPools.TryAdd(endPoint, pool);
                }
                if (pool.TryAdd(tempChannelId, null))
                {
                    Console.WriteLine(true);
                }

                if (pool.Count > clientOptions.MaxConnections)
                {
                    pool.TryRemove(tempChannelId, out IChannel _);
                    throw new Exception($"pool is full: Max:{clientOptions.MaxConnections}, Current:{pool.Count}");
                }
            }

            IChannel channel = await Bootstrap.ConnectAsync(endPoint);
            if (channel != null && channel.Active)
            {
                channel.GetAttribute(AttributeKey<ClientChannelPool>.ValueOf(typeof(ClientChannelPool).Name)).Set(this);
                pool.TryAdd(GetChannelId(channel), channel);
            }

            pool.TryRemove(tempChannelId, out IChannel _);
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