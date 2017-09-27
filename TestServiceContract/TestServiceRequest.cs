using ProtoBuf;

namespace TestServiceContract
{ 
    [ProtoContract]
    public class TestServiceRequest
    {
        [ProtoMember(1)]
        public int IntValue { get; set; } = 111;

        [ProtoMember(2)]
        public string StringValue { get; set; } = "111";

        [ProtoMember(3)]
        public float FloatValue { get; set; } = 11.1f;
    }
}