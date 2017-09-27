using System.Net;
using System.Threading.Tasks;

namespace Simp.Rpc.Server
{
    public interface IServer
    {
        Task StartAsync(); 
    }
}