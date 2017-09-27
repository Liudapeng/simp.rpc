using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;
using Simp.Rpc.Address;
using DotNetty.Transport.Channels;

namespace Simp.Rpc.Server
{
    public class ServerChannelPool
    {
        public ConcurrentDictionary<string, IChannel> ChannelPool { get; } = new ConcurrentDictionary<string, IChannel>();

        public Task<bool> Add(IChannel channel)
        {
           return Task.Run(() => ChannelPool.TryAdd(channel.Id.AsLongText(), channel));
        }

        public Task Remove(IChannel channel)
        {
            return Task.Run(() => ChannelPool.TryRemove(channel.Id.AsLongText(), out IChannel _));
        } 
    }
}