using ProtoBuf;

namespace TestServiceContract
{ 
    [ProtoContract]
    public class TestServiceRequest2
    {
        [ProtoMember(1)]
        public int IntValue { get; set; } = 333;

        [ProtoMember(2)]
        public string StringValue { get; set; } = "333";

        [ProtoMember(3)]
        public float FloatValue { get; set; } = 33.3f;
    }
}