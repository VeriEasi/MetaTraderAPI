using System.Runtime.Serialization;

namespace MTAPIService
{
    [DataContract]
    public class MTMQLRates
    {
        [DataMember]
        public long Time { get; set; }         // Period start time
        [DataMember]
        public double Open { get; set; }         // Open price
        [DataMember]
        public double High { get; set; }         // The highest price of the period
        [DataMember]
        public double Low { get; set; }          // The lowest price of the period
        [DataMember]
        public double Close { get; set; }        // Close price
        [DataMember]
        public long TickVolume { get; set; }  // Tick volume
        [DataMember]
        public int Spread { get; set; }       // Spread
        [DataMember]
        public long RealVolume { get; set; }  // Trade volume
    }
}
