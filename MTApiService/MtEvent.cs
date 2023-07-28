using System.Runtime.Serialization;

namespace MTAPIService
{
    [DataContract]
    public class MTEvent
    {
        [DataMember]
        public int EventType { get; internal set; }

        [DataMember]
        public string Payload { get; internal set; }

        [DataMember]
        public int ExpertHandle { get; internal set; }

        public override string ToString()
        {
            return $"EventType = {EventType}; Payload = {Payload}; ExpertHandle = {ExpertHandle}";
        }
    }
}
