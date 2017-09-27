using System;

namespace Simp.Rpc.Service.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RpcServiceIgnoreAttribute : Attribute
    {

    }
}