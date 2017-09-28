using ProtoBuf;
using Simp.Rpc.Codec;

namespace Simp.Rpc
{

    [ProtoContract]
    public class SimpleParameter
    {
        [ProtoMember(1)]
        public int ValueType { get; set; }

        [ProtoMember(2)]
        public byte[] Value { get; set; } 
    }
}