using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Address
{
    public class PollingAddressProvider : IAddressProvider
    {
        private readonly object _lock = new object();
        private int currentIndex;

        public Task<AddressBase> Acquire(IEnumerable<AddressBase> addressCollection)
        {
            return Task.Run(() =>
            {
                var addresses = addressCollection as AddressBase[] ?? addressCollection.ToArray();
                if (addresses.Length == 0)
                    return null;

                lock (_lock)
                {
                    if (currentIndex >= addresses.Length)
                    {
                        currentIndex = 0;
                    }
                    return addresses[currentIndex++];
                }
            });
        }
    }
}