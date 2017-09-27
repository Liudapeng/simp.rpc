using System.Collections.Generic;
using Simp.Rpc.Address;
using Simp.Rpc.Client;

namespace Simp.Rpc.Route
{
    public class ServerDescription
    {
        public string Name { get; set; }

        public string Group { get; set; } = string.Empty;

        public bool Discovery { get; set; }

        public string Balance { get; set; }

        public ClientOptions ClientOptions { get; set; }

        public IEnumerable<AddressBase> AddressList { get; set; }
    }

    public class ServerDescriptionComparer : IEqualityComparer<ServerDescription>
    {
        public bool Equals(ServerDescription x, ServerDescription y)
        {
            if (x == null) return false;
            if (y == null) return false;
            return x.Name == y.Name && x.Group == y.Group;
        }

        public int GetHashCode(ServerDescription obj)
        {
            return $"Group{obj.Group}Name{obj.Name}".GetHashCode();
        }
    }
}