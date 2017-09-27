using System;
using System.IO;
using ProtoBuf;

namespace Simp.Rpc.Codec
{
    public class ProtoBufSerializer: ISerializer
    {
        public byte[] Serialize(object instance)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, instance);
                return stream.ToArray();
            }
        }

        public T Deserialize<T>(byte[] data)
        {
            if (data == null)
                return default(T);

            using (var stream = new MemoryStream(data))
            {
                return Serializer.Deserialize<T>(stream);
            }
        }


        public object Deserialize(byte[] data, Type objType)
        {
            if (data == null)
                return null;

            using (var stream = new MemoryStream(data))
            {
                return Serializer.Deserialize(objType, stream);
            }
        }

        public byte GetSerializType()
        {
            return 1;
        }
    }
}