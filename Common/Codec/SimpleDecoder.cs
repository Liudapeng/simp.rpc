// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Common.Codec;

namespace Common
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using DotNetty.Buffers;
    using DotNetty.Codecs;
    using DotNetty.Transport.Channels;

    public class SimpleDecoder<T> : ReplayingDecoder<SimpleDecoder<T>.DecodeState>
    {
        public enum DecodeState
        {
            HEADER,//头
            CODERTYPE,//编码类型,Json\Proto
            LENGTH,//BODY长度
            CRLF,
            BODY,// 
        }

        private readonly byte[] HeaderBytes;
        private int BodyLength;
        private byte SerializeType;
        private readonly ISerializer Serializer;

        public SimpleDecoder(ISerializer serializer) : this(Encoding.ASCII.GetBytes("SimpleHeader"), serializer)
        {
        }

        public SimpleDecoder(byte[] headerBytes, ISerializer serializer) : base(DecodeState.HEADER)
        {
            this.HeaderBytes = headerBytes;
            this.Serializer = serializer;
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            switch (this.State)
            {
                case DecodeState.HEADER:
                    this.TryDecodeHeader(context, input, output);
                    break;

                case DecodeState.CODERTYPE:
                    this.TryDecodeCoderType(context, input, output);
                    break;

                case DecodeState.LENGTH:
                    this.TryDecodeLength(context, input, output);
                    break;

                case DecodeState.CRLF:
                    this.TryDecodeCrlf(context, input, output);
                    break;

                case DecodeState.BODY:
                    this.TryDecodeBody(context, input, output);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void TryDecodeBody(IChannelHandlerContext context, IByteBuffer buf, List<object> output)
        {
            if (!buf.IsReadable(this.BodyLength))
            {
                this.RequestReplay();
                return;
            }

            var bytes = new byte[this.BodyLength];
            buf.ReadBytes(bytes);
            output.Add(Serializer.Deserialize<T>(bytes));
            this.Checkpoint(DecodeState.HEADER);
        }

        private void TryDecodeCrlf(IChannelHandlerContext context, IByteBuffer buf, List<object> output)
        {
            if (!buf.IsReadable(2))
            {
                this.RequestReplay();
                return;
            }

            if (buf.ReadByte() != '\r' || buf.ReadByte() != '\n')
            {
                throw new ArgumentException("Crlf error");
            }

            this.Checkpoint(DecodeState.BODY);
        }

        private void TryDecodeLength(IChannelHandlerContext context, IByteBuffer buf, List<object> output)
        {
            if (!buf.IsReadable())
            {
                this.RequestReplay();
                return;
            }

            int endIndex = ByteBufferUtil.IndexOf(buf, buf.ReaderIndex, buf.ReaderIndex + buf.ReadableBytes, (byte)'\r');
            int num = endIndex < 0 ? -1 : endIndex - buf.ReaderIndex;
            if (num == -1)
            {
                this.RequestReplay();
                return;
            }
            var bytes = new byte[num];
            buf.ReadBytes(bytes);
            this.BodyLength = Convert.ToInt32(Encoding.ASCII.GetString(bytes));
            this.Checkpoint(DecodeState.CRLF);
        }

        private void TryDecodeCoderType(IChannelHandlerContext context, IByteBuffer buf, List<object> output)
        {
            if (!buf.IsReadable())
            {
                this.RequestReplay();
                return;
            }

            var bytes = new byte[1];
            buf.ReadBytes(bytes);
            SerializeType = bytes[0];
            this.Checkpoint(DecodeState.LENGTH);
        }

        private void TryDecodeHeader(IChannelHandlerContext context, IByteBuffer buf, List<object> output)
        {
            if (!buf.IsReadable(this.HeaderBytes.Length))
            {
                this.RequestReplay();
                return;
            }

            var bytes = new byte[this.HeaderBytes.Length];
            buf.ReadBytes(bytes);
            if (!equals(bytes, this.HeaderBytes))
            {
                throw new ArgumentException("Header error");
            }
            this.Checkpoint(DecodeState.CODERTYPE);
        }

        public static bool equals(byte[] a, byte[] a2)
        {
            if (a == a2)
                return true;
            if (a == null || a2 == null)
                return false;

            int length = a.Length;
            if (a2.Length != length)
                return false;

            for (int i = 0; i < length; i++)
                if (a[i] != a2[i])
                    return false;

            return true;
        }


    }
}