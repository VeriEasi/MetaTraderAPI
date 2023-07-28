using System;

namespace MT5API
{
    public class MT5TimeBarArgs: EventArgs
    {
        internal MT5TimeBarArgs(int expertHandle, string symbol, MQLRates rates)
        {
            ExpertHandle = expertHandle;
            Rates = rates;
            Symbol = symbol;
        }

        public int ExpertHandle { get; }
        public string Symbol { get; }
        public MQLRates Rates { get; }
    }
}
