using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;

namespace Simp.Rpc.Invoker
{
    public interface Invoker<TResponse>
    {
        Task<TResponse> InvokeAsync(string service, string method, params object[] arg);

        Task<TResponse> GetResponse(IChannelHandlerContext contex, TResponse msg);
    }
}