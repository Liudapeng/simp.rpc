using System.Net;
using Simp.Rpc;

namespace Client
{
    public static class ServerSettings
    {
        public static IPAddress Host => IPAddress.Parse(ConfigHelper.Configuration["host"]).MapToIPv6();

        public static int Port => int.Parse(ConfigHelper.Configuration["port"]);
    }
}