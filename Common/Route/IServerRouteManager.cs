using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Address;

namespace Common.Route
{
    public interface IServerRouteManager
    {
        Task<IEnumerable<ServerDescription>> GetRoutesAsync();

        Task AddRouteAsync(ServerDescription route);

        Task UpdateRouteTask(ServerDescription route);

        Task RemoveRouteAsync(ServerDescription route);

        /// <summary>
        /// 获取一个请求地址
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        Task<AddressBase> GetAddressAsync(string serverName, string group = "");

        /// <summary>
        /// 获取一个服务路由
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        Task<ServerDescription> GetServerRouteAsync(string serverName, string group = "");

        Task ClearAsync();
    }
}