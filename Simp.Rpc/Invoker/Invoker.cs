using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;

namespace Simp.Rpc.Invoker
{
    public interface Invoker<TResponse>
    {
        Task<TResponse> InvokeAsync(string service, string method, List<object> args);

        Task<TResponse> GetResponse(IChannelHandlerContext contex, TResponse msg);
    }
}