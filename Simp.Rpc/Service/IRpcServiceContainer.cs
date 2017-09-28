namespace Simp.Rpc.Service
{
    public interface IRpcServiceContainer
    {
        //void BuildRpcService();

        ServiceExcuter LookupExecuter(string service, string method);
    }
}