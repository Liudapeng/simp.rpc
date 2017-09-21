using System.Collections.Concurrent;
using Common.Invoker;
using Common.Route;

namespace Common.Invoker
{
    public interface InvokerFactory<T>
    {
        Invoker<T> CreateInvoker(string serverName,string group="");
    }
}