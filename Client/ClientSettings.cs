using System.Net;
using Simp.Rpc;

namespace Client
{
    public class ClientSettings
    { 
        public static IPAddress Host => IPAddress.Parse(ConfigHelper.Configuration["host"]).MapToIPv6();

        public static int Port => int.Parse(ConfigHelper.Configuration["port"]); 
    }
}