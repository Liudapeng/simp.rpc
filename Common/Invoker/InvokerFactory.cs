using System.Collections.Concurrent;
using System.Threading.Tasks;
using Common.Invoker;
using Common.Route;

namespace Common.Invoker
{
    public interface InvokerFactory<T>
    {
        Task<Invoker<T>> CreateInvokerAsync(string serverName, string @group = "");
    }
}