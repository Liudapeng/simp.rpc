using System.Net;
using ProtoBuf;

namespace Simp.Rpc.Address 
{ 
    public class IPPortAddress : AddressBase
    { 
        private EndPoint endPoint;
         
        public string Ip { get; set; }

        public int Port { get; set; }

        public override EndPoint CreateEndPoint()
        {
            return endPoint ?? (endPoint = new IPEndPoint(IPAddress.Parse(Ip).MapToIPv6(), Port));
        }

        public override string ToString()
        {
            return $"{Ip}:{Port}";
        }
    }
}