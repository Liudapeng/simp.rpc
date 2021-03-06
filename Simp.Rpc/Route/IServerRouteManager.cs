﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Simp.Rpc.Address;

namespace Simp.Rpc.Route
{
    public interface IServerRouteManager
    {
        Task<IEnumerable<ServerDescription>> GetRoutesAsync();

        Task AddRouteAsync(ServerDescription route);

        Task UpdateRouteTask(ServerDescription route);

        Task RemoveRouteAsync(ServerDescription route);
         
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