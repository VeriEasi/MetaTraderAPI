namespace MT5API.Requests
{
    internal class OrderSendAsyncRequest : RequestBase
    {
        public override RequestType RequestType => RequestType.OrderSendAsync;

        public MQLTradeRequest TradeRequest { get; set; }
    }
}