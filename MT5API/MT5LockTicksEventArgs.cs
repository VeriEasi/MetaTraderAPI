using System;

namespace MT5API
{
    public class MT5LockTicksEventArgs : EventArgs
    {
        internal MT5LockTicksEventArgs(string symbol)
        {
            Symbol = symbol;
        }

        public string Symbol { get; }
    }
}