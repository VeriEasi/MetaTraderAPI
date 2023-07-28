namespace MT5API.Requests
{
    internal class OrderCheckRequest: RequestBase
    {
        public override RequestType RequestType => RequestType.OrderCheck;

        public MQLTradeRequest TradeRequest { get; set; }

    }
}