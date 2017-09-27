using Simp.Rpc.Service.Attributes;
using TestServiceContract;

namespace Server
{
    public class TestRpcBizService : ITestRpcService
    {
        public TestServiceResponse ExecuteService(TestServiceRequest request, TestServiceRequest2 request2, int i)
        {
            return new TestServiceResponse
            {
                FloatValue = request.FloatValue + request2.FloatValue + i,
                IntValue = request.IntValue + request2.IntValue,
                StringValue = request.StringValue + request2.StringValue
            };
        }

        public int TestIgnore()
        {
            throw new System.NotImplementedException();
        }

        public int GetServiceCount()
        {
            return 1;
        }

    }
}