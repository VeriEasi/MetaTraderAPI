using System;

namespace MT5API
{
    public class MT5QuoteEventArgs: EventArgs
    {
        public MT5Quote Quote { get; }

        public MT5QuoteEventArgs(MT5Quote quote)
        {
            Quote = quote;
        }
    }
}
