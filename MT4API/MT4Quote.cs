namespace MT4API
{
    public class MT4Quote
    {
        public string Instrument { get; private set; }
        public double Bid { get; private set; }
        public double Ask { get; private set; }
        public int ExpertHandle { get; private set; }

        public MT4Quote(string instrument, double bid, double ask)
        {
            Instrument = instrument;
            Bid = bid;
            Ask = ask;
        }

        internal MT4Quote(MTAPIService.MTQuote quote)
        {
            Instrument = quote.Instrument;
            Bid = quote.Bid;
            Ask = quote.Ask;
            ExpertHandle = quote.ExpertHandle;
        }
    }
}
