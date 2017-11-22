using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;

namespace Simp.Rpc.Invoker
{
    /// <summary>
    /// 远程过程调用
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public interface Invoker<TResponse>
    {
        /// <summary>
        /// 远程调用
        /// </summary>
        /// <param name="service"></param>
        /// <param name="method"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        Task<TResponse> InvokeAsync(string service, string method, params object[] arg);

        /// <summary>
        /// 获取相应
        /// </summary>
        /// <param name="contex"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        Task<TResponse> GetResponse(IChannelHandlerContext contex, TResponse msg);
    }
}