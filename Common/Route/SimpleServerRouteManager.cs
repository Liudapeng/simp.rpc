using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Common.Address;
using DotNetty.Common.Internal;

namespace Common.Route
{
    public class SimpleServerRouteManager : IServerRouteManager
    {
        protected List<ServerDescription> ServerRouteList = new List<ServerDescription>();

        public SimpleServerRouteManager()
        {
            init();
        }

        public void init()
        {
            AddRouteAsync(new ServerDescription
            {
                Name = "Server1",
                Balance = "polling",
                AddressList = new List<AddressBase>(new List<AddressBase>
                {
                    new IPPortAddress {Ip = "127.0.0.1", Port = 8007}
                }).AsEnumerable(),
                ClientOptions = new ClientOptions()
            }).Wait();
        }
        public Task<IEnumerable<ServerDescription>> GetRoutesAsync()
        {
            return new Task<IEnumerable<ServerDescription>>(() => ServerRouteList);
        }

        public Task AddRouteAsync(ServerDescription route)
        { 
            return Task.Run(() =>
             {
                 if (!ServerRouteList.Contains(route))
                 {
                     ServerRouteList.Add(route);
                 }
             });
        }

        public Task UpdateRouteTask(ServerDescription route)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRouteAsync(ServerDescription route)
        {
            throw new NotImplementedException();
        }
         
        public Task<ServerDescription> GetServerRouteAsync(string serverName, string @group = "")
        {
            return Task.Run(() =>
            {
                ServerDescription route = ServerRouteList.Find(server => server.Group == group && server.Name == serverName); 
                return route;
            });
        }

        public Task ClearAsync()
        {
            throw new NotImplementedException();
        }
    }
}