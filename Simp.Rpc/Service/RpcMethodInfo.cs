using System.Reflection;

namespace Simp.Rpc.Service
{
    public class RpcMethodInfo
    {

        public string Name { get; set; }

        public string Description { get; set; }

        public MethodInfo MethodInfo { get; set; }

        public RpcParameterInfo[] RpcParameters { get; set; }

        public RpcParameterInfo RpcReturnType { get; set; }
    }
}