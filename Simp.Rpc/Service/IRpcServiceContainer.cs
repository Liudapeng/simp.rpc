using System.Collections.Generic;

namespace Simp.Rpc.Service
{
    public interface IRpcServiceContainer
    {
        /// <summary>
        /// 查找执行器
        /// </summary>
        /// <param name="service"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        ServiceExcuter LookupExecuter(string service, string method);

        IDictionary<string, RpcServiceInfo> GetRegisterServices();

    }
}