using Simp.Rpc.Service.Attributes;

namespace TestServiceContract
{
    [RpcServiceContract(name: "TestRpcService", Description = "测试服务")]
    public interface ITestRpcService
    {
        [RpcServiceContract]
        int GetServiceCount();
         
        TestServiceResponse ExecuteService(TestServiceRequest request, TestServiceRequest2 request2);

        [RpcServiceIgnore]
        int TestIgnore();
    }
}