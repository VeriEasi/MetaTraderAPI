using System.Runtime.Serialization;

namespace MTAPIService
{
    [DataContract]
    public class MTQuote
    {
        [DataMember]
        public string Instrument { get; internal set; }
        
        [DataMember]
        public double Bid { get; internal set; }

        [DataMember]
        public double Ask { get; internal set; }

        [DataMember]
        public int ExpertHandle { get; internal set; }

        public override string ToString()
        {
            return $"Instrument = {Instrument}; Bid = {Bid}; Ask = {Ask}; ExpertHandle = {ExpertHandle}";
        }
    }
}
