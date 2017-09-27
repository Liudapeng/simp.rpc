using System.Collections.Generic;
using System.Threading.Tasks;
using Simp.Rpc.Address;

namespace Simp.Rpc.Address
{
    public interface IAddressProvider
    {
        Task<AddressBase> AcquireAsync(IEnumerable<AddressBase> addressCollection);
    }
}