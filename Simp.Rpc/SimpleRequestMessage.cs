
using System;
using System.ComponentModel;
using Simp.Rpc.Codec;
using ProtoBuf;

namespace Simp.Rpc
{ 

    [ProtoContract]
    public class SimpleRequestMessage
    {
        [ProtoMember(1)]
        public string ClientName { get; set; }

        [ProtoMember(2)]
        public string ServiceName { get; set; }

        [ProtoMember(3)]
        public string MethodName { get; set; }

        [ProtoMember(4)]
        public string ContextID { get; set; }

        [ProtoMember(5)]
        public string MessageID { get; set; }

        [ProtoMember(6)]
        public string ServerName { get; set; }

        [ProtoMember(7)]
        public SimpleParameter[] Parameters { get; set; }
    }

    [ProtoContract]
    public class SimpleParameter
    {
        [ProtoMember(1)]
        public int ValueType { get; set; }

        [ProtoMember(2)]
        public byte[] Value { get; set; }
         
    } 
}