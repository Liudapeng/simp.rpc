using System.Collections.Generic;
using System.Threading.Tasks;
using Simp.Rpc.Service.Attributes;
using TestServiceContract;

namespace Server
{
    public class TestRpcBizService : ITestRpcService
    {
        public List<TestServiceResponse> ExecuteServiceList(List<TestServiceRequest> requests)
        {
            var response = new List<TestServiceResponse>();

            foreach (var request in requests)
            {
                response.Add(new TestServiceResponse
                {
                    FloatValue = request.FloatValue,
                    IntValue = request.IntValue ,
                    StringValue = request.StringValue 
                });
            }
            return response; 
        }

        public TestServiceResponse ExecuteService(TestServiceRequest request, TestServiceRequest2 request2, int i)
        {
            return new TestServiceResponse
            {
                FloatValue = request.FloatValue + request2.FloatValue + i,
                IntValue = request.IntValue + request2.IntValue,
                StringValue = request.StringValue + request2.StringValue
            };
        }

        public void Execute(List<TestServiceRequest> request, TestServiceRequest2 request2)
        {
             
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