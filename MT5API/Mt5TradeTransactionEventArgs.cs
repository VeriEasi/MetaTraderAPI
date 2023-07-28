using System;

namespace MT5API
{
    public class Mt5TradeTransactionEventArgs : EventArgs
    {
        public int ExpertHandle { get; set; }
        public MQLTradeTransaction Trans { get; set; }  // trade transaction structure 
        public MQLTradeRequest Request { get; set; }    // request structure
        public MQLTradeResult Result { get; set; }      // result structure 
    }
}