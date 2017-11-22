using System;
using System.Linq;
using Simp.Rpc.Codec.Serializer;

namespace Simp.Rpc.Codec
{

    public class SimpleTypeCodec : ITypeCodec<SimpleParameter>
    {
        private static ISerializer _serializer; //= new ProtoBufSerializer();
         
        public SimpleTypeCodec(ISerializer serializer)
        {
            _serializer = serializer;
        }

        public object DeCode(byte[] bytesValue, int typeCode, Type objType)
        {
            if (typeCode == (int)TypeCode.DBNull)
            {
                return null;
            }

            if (typeCode == (int)TypeCode.Object)
            {
                return _serializer.Deserialize(bytesValue, objType);
            }

            return _serializer.Deserialize(bytesValue, Type.GetType("System." + Enum.GetName(typeof(TypeCode), typeCode)));
        }

        public object[] Decode(SimpleParameter[] encodeParameters, Type[] decodeTypes)
        {
            object[] args = { };
            if (encodeParameters != null && encodeParameters.Any())
            {
                if (encodeParameters.Length != decodeTypes.Length)
                    throw new ArgumentException("参数个数不匹配");

                args = new object[encodeParameters.Length];
                for (int i = 0; i < encodeParameters.Length; i++)
                {
                    args[i] = DeCode(encodeParameters[i].Value, encodeParameters[i].ValueType, decodeTypes[i]);
                }
            }
            return args;
        }

        public byte[] EnCode(object objectValue, out int typeCode)
        {
            if (objectValue == null)
            {
                typeCode = (int)TypeCode.DBNull;
                return new byte[] { 0 };
            }

            var valueType = objectValue.GetType();
            typeCode = (int)Type.GetTypeCode(valueType);
            return _serializer.Serialize(objectValue);
        }

    }
}