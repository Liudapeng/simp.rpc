using System.Collections.Generic;
using System.Net;

namespace Simp.Rpc.Address
{
    public abstract class AddressBase 
    { 
        public abstract EndPoint CreateEndPoint(); 
    }

    public class EndPointComparer : IEqualityComparer<EndPoint>
    {
        public bool Equals(EndPoint x, EndPoint y)
        {
            return GetEndPointId(x) == GetEndPointId(y);
        }

        public int GetHashCode(EndPoint obj)
        {
            return GetEndPointId(obj).GetHashCode();
        }

        private string GetEndPointId(EndPoint endPoint)
        {
            var ep = endPoint as IPEndPoint;
            if (ep != null)
            {
                return ep.Address + ":" + ep.Port;
            }
            return endPoint.ToString();
        }
    }

}