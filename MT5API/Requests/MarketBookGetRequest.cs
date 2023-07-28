namespace MT5API.Requests
{
    internal class MarketBookGetRequest: RequestBase
    {
        public override RequestType RequestType => RequestType.MarketBookGet;

        public string Symbol { get; set; }
    }
}