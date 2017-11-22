using System;

namespace Simp.Rpc.Codec
{
    /// <summary>
    /// 类型编解码器, 用于解释跨语言时的基本类型编码
    /// </summary>
    public interface ITypeCodec<in T>
    {
        byte[] EnCode(object objectValue, out int typeCode);

        object DeCode(byte[] bytesValue, int typeCode, Type objType);

        object[] Decode(T[] encodeParameters, Type[] decodeTypes);
    }
}