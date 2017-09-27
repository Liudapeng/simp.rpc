using System;

namespace Simp.Rpc.Codec
{
    public class BodyCodec
    {
        private static readonly ISerializer serializer = new ProtoBufSerializer();

        public static byte[] EnCode(object objectValue, out int typeCode)
        {
            if (objectValue == null)
            {
                typeCode = (int) TypeCode.DBNull;
                return new byte[] {0};
            }
            else
            {
                var valueType = objectValue.GetType();
                typeCode = (int) Type.GetTypeCode(valueType);
                return serializer.Serialize(objectValue);
            }
        }

        public static object DeCode(byte[] bytesValue, int typeCode,Type objType)
        {
            if (typeCode == (int) TypeCode.DBNull)
            {
                return null;
            }
            else if (typeCode == (int)TypeCode.Object)
            {
                return serializer.Deserialize(bytesValue, objType);
            }
            return serializer.Deserialize(bytesValue, Type.GetType("System." + Enum.GetName(typeof(TypeCode), typeCode)));
        }
    }
}