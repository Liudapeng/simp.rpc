namespace Common.Codec
{
    public interface ISerializer
    { 
        byte[] Serialize(object instance);

        T Deserialize<T>(byte[] data);

        byte GetSerializType();
    }
}