using System;

namespace MT4API
{
    public class MT4LockTicksEventArgs : EventArgs
    {
        internal MT4LockTicksEventArgs(int expertHandle, string symbol)
        {
            ExpertHandle = expertHandle;
            Symbol = symbol;
        }

        public int ExpertHandle { get; }
        public string Symbol { get; }
    }
}