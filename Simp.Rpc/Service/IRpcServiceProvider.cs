using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Simp.Rpc.Service
{
    public interface IRpcServiceProvider
    {
        /// <summary>
        ///  
        /// </summary>
        /// <returns></returns>
        IDictionary<string, RpcServiceInfo> ScanRpcServices();
    }
}