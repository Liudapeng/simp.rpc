using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Address
{
    public interface IAddressProvider
    {
        Task<AddressBase> AcquireAsync(IEnumerable<AddressBase> addressCollection);
    }
}