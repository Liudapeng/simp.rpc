 
using ProtoBuf;

namespace Simp.Rpc
{ 

    [ProtoContract]
    public class SimpleResponseMessage
    {
        [ProtoMember(1)]
        public bool Success { get; set; }

        [ProtoMember(2)]
        public string ErrorInfo { get; set; }

        [ProtoMember(3)]
        public long ServerTime { get; set; }

        [ProtoMember(4)]
        public int ErrorCode { get; set; }

        [ProtoMember(5)]
        public string ErrorDetail { get; set; }

        [ProtoMember(6)]
        public string MessageID { get; set; }

        [ProtoMember(7)]
        public string ContextID { get; set; }
    }
}