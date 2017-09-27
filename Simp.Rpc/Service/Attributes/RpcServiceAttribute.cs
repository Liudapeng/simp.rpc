using System; 
namespace Simp.Rpc.Service.Attributes
{
    /// <summary>
    /// 服务绑定标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
    public class RpcServiceContractAttribute : Attribute
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public RpcServiceContractAttribute()
        {
        }

        public RpcServiceContractAttribute(string name, string description = "")
        {
            Name = name;
            Description = description;
        }
    }
}