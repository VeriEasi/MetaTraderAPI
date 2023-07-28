namespace MT5API.Events
{
    internal class OnTradeTransactionEvent
    {
        public MQLTradeTransaction Trans { get; set; }
        public MQLTradeRequest Request { get; set; }
        public MQLTradeResult Result { get; set; }
    }
}