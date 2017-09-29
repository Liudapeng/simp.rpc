using System.Net;

namespace Simp.Rpc.Util
{
    public class ClientSettings
    { 
        public static IPAddress Host => IPAddress.Parse(ConfigHelper.Configuration["host"]).MapToIPv6();

        public static int Port => int.Parse(ConfigHelper.Configuration["port"]); 
    }
}