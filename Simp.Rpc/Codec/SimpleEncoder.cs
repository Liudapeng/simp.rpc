// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Simp.Rpc.Codec;

namespace Common
{ 
    using System.Text;
    using DotNetty.Buffers;
    using DotNetty.Codecs;
    using DotNetty.Transport.Channels; 

    public class SimpleEncoder<T> : MessageToByteEncoder<T>
    {
        readonly byte[] HeaderBytes;
        private static readonly byte[] CRLF = { (byte)'\r', (byte)'\n' };
        readonly ISerializer Serializer;

        public SimpleEncoder(byte[] headerBytes, ISerializer serializer)
        {
            this.HeaderBytes = headerBytes;
            this.Serializer = serializer;
        }

        public SimpleEncoder(ISerializer serializer) : this(Encoding.ASCII.GetBytes("SimpleHeader"), serializer)
        {  
        }

        protected override void Encode(IChannelHandlerContext context, T message, IByteBuffer output)
        {
            byte[] bytes = Serializer.Serialize(message);
            output.WriteBytes(this.HeaderBytes);
            output.WriteBytes(new[] { Serializer.GetSerializType() });
            output.WriteBytes(Encoding.ASCII.GetBytes(bytes.Length.ToString()));
            output.WriteBytes(CRLF);
            output.WriteBytes(bytes);
        }
 
    }
}