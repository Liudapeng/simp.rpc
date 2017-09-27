using ProtoBuf;

namespace TestServiceContract
{
    [ProtoContract]
    public class TestServiceResponse
    {
        [ProtoMember(1)]
        public int IntValue { get; set; }

        [ProtoMember(2)]
        public string StringValue { get; set; }

        [ProtoMember(3)]
        public float FloatValue { get; set; }

        public override string ToString()
        {
            return $"{IntValue}-{StringValue}-{FloatValue}";
        }
    }
}