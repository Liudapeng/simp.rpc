using System.Collections.Concurrent;
using System.Threading.Tasks;
using Simp.Rpc.Invoker;
using Simp.Rpc.Route;

namespace Simp.Rpc.Invoker
{
    public interface InvokerFactory<T>
    {
        Task<Invoker<T>> CreateInvokerAsync(string serverName, string @group = "");
    }
}