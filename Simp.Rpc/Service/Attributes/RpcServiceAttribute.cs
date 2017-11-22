using System; 
namespace Simp.Rpc.Service.Attributes
{
    /// <summary>
    /// 服务绑定标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
    public class RpcServiceContractAttribute : Attribute
    {
        public string Server { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public RpcServiceContractAttribute()
        {
        }

        public RpcServiceContractAttribute(string server, string name, string description = "")
        {
            Server = server;
            Name = name;
            Description = description;
        }
    }
}