namespace MT5API.Requests
{
    internal class OrderSendRequest: RequestBase
    {
        public override RequestType RequestType => RequestType.OrderSend;

        public MQLTradeRequest TradeRequest { get; set; }
    }
}
