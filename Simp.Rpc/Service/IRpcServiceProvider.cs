using System.Collections.Generic;
using System.Threading.Tasks; 

namespace Simp.Rpc.Service
{
    public interface IRpcServiceProvider
    {
        Task<IDictionary<string, RpcServiceInfo>> ScanRpcServices();
    }
}