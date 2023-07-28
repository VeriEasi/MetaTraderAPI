namespace MT5API.Requests
{
    internal class BuyRequest : RequestBase
    {
        public override RequestType RequestType => RequestType.Buy;

        public double Volume { get; set; }
        public string Symbol { get; set; }
        public double Price { get; set; }
        public double SL { get; set; }
        public double TP { get; set; }
        public string Comment { get; set; }
    }
}
