using System.IO;
using ProtoBuf;

namespace Common.Codec
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

        public byte GetSerializType()
        {
            return 1;
        }
    }
}