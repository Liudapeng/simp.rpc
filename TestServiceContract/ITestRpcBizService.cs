using System.Collections.Generic;
using System.Threading.Tasks;
using Simp.Rpc.Service.Attributes;

namespace TestServiceContract
{
    [RpcServiceContract(server: "TestServer", name: "TestRpcService", Description = "测试服务")]
    public interface ITestRpcService
    {
        [RpcServiceContract]
        int GetServiceCount();
         
        List<TestServiceResponse> ExecuteServiceList(List<TestServiceRequest> requests);

        TestServiceResponse ExecuteService(TestServiceRequest request, TestServiceRequest2 request2, int i);

        void Execute(List<TestServiceRequest> request, TestServiceRequest2 request2);

        [RpcServiceIgnore]
        int TestIgnore();

    }
}