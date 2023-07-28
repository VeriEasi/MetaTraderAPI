using System;
using MTAPIService;

namespace MT5API
{
    public class MT5Quote
    {
        public string Instrument { get; }
        public double Bid { get; }
        public double Ask { get; }
        public int ExpertHandle { get; set; }
        public DateTime Time { get; set; }
        public double Last { get; set; }
        public ulong Volume { get; set; }
//        public long TimeMsc { get; set; }
//        public uint Flags { get; set; }

        internal MT5Quote(string instrument, double bid, double ask)
        {
            Instrument = instrument;
            Bid = bid;
            Ask = ask;
        }

        internal MT5Quote(MTQuote quote)
            :this(quote.Instrument, quote.Bid, quote.Ask)
        {
            ExpertHandle = quote.ExpertHandle;
        }
    }
}
