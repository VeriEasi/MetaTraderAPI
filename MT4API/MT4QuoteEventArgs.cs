using System;

namespace MT4API
{
    public class MT4QuoteEventArgs : EventArgs
    {
        public MT4Quote Quote { get; private set; }

        public MT4QuoteEventArgs(MT4Quote quote)
        {
            Quote = quote;
        }
    }
}
