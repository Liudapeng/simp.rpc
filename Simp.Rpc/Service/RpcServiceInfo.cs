using System;
using System.Collections.Generic;

namespace Simp.Rpc.Service
{
    public class RpcServiceInfo
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Type ServiceType { get; set; }

        public Type ImplServiceType { get; set; }

        public bool IsImpl { get; set; }
          
        public Dictionary<string, RpcMethodInfo> Methods { get; set; }

        public override string ToString()
        {
            return $"[{Name},{Description}]";
        }
    }
}