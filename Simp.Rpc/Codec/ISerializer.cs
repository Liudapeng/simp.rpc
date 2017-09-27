using System;

namespace Simp.Rpc.Codec
{
    public interface ISerializer
    { 
        byte[] Serialize(object instance);

        T Deserialize<T>(byte[] data);

        object Deserialize(byte[] data, Type objType);

        byte GetSerializType();
    }
}