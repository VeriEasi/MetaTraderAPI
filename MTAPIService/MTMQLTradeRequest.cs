using System;
using System.Runtime.Serialization;

namespace MTAPIService
{
    [DataContract]
    public class MTMQLTradeRequest
    {
        [DataMember]
        public int Action { get; set; }
        [DataMember]
        public ulong Magic { get; set; }
        [DataMember]
        public ulong Order { get; set; }
        [DataMember]
        public string Symbol { get; set; }
        [DataMember]
        public double Volume { get; set; }
        [DataMember]
        public double Price { get; set; }
        [DataMember]
        public double StopLimit { get; set; }
        [DataMember]
        public double SL { get; set; }
        [DataMember]
        public double TP { get; set; }
        [DataMember]
        public ulong Deviation { get; set; }
        [DataMember]
        public int Type { get; set; }
        [DataMember]
        public int TypeFilling { get; set; }
        [DataMember]
        public int TypeTime { get; set; }
        [DataMember]
        public DateTime Expiration { get; set; }
        [DataMember]
        public string Comment { get; set; }
        [DataMember]
        public ulong Position { get; set; }
        [DataMember]
        public ulong PositionBy { get; set; }
    }
}
