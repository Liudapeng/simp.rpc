using System.Net;
using Simp.Rpc.Util;

namespace Simp.Rpc.Server
{
    public class SimpleServerOptionProvider : IServerOptionProvider
    {
        public ServerOptions GetOption()
        {
            return new ServerOptions { EndPoint = new IPEndPoint(ServerSettings.Host, ServerSettings.Port) };
        }
    }
}