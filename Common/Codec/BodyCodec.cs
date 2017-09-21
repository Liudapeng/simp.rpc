using System;

namespace Common.Codec
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

        public static object DeCode(byte[] bytesValue, int typeCode)
        {
           throw new NotImplementedException();
        }
    }
}